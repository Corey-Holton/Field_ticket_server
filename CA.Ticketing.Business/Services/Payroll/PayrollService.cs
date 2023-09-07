using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees;
using CA.Ticketing.Business.Services.Payroll.Dto;
using CA.Ticketing.Business.Services.Settings.Dto;
using CA.Ticketing.Business.Services.Tickets;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Payroll
{
    public class PayrollService : EntityServiceBase, IPayrollService
    {

        public PayrollService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<EmployeePayrollDataDto>> GetPayrollData(DateTime startTime, DateTime endTime)
        {
            await Task.Delay(10);

            var tickets = await GetTicketDetailsByDates(startTime, endTime);

            var payrollData = await ExtractPayrollData(tickets.ToList());

            var groupedPayrollData = GroupPayrollDataByEmployee(payrollData.ToList());

            return groupedPayrollData.ToList();
        }

        private async Task<IEnumerable<TicketDetailsDto>> GetTicketDetailsByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await _context.FieldTickets
               .Include(x => x.PayrollData)
               .ThenInclude(p => p.Employee)
               .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
               .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDetailsDto>(x));
        }

        private async Task<IEnumerable<EmployeePayrollDataDto>> ExtractPayrollData(List<TicketDetailsDto> tickets)
        {
            var payrollData = new List<EmployeePayrollDataDto>();
            var settings = await GetSettings();

            foreach (var ticket in tickets)
            {
                foreach (var payrollItem in ticket.PayrollData)
                {
                    var employee = await GetEmployee((int)payrollItem.EmployeeId);
                    payrollData.Add(new EmployeePayrollDataDto
                    {
                        ExecutionTime = ticket.ExecutionDate,
                        Employee = $"{payrollItem.Employee.FirstName} {payrollItem.Employee.LastName}",
                        EmployeeId = (int)payrollItem.EmployeeId,
                        RegularHours = ticket.CompanyHours,
                        OvertimeHours = 0,
                        Mileage = ticket.Mileage,
                        TotalHours = ticket.CompanyHours * employee.PayRate,
                        TotalMileage = ticket.Mileage * settings.MileageCost,
                        TotalAmount = ticket.CompanyHours * employee.PayRate + ticket.Mileage * settings.MileageCost
                    });
                }
            }
            return payrollData;
        }

        private IEnumerable<EmployeePayrollDataDto> GroupPayrollDataByEmployee(List<EmployeePayrollDataDto> payrollData)
        {
            return payrollData
                .GroupBy(item => item.EmployeeId)
                .Select(group => new EmployeePayrollDataDto
                {
                    EmployeeId = group.Key,
                    Employee = group.First().Employee,
                    RegularHours = group.Sum(item => item.RegularHours),
                    Mileage = group.Sum(item => item.Mileage),
                    TotalHours = group.Sum(item => item.TotalHours),
                    TotalMileage = group.Sum(item => item.TotalMileage),
                    TotalAmount = group.Sum(item => item.TotalAmount)
                });
        }

        private async Task<SettingDto> GetSettings()
        {
            var setting = await _context.Settings.FirstAsync();
            return _mapper.Map<SettingDto>(setting);
        }

        private async Task<Employee> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .SingleOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                throw new KeyNotFoundException(nameof(Employee));
            }

            return employee!;
        }
    }
}
