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
        Task<IEnumerable<CustomerDetailsDto>> GetAll();

        Task<CustomerDetailsDto> GetById(int id);

        Task<int> Create(CustomerDetailsDto entity);

        Task Update(CustomerDto entity);

        Task Delete(int id);

        Task<IEnumerable<CustomerLocationDto>> GetCustomerLocations(int id);

        Task<int> AddLocation(AddLocationDto entity);

        Task UpdateLocation(AddLocationDto entity);

        Task DeleteLocation(int id);

        Task<int> AddContact(AddContactDto entity);

        Task UpdateContact(AddContactDto entity);

        Task DeleteContact(int id);

        Task AddLogin(AddCustomerLoginDto loginDto);

        Task AddPassword(AddCustomerContactPasswordDto addCustomerContactPasswordModel);

        Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordDto);

        Task DeleteLogin(int customerContactId);
    }
}
