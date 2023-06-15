using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus status);

        Task<EmployeeDto> GetById(int id);

        Task<int> Create(EmployeeDto entity);

        Task Update(EmployeeDto entity);

        Task Delete(int id);
    }
}
