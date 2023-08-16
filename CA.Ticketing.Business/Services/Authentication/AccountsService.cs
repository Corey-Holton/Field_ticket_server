using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Business.Services.Notifications;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Common.Setup;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CA.Ticketing.Business.Services.Authentication
{
    public class AccountsService : IAccountsService
    {
        private readonly IMapper _mapper;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly CATicketingContext _context;

        private readonly INotificationService _notificationService;

        private readonly IUserContext _userContext;

        private readonly MessagesComposer _messagesComposer;

        private readonly SecuritySettings _securitySettings;

        public AccountsService(
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService, 
            IUserContext userContext, 
            MessagesComposer messagesComposer,
            IOptions<SecuritySettings> securitySettings,
            CATicketingContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
            _userContext = userContext;
            _messagesComposer = messagesComposer;
            _securitySettings = securitySettings.Value;
            _context = context;
        }

        public async Task<AuthenticationResultDto> Authenticate(LoginDto loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null)
            {
                return new AuthenticationResultDto();
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!checkPassword)
            {
                return new AuthenticationResultDto();
            }

            var claims = await GetUserClaims(user);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task<AuthenticationResultDto> EmailAuthenticate(EmailLoginDto loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                return new AuthenticationResultDto();
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!checkPassword)
            {
                return new AuthenticationResultDto();
            }

            var claims = await GetUserClaims(user);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task ChangePassword(ChangePasswordDto changePasswordModel)
        {
            var userId = _userContext.User?.Id;

            if (userId == null)
            {
                throw new Exception("User not found");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

            if (!result.Succeeded)
            {
                throw new Exception($"There was an error while changing password. {string.Join("; ", result.Errors.Select(x => x.Description))}");
            }
        }

        public async Task CreateCustomerContactLogin(CreateCustomerContactLoginDto customerContactLoginModel)
        {
            var existingUser = await _userManager.FindByCustomerContactIdAsync(customerContactLoginModel.CustomerContactId);

            if (existingUser != null)
            {
                throw new Exception($"User for Customer Contact Id {customerContactLoginModel.CustomerContactId} was already created");
            }

            existingUser = await _userManager.FindByEmailAsync(customerContactLoginModel.Email);

            if (existingUser != null)
            {
                throw new Exception($"There is already a user with email {customerContactLoginModel.Email}");
            }

            var userAdded = _mapper.Map<ApplicationUser>(customerContactLoginModel);

            await CreateUserInternal(userAdded, string.Empty, RoleNames.Customer);

            await SendUserInvite(userAdded, customerContactLoginModel.RedirectUrl);
        }

        public async Task ResendCustomerContactEmail(CustomerLoginDto customerLoginDto)
        {
            var user = await _userManager.FindByCustomerContactIdAsync(customerLoginDto.CustomerContactId);

            if (user == null)
            {
                throw new Exception($"User with id {customerLoginDto.CustomerContactId} was not found");
            }

            await SendUserInvite(user, customerLoginDto.RedirectUrl);
        }

        public async Task<AuthenticationResultDto> SetCustomerPassword(SetCustomerPasswordDto setCustomerPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(setCustomerPasswordModel.Email);

            if (user == null)
            {
                return new AuthenticationResultDto();
            }

            if (!await _userManager.HasPasswordAsync(user))
            {
                throw new Exception("User has already set the password");
            }

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, setCustomerPasswordModel.Code);

            if (!emailConfirmationResult.Succeeded)
            {
                throw new Exception($"There was an error while confirming email address. {string.Join("; ", emailConfirmationResult.Errors.Select(x => x.Description))}.");
            }

            var setPasswordResult = await _userManager.AddPasswordAsync(user, setCustomerPasswordModel.Password);

            if (!setPasswordResult.Succeeded)
            {
                throw new Exception($"There was an error while setting user password. {string.Join("; ", setPasswordResult.Errors.Select(x => x.Description))}.");
            }

            var claims = await GetUserClaims(user);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task ResetCustomerContactPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel)
        {
            var user = await _userManager.FindByCustomerContactIdAsync(resetCustomerContactPasswordModel.CustomerContactId);
            await ResetUserPasswordInternal(user!, resetCustomerContactPasswordModel.Password);
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Send email here;
            await SendResetPasswordEmail(user, resetPasswordModel.RedirectUrl);
        }

        public async Task SetPasswordFromLink(SetPasswordDto setPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(setPasswordModel.Email);

            await _userManager.ResetPasswordAsync(user, setPasswordModel.Code, setPasswordModel.Password);
        }

        public Task SetPassword(SetPasswordDto setPasswordModel)
        {
            throw new NotImplementedException();
        }

        public async Task AddEmployeeLogin(CreateEmployeeLoginDto createEmployeeLoginDto)
        {
            var user = _mapper.Map<ApplicationUser>(createEmployeeLoginDto);
            await CreateUserInternal(user, createEmployeeLoginDto.Password, RoleNames.ToolPusher);
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var result = new List<UserDto>();

            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var userMapped = _mapper.Map<UserDto>(user);
                userMapped.Role = (await _userManager.GetRolesAsync(user)).First();
                result.Add(userMapped);
            }

            return result;
        }

        public async Task<string> CreateUser(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<ApplicationUser>(createUserDto);
            return await CreateUserInternal(user, createUserDto.Password, createUserDto.Role.GetRoleName());
        }

        public async Task UpdateUser(UpdateUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            _mapper.Map(userDto, user);
            await _userManager.UpdateAsync(user);

            var currentRole = (await _userManager.GetRolesAsync(user)).First();

            if (currentRole != userDto.Role.GetRoleName())
            {
                var roleRemovalResult = await _userManager.RemoveFromRoleAsync(user, currentRole);

                if (!roleRemovalResult.Succeeded)
                {
                    throw new Exception($"There was an error while removing user from role. {string.Join("; ", roleRemovalResult.Errors.Select(x => x.Description))}.");
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, userDto.Role.GetRoleName());

                if (!addRoleResult.Succeeded)
                {
                    throw new Exception($"There was an error while adding user to role. {string.Join("; ", addRoleResult.Errors.Select(x => x.Description))}.");
                }
            }
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);
        }

        public async Task ResetUserPassword(ResetUserPasswordDto resetUserPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(resetUserPasswordDto.UserId);
            await ResetUserPasswordInternal(user, resetUserPasswordDto.Password);
        }

        private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            var role = userRoles.First();
            userClaims.Add(new Claim(ClaimTypes.Role, role));

            if (role == RoleNames.ToolPusher)
            {
                var employee = _context.Employees.Single(x => x.Id == user.EmployeeId);
                if (employee.AssignedRigId.HasValue)
                {
                    userClaims.Add(new Claim(CAClaims.RigId, employee.AssignedRigId.Value.ToString()));
                }
            }

            if (!string.IsNullOrEmpty(user.TicketIdentifier))
            {
                userClaims.Add(new Claim(CAClaims.TicketIdentifier, user.TicketIdentifier));
            }

            return userClaims;
        }

        private AuthenticatedUser GetAuthenticatedUser(IEnumerable<Claim> userClaims, string displayName, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_securitySettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticatedUser()
            {
                Id = userId,
                DisplayName = displayName,
                Token = tokenHandler.WriteToken(token)
            };
        }

        private async Task SendUserInvite(ApplicationUser user, string redirectUrl)
        {
            var emailConfirmationToken = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));

            var callBackUrl = $"{redirectUrl}?code={emailConfirmationToken}&email={user.Email}";

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.InviteUser, (3, callBackUrl));

            await _notificationService.SendEmail(user.Email, emailMessage);
        }

        private async Task SendResetPasswordEmail(ApplicationUser user, string redirectUrl)
        {
            var emailPasswordResetToken = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

            var callBackUrl = $"{redirectUrl}?code={emailPasswordResetToken}";

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.ResetPassword, (3, callBackUrl));

            await _notificationService.SendEmail(user.Email, emailMessage);
        }

        private async Task<string> CreateUserInternal(ApplicationUser user, string password, string role)
        {
            var userCreateResult = !string.IsNullOrEmpty(password) ? 
                await _userManager.CreateAsync(user, password) : 
                await _userManager.CreateAsync(user);

            if (!userCreateResult.Succeeded)
            {
                throw new Exception($"There was an error while creating user. {string.Join("; ", userCreateResult.Errors.Select(x => x.Description))}.");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, role);

            if (!addRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new Exception($"There was an error while adding user to role. {string.Join("; ", addRoleResult.Errors.Select(x => x.Description))}.");
            }

            return user.Id;
        }

        private async Task ResetUserPasswordInternal(ApplicationUser user, string password)
        {
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);

            if (!resetPasswordResult.Succeeded)
            {
                throw new Exception($"There was an error while resetting user password. {string.Join("; ", resetPasswordResult.Errors.Select(x => x.Description))}.");
            }
        }
    }
}
