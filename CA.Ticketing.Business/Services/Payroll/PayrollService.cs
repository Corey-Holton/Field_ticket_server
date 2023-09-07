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

            var payrollData = ExtractPayrollData(tickets.ToList());

            var groupedPayrollData = GroupPayrollDataByEmployee(payrollData);

            var finalList = await CalculateTotals(groupedPayrollData.ToList());

            return finalList.ToList();
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

        private List<EmployeePayrollDataDto> ExtractPayrollData(List<TicketDetailsDto> tickets)
        {
            var payrollData = new List<EmployeePayrollDataDto>();

            foreach (var ticket in tickets)
            {
                foreach (var payrollItem in ticket.PayrollData)
                {
                    payrollData.Add(new EmployeePayrollDataDto
                    {
                        ExecutionTime = ticket.ExecutionDate,
                        Employee = $"{payrollItem.Employee.FirstName} {payrollItem.Employee.LastName}",
                        EmployeeId = (int)payrollItem.EmployeeId,
                        RegularHours = ticket.CompanyHours,
                        OvertimeHours = 0,
                        Mileage = ticket.Mileage,
                        TotalHours = 0,
                        TotalMileage = 0,
                        TotalAmount = 0
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
                    Mileage = group.Sum(item => item.Mileage)
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


        private async Task<IEnumerable<EmployeePayrollDataDto>> CalculateTotals(List<EmployeePayrollDataDto> data)
        {
            var settings = await GetSettings();
            var list = data.Select(async item =>
            {
                var employee = await GetEmployee(item.EmployeeId);
                item.TotalMileage = settings.MileageCost * item.Mileage;
                item.TotalHours = item.RegularHours * employee.PayRate;
                item.TotalAmount = item.TotalHours + item.TotalMileage;
                return item;
            }).ToList();
            await Task.WhenAll(list);
            return list.Select(x => x.Result);
        }
    }
}
