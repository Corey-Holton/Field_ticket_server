using CA.Ticketing.Business.Services.Customers.Dto;

namespace CA.Ticketing.Business.Services.Customers
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAll();

        Task<CustomerDetailsDto> GetById(int id);

        Task<int> Create(CustomerDetailsDto entity);

        Task Update(CustomerDetailsDto entity);

        Task Delete(int id);

        Task<int> AddLocation(CustomerLocationDto entity);

        Task UpdateLocation(CustomerLocationDto entity);

        Task DeleteLocation(int id);

        Task<int> AddContact(CustomerContactDto entity);

        Task UpdateContact(CustomerContactDto entity);

        Task DeleteContact(int id);

        Task AddLogin(CustomerLoginDto loginDto);

        Task ResendInvitation(CustomerLoginDto customerLoginDto);

        Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordDto);

        Task DeleteLogin(int customerContactId);
    }
}
