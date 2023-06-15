using AutoMapper;
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
        public EmployeeService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<EmployeeDto>> GetAll(EmployeeStatus status)
        {
            var employees = await _context.Employees
                .Where(x => x.Status == status)
                .ToListAsync();
            return employees.Select(x => _mapper.Map<EmployeeDto>(x));
        }

        public async Task<EmployeeDto> GetById(int id)
        {
            var employee = await GetEmployee(id);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<int> Create(EmployeeDto entity)
        {
            var employee = _mapper.Map<Employee>(entity);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee.Id;
        }

        public async Task Update(EmployeeDto entity)
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
