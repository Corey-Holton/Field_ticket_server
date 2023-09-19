using CA.Ticketing.Business.Services.Customers.Dto;

namespace CA.Ticketing.Business.Services.Customers
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAll();

        Task<CustomerDetailsDto> GetById(string id);

        Task<string> Create(CustomerDetailsDto entity);

        Task Update(CustomerDetailsDto entity);

        Task Delete(string id);

        Task<string> AddLocation(CustomerLocationDto entity);

        Task UpdateLocation(CustomerLocationDto entity);

        Task DeleteLocation(string id);

        Task<string> AddContact(CustomerContactDto entity);

        Task UpdateContact(CustomerContactDto entity);

        Task DeleteContact(string id);

        Task AddLogin(CustomerLoginDto loginDto);

        Task ResendInvitation(CustomerLoginDto customerLoginDto);

        Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordDto);

        Task DeleteLogin(string customerContactId);
    }
}
