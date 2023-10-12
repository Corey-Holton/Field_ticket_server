using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountsService _accountsService;

        public AuthController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="loginModel">loginModel</param>
        /// <returns>AuthenticatedUser</returns>
        [Route(ApiRoutes.Authentication.Login)]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticatedUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginDto loginModel)
        {
            var authUser = await _accountsService.Authenticate(loginModel);
            return Ok(authUser);
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="refreshTokenDto">Refresh Token Model</param>
        /// <returns>AuthenticatedUser</returns>
        [Route(ApiRoutes.Authentication.RefreshToken)]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticatedUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var authUser = await _accountsService.RefreshToken(refreshTokenDto);
            return Ok(authUser);
        }

        /// <summary>
        /// Get Employee Reset password token
        /// </summary>
        /// <param name="employeeResetPasswordDto">Employee Password Reset Dto</param>
        /// <returns>AuthenticatedUser</returns>
        [Route(ApiRoutes.Authentication.GetEmployeeResetPasswordToken)]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployeeResetPasswordToken(EmployeePasswordResetDto employeeResetPasswordDto)
        {
            var token = await _accountsService.GetEmployeeResetPasswordToken(employeeResetPasswordDto);
            return Ok(token);
        }

        /// <summary>
        /// Generate reset password link
        /// </summary>
        /// <param name="resetPasswordModel"></param>
        [Route(ApiRoutes.Authentication.GenerateResetPasswordLink)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GeneratePasswordResetLink(ResetPasswordDto resetPasswordModel)
        {
            await _accountsService.ResetPassword(resetPasswordModel);
            return Ok();
        }

        /// <summary>
        /// Set password from reset link
        /// </summary>
        /// <param name="setPasswordDto"></param>
        [Route(ApiRoutes.Authentication.SetPasswordFromLink)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetPasswordFromLink(SetPasswordDto setPasswordDto)
        {
            await _accountsService.SetPasswordFromLink(setPasswordDto);
            return Ok();
        }

        /// <summary>
        /// Set Customer password
        /// </summary>
        /// <param name="setPasswordDto"></param>
        [Route(ApiRoutes.Authentication.AddPassword)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetCustomerPassword(SetPasswordDto setPasswordDto)
        {
            await _accountsService.SetUserPassword(setPasswordDto);
            return Ok();
        }

        /// <summary>
        /// Change own password
        /// </summary>
        /// <param name="changePasswordDto">ChangePasswordDto</param>
        [Route(ApiRoutes.Authentication.ChangePassword)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            await _accountsService.ChangePassword(changePasswordDto);
            return Ok();
        }
    }
}
