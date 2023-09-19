using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CA.Ticketing.Business.Services.Employees
{
    public class EmployeeService : EntityServiceBase, IEmployeeService
    {
        private readonly IAccountsService _accountsService;

        public EmployeeService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base(context, mapper)
        {
            _accountsService = accountsService;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus? status)
        {
            IQueryable<Employee> employeesQuery = _context.Employees
                .Include(x => x.ApplicationUser);

            if (status.HasValue)
            {
                var terminationDateCutoff = GetEndofDay(DateTime.Now);
                Expression<Func<Employee, bool>> statusFilter = status == EmployeeStatus.Active ?
                    x => !x.TerminationDate.HasValue || x.TerminationDate > terminationDateCutoff :
                    x => x.TerminationDate.HasValue && x.TerminationDate < terminationDateCutoff;

                employeesQuery = employeesQuery
                    .Where(statusFilter);
            }

            var employees = await employeesQuery
                .ToListAsync();

            return employees.Select(x => _mapper.Map<EmployeeDto>(x));
        }

        public async Task<EmployeeDetailsDto> GetById(string id)
        {
            var employee = await GetEmployee(id);
            return _mapper.Map<EmployeeDetailsDto>(employee);
        }

        public async Task<string> Create(EmployeeDetailsDto entity)
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

        public async Task Delete(string id)
        {
            var employee = await GetEmployee(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDateDto>> GetEmployeesBirthdays()
        {
            return (await _context.Employees.ToListAsync())
                .Where(x => DayTimeExtensions.IsWithinMonth(x.DoB))
                .Select(x => _mapper.Map<EmployeeDateDto>(x));
        }

        public async Task<IEnumerable<EmployeeDateDto>> GetEmployeesAnniversaries()
        {
            return (await _context.Employees.ToListAsync())
                .Where(x => DayTimeExtensions.IsWithinMonth(x.HireDate))
                .Select(x => _mapper.Map<EmployeeDateDto>(x));
        }

        public async Task AddLogin(AddEmployeeLoginDto addEmployeeLoginModel)
        {
            var employee = await GetEmployee(addEmployeeLoginModel.Id);

            if (employee.ApplicationUser != null)
            {
                throw new Exception("Employee already has a login");
            }

            var createUserDto = _mapper.Map<CreateEmployeeLoginDto>((employee, addEmployeeLoginModel));

            await _accountsService.AddEmployeeLogin(createUserDto);
        }

        public async Task ResetPassword(ResetEmployeePasswordDto resetEmployeePasswordModel)
        {
            var employee = await GetEmployee(resetEmployeePasswordModel.Id);
            
            if (employee.ApplicationUser == null)
            {
                return;
            }

            await _accountsService.ResetUserPassword(new ResetUserPasswordDto { UserId = employee.ApplicationUser.Id, Password = resetEmployeePasswordModel.Password });
        }

        public async Task DeleteLogin(string id)
        {
            var employee = await GetEmployee(id);

            if (employee.ApplicationUser == null)
            {
                return;
            }

            await _accountsService.DeleteUser(employee.ApplicationUser.Id);
        }

        private async Task<Employee> GetEmployee(string? id)
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

        private DateTime GetEndofDay(DateTime initialDate)
        {
            var date = initialDate.AddDays(1);
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }
    }
}
