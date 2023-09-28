using AutoMapper;
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
            // Get settings for calculation
            var settings = await _context.Settings.FirstAsync();

            // Catch end of the day of the end date
            endTime = endTime.AddDays(1);

            // Get all tickets
            var tickets = await _context.FieldTickets
                .Include(x => x.TicketSpecifications)
                .Include(x => x.PayrollData)
                    .ThenInclude(p => p.Employee)
                .Where(x => x.ExecutionDate >= startTime && x.ExecutionDate <= endTime)
                .AsSplitQuery()
                .ToListAsync();

            return tickets
                // Select all PayrollData from tickets
                .SelectMany(ticket => ticket.PayrollData
                    // Transform Payroll data
                    .Select(payrollEntry =>
                        {
                            // Charge company or input hours
                            var chargeCompanyHours = ticket.ServiceType < ServiceType.Roustabout;
                            
                            // Special logic for Tool pushers
                            var isToolPusher = payrollEntry.Employee?.JobTitle == JobTitle.ToolPusher;

                            // Return anonymous object for calculation
                            return new
                            {
                                // Set grouping identifier
                                GroupingIdentifier = payrollEntry.EmployeeId?.ToString() ?? payrollEntry.Name.ToLower(),
                                // Payroll data can contain Employee or just a name
                                Name = payrollEntry.Employee?.DisplayName ?? payrollEntry.Name,
                                Rate = payrollEntry.Employee?.PayRate ?? 20,
                                payrollEntry.RigHours,
                                payrollEntry.TravelHours,
                                payrollEntry.YardHours,
                                payrollEntry.RoustaboutHours,
                                ticket.CompanyHours,
                                // Hours charged only calculated if not a tool pusher for company hours or input hours for 
                                HoursCharged = (ticket.ServiceType < ServiceType.Yard) ? (!isToolPusher ? payrollEntry.RigHours + payrollEntry.TravelHours - ticket.CompanyHours : 0)
                                       : (ticket.ServiceType == ServiceType.Yard) ? payrollEntry.YardHours - ticket.CompanyHours
                                       : (ticket.ServiceType == ServiceType.Roustabout) ? payrollEntry.RoustaboutHours - ticket.CompanyHours
                                       : 0,
                                // Set week for the entry
                                Week = (int)Math.Ceiling((ticket.ExecutionDate - startTime).TotalDays / 7),
                                ticket.Mileage,
                                IsToolPusher = isToolPusher,
                                // Get Tool pusher rate from specifications
                                ToolPusherRate = !chargeCompanyHours || !isToolPusher ? 0 :
                                    ticket.TicketSpecifications
                                        .FirstOrDefault(x => x.Charge == ChargeNames.ToolPusher)?.Rate ?? 0
                            };
                        }))
                // Group by set identifier
                .GroupBy(x => x.GroupingIdentifier)
                // Calculate totals
                .Select(employeePayrollData => 
                {
                    var payRate = employeePayrollData.First().Rate;
                    var name = employeePayrollData.First().Name;

                    var companyHours = employeePayrollData.Sum(x => x.CompanyHours);
                    var rigHours = employeePayrollData.Sum(x => x.RigHours);
                    var travelHours = employeePayrollData.Sum(x => x.TravelHours);
                    var yardHours = employeePayrollData.Sum(x => x.YardHours);
                    var roustaboutHours = employeePayrollData.Sum(x => x.RoustaboutHours);

                    roustaboutHours = roustaboutHours > 0 ? roustaboutHours - companyHours : roustaboutHours;
                    rigHours = rigHours > 0 ? rigHours - companyHours : rigHours;
                    yardHours = yardHours > 0 ? yardHours - companyHours : yardHours;

                    // Get total hours for each week
                    var perWeekTotals = employeePayrollData
                        .GroupBy(x => x.Week)
                        .Select(w => w.Sum(g => g.HoursCharged));

                    // Total hours for employee
                    var totalHours = rigHours + travelHours + yardHours + roustaboutHours;

                    // Get overtime hours by calculating 
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
    }
}
