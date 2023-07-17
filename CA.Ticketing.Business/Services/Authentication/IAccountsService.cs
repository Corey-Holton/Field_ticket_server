using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.Employees.Dto;

namespace CA.Ticketing.Business.Services.Authentication
{
    public interface IAccountsService
    {
        Task<AuthenticationResultDto> Authenticate(LoginDto loginModel);

        Task ResetPassword(ResetPasswordDto resetPasswordModel);

        Task SetPasswordFromLink(SetPasswordDto setPasswordModel);

        Task SetPassword(SetPasswordDto setPasswordModel);

        Task ChangePassword(ChangePasswordDto changePasswordModel);

        Task CreateEmployeeLogin(CreateEmployeeLoginDto employeeLoginModel);

        Task DeleteEmployeeLogin(int employeeId);

        Task ResetEmployeePassword(ResetEmployeePasswordDto resetEmployeePasswordModel);

        Task CreateCustomerContactLogin(CreateCustomerContactLoginDto customerContactLoginModel);

        Task ResendCustomerContactEmail(ResendInviteDto resendInviteModel);

        Task<AuthenticationResultDto> SetCustomerPassword(SetCustomerPasswordDto setCustomerPasswordModel);

        Task ResetCustomerContactPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel);

        Task DeleteCustomerContactLogin(int customerContactId);
        
    }
}
