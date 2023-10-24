using CA.Ticketing.Business.Services.Payroll.Dto;

namespace CA.Ticketing.Business.Services.Payroll
{
    public interface IPayrollService
    {
        Task<IEnumerable<EmployeePayrollDataDto>> GetPayrollData(DateTime startTime, DateTime endTime);

        Task<IEnumerable<EmployeeReportingDataDto>> GetHoursReport(DateTime startTime, DateTime endTime);
    }
}
