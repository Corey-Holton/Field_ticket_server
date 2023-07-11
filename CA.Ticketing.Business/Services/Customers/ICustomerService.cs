using CA.Ticketing.Business.Services.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAll();

        Task<CustomerDetailsDto> GetById(int id);

        Task<int> Create(CustomerDetailsDto entity);

        Task Update(CustomerDetailsDto entity);
    }
}
