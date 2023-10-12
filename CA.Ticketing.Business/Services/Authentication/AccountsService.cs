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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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

        private readonly ServerConfiguration _serverConfiguration;

        public AccountsService(
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService, 
            IUserContext userContext, 
            MessagesComposer messagesComposer,
            IOptions<SecuritySettings> securitySettings,
            CATicketingContext context,
            IOptions<ServerConfiguration> serverConfiguration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
            _userContext = userContext;
            _messagesComposer = messagesComposer;
            _securitySettings = securitySettings.Value;
            _context = context;
            _serverConfiguration = serverConfiguration.Value;
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

            var userRoles = await _userManager.GetRolesAsync(user);

            if (!_serverConfiguration.IsMainServer)
            {
                var userRole = userRoles.First();
                if (userRole != RoleNames.ToolPusher)
                {
                    return new AuthenticationResultDto("Only tool pushers can login in the desktop application");
                }
            }

            if (string.IsNullOrEmpty(user.RefreshToken))
            {
                user.RefreshToken = GenerateRefreshToken();
                user.LastModifiedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            var claims = GetUserClaims(user, userRoles);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id, user.RefreshToken);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task<AuthenticationResultDto> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);

            if (userPrincipal == null)
            {
                throw new Exception("Invalid request");
            }

            var user = await _userManager.FindByIdAsync(userPrincipal.Identity!.Name);

            if (user == null)
            {
                return new AuthenticationResultDto();
            }

            if (user.RefreshToken != refreshTokenDto.RefreshToken)
            {
                throw new Exception("Invalid token request");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = GetUserClaims(user, userRoles);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id, user.RefreshToken);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task<string> GetEmployeeResetPasswordToken(EmployeePasswordResetDto employeePasswordResetDto)
        {
            var user = await _context.Users
                .Include(x => x.Employee)
                .SingleOrDefaultAsync(x => x.NormalizedUserName == employeePasswordResetDto.Username.ToUpper());

            if (user == null || user.Employee == null || user.Employee.SSN.ToUpper() != employeePasswordResetDto.SSN)
            {
                throw new Exception("There was an error with password reset");
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task SetUserPassword(SetPasswordDto setPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(setPasswordDto.Username);

            if (user == null)
            {
                throw new Exception("There was an issue while reseting password");
            }

            user.LastModifiedDate = DateTime.UtcNow;

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new Exception("Can't use this code again. Please use Forgot password link instead");
            }

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, setPasswordDto.Code);

            if (!emailConfirmationResult.Succeeded)
            {
                throw new Exception($"There was an error while setting password. {string.Join("; ", emailConfirmationResult.Errors.Select(x => x.Description))}.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, setPasswordDto.Password);

            if (!addPasswordResult.Succeeded)
            {
                throw new Exception($"There was an error while setting password. {string.Join("; ", addPasswordResult.Errors.Select(x => x.Description))}.");
            }
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

            user.LastModifiedDate = DateTime.UtcNow;

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

            existingUser = await _userManager.FindByNameAsync(customerContactLoginModel.Email);

            if (existingUser != null)
            {
                throw new Exception($"There is already a user with username {customerContactLoginModel.Email}");
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
            user.LastModifiedDate = DateTime.UtcNow;
            await SendUserInvite(user, customerLoginDto.RedirectUrl);
        }

        public async Task<AuthenticationResultDto> SetCustomerPassword(SetCustomerPasswordDto setCustomerPasswordModel)
        {
            var user = await _userManager.FindByNameAsync(setCustomerPasswordModel.Email);

            if (user == null)
            {
                return new AuthenticationResultDto();
            }

            if (!await _userManager.HasPasswordAsync(user))
            {
                throw new Exception("User has already set the password");
            }

            user.LastModifiedDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(user.RefreshToken))
            {
                user.RefreshToken = GenerateRefreshToken();
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

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = GetUserClaims(user, userRoles);

            var authenticatedUser = GetAuthenticatedUser(claims, !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email, user.Id, user.RefreshToken);

            return new AuthenticationResultDto(authenticatedUser);
        }

        public async Task ResetCustomerContactPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel)
        {
            var user = await _userManager.FindByCustomerContactIdAsync(resetCustomerContactPasswordModel.CustomerContactId);
            await ResetUserPasswordInternal(user!, resetCustomerContactPasswordModel.Password);
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordModel)
        {
            var user = await _userManager.FindByNameAsync(resetPasswordModel.Email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Send email here;
            await SendResetPasswordEmail(user, resetPasswordModel.RedirectUrl);
        }

        public async Task SetPasswordFromLink(SetPasswordDto setPasswordModel)
        {
            var user = await _userManager.FindByNameAsync(setPasswordModel.Username);

            if (user == null)
            {
                throw new Exception("There was an issue while reseting password");
            }

            user.LastModifiedDate = DateTime.UtcNow;
            
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, setPasswordModel.Code, setPasswordModel.Password);

            if (!resetPasswordResult.Succeeded)
            {
                throw new Exception($"There was an error while resetting password. {string.Join("; ", resetPasswordResult.Errors.Select(x => x.Description))}.");
            }
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
            user.LastModifiedDate = DateTime.UtcNow;
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
            user.LastModifiedDate = DateTime.UtcNow;
            await ResetUserPasswordInternal(user, resetUserPasswordDto.Password);
        }

        private IEnumerable<Claim> GetUserClaims(ApplicationUser user, IList<string> userRoles)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            var role = userRoles.First();
            userClaims.Add(new Claim(ClaimTypes.Role, role));

            if (role == RoleNames.ToolPusher)
            {
                var employee = _context.Employees.Single(x => x.Id == user.EmployeeId);
                if (!string.IsNullOrEmpty(employee.AssignedRigId))
                {
                    userClaims.Add(new Claim(CAClaims.RigId, employee.AssignedRigId));
                }
            }

            if (!string.IsNullOrEmpty(user.TicketIdentifier))
            {
                userClaims.Add(new Claim(CAClaims.TicketIdentifier, user.TicketIdentifier));
            }

            if (role == RoleNames.Customer && !string.IsNullOrEmpty(user.CustomerContactId))
            {
                userClaims.Add(new Claim(CAClaims.CustomerContactId, user.CustomerContactId));
            }

            return userClaims;
        }

        private AuthenticatedUser GetAuthenticatedUser(IEnumerable<Claim> userClaims, string displayName, string userId, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_securitySettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _securitySettings.Issuer,
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var initials = GetInitials(displayName);
            return new AuthenticatedUser()
            {
                Id = userId,
                DisplayName = displayName,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                Initials = initials
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var key = Encoding.ASCII.GetBytes(_securitySettings.Secret);
            var tokenValidationParameters = AuthenticationExtensions.GetTokenValidationParameters(false, key, _securitySettings.Issuer);

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string GetInitials(string displayName)
        {
            string[] words = displayName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string initials = string.Join("", words.Select(word => word[0]));
            return initials;
        }

        private async Task SendUserInvite(ApplicationUser user, string redirectUrl)
        {
            var emailConfirmationToken = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));

            var callBackUrl = $"{redirectUrl}?code={emailConfirmationToken}&username={user.UserName}";

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.InviteUser, (1, new string[] { $"{user.DisplayName}" }), (4, callBackUrl));

            await _notificationService.SendEmail(user.Email, emailMessage);
        }

        private async Task SendResetPasswordEmail(ApplicationUser user, string redirectUrl)
        {
            user.LastModifiedDate = DateTime.UtcNow;

            var emailPasswordResetToken = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

            var callBackUrl = $"{redirectUrl}?code={emailPasswordResetToken}&username={user.UserName}";

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.ResetPassword, (3, callBackUrl));

            await _notificationService.SendEmail(user.Email, emailMessage);
        }

        private async Task<string> CreateUserInternal(ApplicationUser user, string password, string role)
        {
            user.LastModifiedDate = DateTime.UtcNow;
            user.RefreshToken = GenerateRefreshToken();
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
            user.LastModifiedDate = DateTime.UtcNow;
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);

            if (!resetPasswordResult.Succeeded)
            {
                throw new Exception($"There was an error while resetting user password. {string.Join("; ", resetPasswordResult.Errors.Select(x => x.Description))}.");
            }
        }
    }
}
