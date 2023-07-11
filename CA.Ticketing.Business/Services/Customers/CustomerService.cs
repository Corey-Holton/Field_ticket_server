using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Context;
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
    }
}
