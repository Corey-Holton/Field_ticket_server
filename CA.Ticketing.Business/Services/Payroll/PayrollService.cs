using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Payroll.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CA.Ticketing.Business.Services.Payroll
{
    public class PayrollService : EntityServiceBase, IPayrollService
    {

        public PayrollService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<EmployeePayrollDataDto>> GetPayrollData(DateTime startTime, DateTime endTime)
        {
            var settings = await _context.Settings.FirstAsync();

            endTime = endTime.AddDays(1);

            var tickets = await _context.FieldTickets
                .Include(x => x.TicketSpecifications.Where(ts => ts.Charge == ChargeNames.ToolPusher))
                .Include(x => x.PayrollData)
                    .ThenInclude(p => p.Employee)
                .Where(x => x.ExecutionDate >= startTime && x.ExecutionDate < endTime)
                .AsSplitQuery()
                .ToListAsync();

            return tickets
                .SelectMany(ticket => ticket.PayrollData
                    .Select(payrollEntry =>
                    {
                        var isRigWork = ticket.IsRigWork();

                        var isToolPusher = payrollEntry.Employee?.JobTitle == JobTitle.ToolPusher;

                        var CalculateHours = () => {
                            double hoursTotal = 0;
                            if (isRigWork)
                            {
                                // Tool pushers are paid per job, not hours
                                hoursTotal = !isToolPusher ? payrollEntry.RigHours : 0;
                            }
                            else
                            {
                                hoursTotal = ticket.IsServiceType(ServiceType.Roustabout)
                                    ? payrollEntry.RoustaboutHours
                                    : payrollEntry.YardHours;
                            }

                            if (!isToolPusher)
                            {
                                hoursTotal += payrollEntry.TravelHours;
                            }

                            if (hoursTotal > ticket.CompanyHours && !isToolPusher)
                            {
                                // Reducing with company hours for all workers except for Tool pusher
                                hoursTotal -= ticket.CompanyHours;
                            }

                            return hoursTotal;
                        };

                        var TrySubtractCompanyHours = (double hoursToReduce) =>
                        {
                            if (hoursToReduce <= ticket.CompanyHours || isToolPusher)
                            {
                                return hoursToReduce;
                            }

                            return hoursToReduce - ticket.CompanyHours;
                        };

                        var toolPusherSpec = ticket.TicketSpecifications
                                    .FirstOrDefault(x => x.Charge == ChargeNames.ToolPusher);

                        return new
                        {
                            GroupingIdentifier = payrollEntry.EmployeeId?.ToString() ?? payrollEntry.Name.ToLower(),
                            Name = payrollEntry.Employee?.DisplayName ?? payrollEntry.Name,
                            Rate = payrollEntry.Employee?.PayRate ?? 20,
                            RigHours = isRigWork ? TrySubtractCompanyHours(payrollEntry.RigHours) : 0,
                            TravelHours = isRigWork ? payrollEntry.TravelHours : 0,
                            YardHours = ticket.IsServiceType(ServiceType.Yard)
                                ? TrySubtractCompanyHours(payrollEntry.YardHours) : 0,
                            RoustaboutHours = ticket.IsServiceType(ServiceType.Roustabout)
                                ? TrySubtractCompanyHours(payrollEntry.RoustaboutHours) : 0,
                            HoursToPay = CalculateHours(),
                            Week = (int)Math.Ceiling((ticket.ExecutionDate - startTime).TotalDays / 7),
                            ticket.Mileage,
                            // Get tool pusher rate for any Rig work if the worker is Tool Pusher
                            ToolPusherRate = !isRigWork || !isToolPusher ? 0 :
                                (toolPusherSpec?.Rate * toolPusherSpec?.Quantity) ?? 0
                        };
                    }))
                .GroupBy(x => x.GroupingIdentifier)
                .Select(employeePayrollData =>
                {
                    var payRate = employeePayrollData.First().Rate;
                    var name = employeePayrollData.First().Name;

                    var rigHours = employeePayrollData.Sum(x => x.RigHours);
                    var travelHours = employeePayrollData.Sum(x => x.TravelHours);
                    var yardHours = employeePayrollData.Sum(x => x.YardHours);
                    var roustaboutHours = employeePayrollData.Sum(x => x.RoustaboutHours);

                    var perWeekTotals = employeePayrollData
                        .GroupBy(x => x.Week)
                        .Select(w => w.Sum(g => g.HoursToPay));

                    var totalHours = employeePayrollData.Sum(x => x.HoursToPay);

                    var weeklyRegularHours = 40;
                    var overTimeHours = perWeekTotals
                        .Where(x => x > weeklyRegularHours).Sum(x => x - weeklyRegularHours);

                    // Set regular hours based on overtime hours
                    var regularHours = totalHours - overTimeHours;

                    // Mileage calculation
                    var mileage = employeePayrollData.Sum(x => x.Mileage);
                    var totalMileage = mileage * settings.MileageCost;

                    // Set Jobs count
                    var jobsCount = employeePayrollData.Count();

                    // Set total from jobs for ToolPusher
                    var totalJobs = employeePayrollData.Sum(x => x.ToolPusherRate);

                    // Set amount total
                    var amountTotal = totalMileage +
                        totalJobs +
                        regularHours * payRate +
                        overTimeHours * payRate * (1 + settings.OvertimePercentageIncrease / 100);

                    return new EmployeePayrollDataDto()
                    {
                        Employee = name,
                        TotalHours = totalHours,
                        Jobs = jobsCount,
                        TotalJobs = totalJobs,
                        RegularHours = regularHours,
                        OvertimeHours = overTimeHours,
                        RigHours = rigHours,
                        TravelHours = travelHours,
                        YardHours = yardHours,
                        RoustaboutHours = roustaboutHours,
                        Mileage = mileage,
                        TotalMileage = totalMileage,
                        TotalAmount = amountTotal
                    };
                });
        }

        public async Task<IEnumerable<EmployeeReportingDataDto>> GetHoursReport(DateTime startTime, DateTime endTime)
        {
            endTime = endTime.AddDays(1);

            var tickets = await _context.FieldTickets
                .Include(x => x.PayrollData)
                    .ThenInclude(p => p.Employee)
                .Where(x => x.ExecutionDate >= startTime && x.ExecutionDate < endTime)
                .AsSplitQuery()
                .ToListAsync();

            return tickets
                .SelectMany(ticket => ticket.PayrollData
                    .Select(payrollEntry =>
                    {
                        var isRigWork = ticket.IsRigWork();

                        var isToolPusher = payrollEntry.Employee?.JobTitle == JobTitle.ToolPusher;

                        var CalculateHours = () => {
                            double hoursTotal = 0;
                            if (isRigWork)
                            {
                                hoursTotal = payrollEntry.RigHours;
                            }
                            else
                            {
                                hoursTotal = ticket.IsServiceType(ServiceType.Roustabout)
                                    ? payrollEntry.RoustaboutHours
                                    : payrollEntry.YardHours;
                            }

                            if (hoursTotal > ticket.CompanyHours && !isToolPusher)
                            {
                                // Reducing with company hours for all workers except for Tool pusher
                                hoursTotal -= ticket.CompanyHours;
                            }

                            return hoursTotal;
                        };

                        var TrySubtractCompanyHours = (double hoursToReduce) =>
                        {
                            if (hoursToReduce <= ticket.CompanyHours || isToolPusher)
                            {
                                return hoursToReduce;
                            }

                            return hoursToReduce - ticket.CompanyHours;
                        };

                        return new
                        {
                            GroupingIdentifier = payrollEntry.EmployeeId?.ToString() ?? payrollEntry.Name.ToLower(),
                            Name = payrollEntry.Employee?.DisplayName ?? payrollEntry.Name,
                            RigHours = isRigWork ? TrySubtractCompanyHours(payrollEntry.RigHours) : 0,
                            TravelHours = isRigWork ? payrollEntry.TravelHours : 0,
                            YardHours = ticket.IsServiceType(ServiceType.Yard)
                                ? TrySubtractCompanyHours(payrollEntry.YardHours) : 0,
                            RoustaboutHours = ticket.IsServiceType(ServiceType.Roustabout)
                                ? TrySubtractCompanyHours(payrollEntry.RoustaboutHours) : 0,
                            Week = (int)Math.Ceiling((ticket.ExecutionDate - startTime).TotalDays / 7),
                            ticket.Mileage
                        };
                    }))
                .GroupBy(x => x.GroupingIdentifier)
                .Select(employeePayrollData =>
                {
                    var name = employeePayrollData.First().Name;

                    var rigHours = employeePayrollData.Sum(x => x.RigHours);
                    var travelHours = employeePayrollData.Sum(x => x.TravelHours);
                    var yardHours = employeePayrollData.Sum(x => x.YardHours);
                    var roustaboutHours = employeePayrollData.Sum(x => x.RoustaboutHours);

                    var totalHours = rigHours + travelHours + yardHours + roustaboutHours;

                    // Mileage calculation
                    var mileage = employeePayrollData.Sum(x => x.Mileage);

                    // Set Jobs count
                    var jobsCount = employeePayrollData.Count();

                    return new EmployeeReportingDataDto()
                    {
                        Employee = name,
                        TotalHours = totalHours,
                        Jobs = jobsCount,
                        RigHours = rigHours,
                        TravelHours = travelHours,
                        YardHours = yardHours,
                        RoustaboutHours = roustaboutHours,
                        Mileage = mileage
                    };
                });
        }
    }
}
