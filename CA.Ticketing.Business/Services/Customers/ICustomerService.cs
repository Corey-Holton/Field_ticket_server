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

        Task Delete(int id);

        Task<int> AddLocation(AddLocationDto entity);

        Task UpdateLocation(AddLocationDto entity);

        Task DeleteLocation(int id);

        Task AddLogin(int customerId);

        Task AddPassword(AddCustomerContactPasswordDto addCustomerContactPasswordModel);

        Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordDto);

        Task DeleteLogin(int customerContactId);
    }
}
