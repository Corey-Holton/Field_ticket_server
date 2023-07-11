using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Customers
{
    public class CustomerService : EntityServiceBase, ICustomerService
    {
        public CustomerService(CATicketingContext context, IMapper mapper) : base (context, mapper)
        {
            
        }

        public async Task<IEnumerable<CustomerDto>> GetAll()
        {
            var customers = await _context.Customers
                .ToListAsync();
            return customers.Select(x => _mapper.Map<CustomerDto>(x));
        }

        public async Task<CustomerDetailsDto> GetById(int id)
        {
            var customer = await GetCustomer(id);
            return _mapper.Map<CustomerDetailsDto>(customer);
        }

        public async Task<int> Create(CustomerDetailsDto entity)
        {
            var customer = _mapper.Map<Customer>(entity);
            
            if(entity.Locations != null)
                customer.Locations = _mapper.Map<List<CustomerLocation>>(entity.Locations);
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task Update(CustomerDetailsDto entity)
        {
            var customer = await GetCustomer(entity.Id);
            
            _mapper.Map(entity, customer);
            
            if(entity.Locations != null)
                customer.Locations = _mapper.Map<List<CustomerLocation>>(entity.Locations);
            
            await _context.SaveChangesAsync();
        }

        private async Task<Customer> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(x => x.Locations)
                .ThenInclude(x => x.Contacts)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                throw new KeyNotFoundException(nameof(Customer));
            }

            return customer!;
        }
    }
}
