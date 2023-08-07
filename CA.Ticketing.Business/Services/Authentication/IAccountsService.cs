using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Customers.Dto;

namespace CA.Ticketing.Business.Services.Authentication
{
    public interface IAccountsService
    {
        Task<AuthenticationResultDto> Authenticate(LoginDto loginModel);

        Task<AuthenticationResultDto> EmailAuthenticate(EmailLoginDto loginModel);

        Task ResetPassword(ResetPasswordDto resetPasswordModel);

        Task SetPasswordFromLink(SetPasswordDto setPasswordModel);

        Task SetPassword(SetPasswordDto setPasswordModel);

        Task ChangePassword(ChangePasswordDto changePasswordModel);

        Task AddEmployeeLogin(CreateEmployeeLoginDto createEmployeeLoginDto);

        Task CreateCustomerContactLogin(CreateCustomerContactLoginDto customerContactLoginModel);

        Task ResendCustomerContactEmail(ResendInviteDto resendInviteModel);

        Task<AuthenticationResultDto> SetCustomerPassword(SetCustomerPasswordDto setCustomerPasswordModel);

        Task ResetCustomerContactPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel);

        Task DeleteCustomerContactLogin(int customerContactId);

        Task<IEnumerable<UserDto>> GetUsers();

        Task UpdateUser(UpdateUserDto userDto);

        Task<string> CreateUser(CreateUserDto createUserDto);

        Task DeleteUser(string userId);

        Task ResetUserPassword(ResetUserPasswordDto resetUserPasswordDto);
    }
}
