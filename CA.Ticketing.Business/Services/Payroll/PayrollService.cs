using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Payroll.Dto;
using CA.Ticketing.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Payroll
{
    public class PayrollService : EntityServiceBase, IPayrollService
    {
        private readonly List<EmployeePayrollDataDto> _payrollData = new()
        {
            new EmployeePayrollDataDto { ExecutionTime = new DateTime(2023, 07, 05), Employee = "John Doe", RegularHours = 40, OvertimeHours = 12, Mileage = 10, TotalHours = 52, TotalMileage = 220.40, TotalAmount = 1200.40 },
            new EmployeePayrollDataDto { ExecutionTime = new DateTime(2023, 08, 04), Employee = "John Ingles", RegularHours = 40, OvertimeHours = 20, Mileage = 30, TotalHours = 52, TotalMileage = 220.40, TotalAmount = 1400.40 },
            new EmployeePayrollDataDto { ExecutionTime = new DateTime(2023, 07, 05), Employee = "Sam Hopes", RegularHours = 40, OvertimeHours = 12, Mileage = 10, TotalHours = 52, TotalMileage = 220.40, TotalAmount = 1200.40 },
            new EmployeePayrollDataDto { ExecutionTime = new DateTime(2023, 08, 04), Employee = "Adam Garza", RegularHours = 40, OvertimeHours = 20, Mileage = 30, TotalHours = 52, TotalMileage = 220.40, TotalAmount = 1400.40 }
        };

        public PayrollService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<EmployeePayrollDataDto>> GetPayrollData(DateTime startTime, DateTime endTime)
        {
            await Task.Delay(10);
            return _payrollData.Where(x => x.ExecutionTime > startTime && x.ExecutionTime < endTime).ToList();
        }
    }
}
