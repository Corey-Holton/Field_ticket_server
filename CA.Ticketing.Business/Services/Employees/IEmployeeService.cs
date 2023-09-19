using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus? status);

        Task<EmployeeDetailsDto> GetById(string id);

        Task<string> Create(EmployeeDetailsDto entity);

        Task Update(EmployeeDetailsDto entity);

        Task Delete(string id);

        Task AddLogin(AddEmployeeLoginDto addEmployeeLoginModel);

        Task ResetPassword(ResetEmployeePasswordDto resetEmployeePasswordModel);

        Task DeleteLogin(string id);

        Task<IEnumerable<EmployeeDateDto>> GetEmployeesBirthdays();

        Task<IEnumerable<EmployeeDateDto>> GetEmployeesAnniversaries();
    }
}
