using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus status);

        Task<EmployeeDetailsDto> GetById(int id);

        Task<int> Create(EmployeeDetailsDto entity);

        Task Update(EmployeeDetailsDto entity);

        Task Delete(int id);

        Task AddLogin(AddEmployeeLoginDto addEmployeeLoginModel);

        Task ResetPassword(ResetEmployeePasswordDto resetEmployeePasswordModel);

        Task DeleteLogin(int id);
    }
}
