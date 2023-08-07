using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Employees
{
    public class EmployeeService : EntityServiceBase, IEmployeeService
    {
        private readonly IAccountsService _accountsService;

        public EmployeeService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base(context, mapper)
        {
            _accountsService = accountsService;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus status)
        {
            var employees = await _context.Employees
                .Include(x => x.ApplicationUser)
                .Where(x => x.Status == status)
                .ToListAsync();
            return employees.Select(x => _mapper.Map<EmployeeDto>(x));
        }

        public async Task<EmployeeDetailsDto> GetById(int id)
        {
            var employee = await GetEmployee(id);
            return _mapper.Map<EmployeeDetailsDto>(employee);
        }

        public async Task<int> Create(EmployeeDetailsDto entity)
        {
            var employee = _mapper.Map<Employee>(entity);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee.Id;
        }

        public async Task Update(EmployeeDetailsDto entity)
        {
            var employee = await GetEmployee(entity.Id);

            _mapper.Map(entity, employee);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var employee = await GetEmployee(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDateDto>> GetEmployeesBirthdays()
        {
            var employeesUnfiltered = await _context.Employees.ToListAsync();
            var employees = new List<Employee>();
            foreach (var employee in employeesUnfiltered)
            {
                if (IsWithinMonth(employee.DoB))
                    employees.Add(employee);

            }
            return employees.Select(x => _mapper.Map<EmployeeDateDto>(x));
        }

        public async Task<IEnumerable<EmployeeDateDto>> GetEmployeesAnniversaries()
        {
            var employeesUnfiltered = await _context.Employees.ToListAsync();
            var employees = new List<Employee>();
            foreach (var employee in employeesUnfiltered)
            {
                if (IsWithinMonth(employee.HireDate))
                    employees.Add(employee);

            }
            return employees.Select(x => _mapper.Map<EmployeeDateDto>(x));
        }

        private bool IsWithinMonth(DateTime? date)
        {
            if (date != null)
            {
                var birthday = new DateTime(DateTime.Now.Year, date.Value.Month, date.Value.Day);
                var difference = birthday - DateTime.Now;
                if (difference.TotalDays <= 30 && difference.TotalDays >= 0)
                    return true;
                else return false;
            }
            return false;
        }

        private async Task<Employee> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                throw new KeyNotFoundException(nameof(Employee));
            }

            return employee!;
        }

        public async Task AddLogin(AddEmployeeLoginDto addEmployeeLoginModel)
        {
            var employee = await GetEmployee(addEmployeeLoginModel.Id);

            var createEmployeeLoginModel = _mapper.Map<CreateEmployeeLoginDto>((employee, addEmployeeLoginModel));

            await _accountsService.CreateEmployeeLogin(createEmployeeLoginModel);
        }

        public async Task ResetPassword(ResetEmployeePasswordDto resetEmployeePasswordModel)
        {
            await _accountsService.ResetEmployeePassword(resetEmployeePasswordModel);
        }

        public async Task DeleteLogin(int id)
        {
            await _accountsService.DeleteEmployeeLogin(id);
        }
    }
}
