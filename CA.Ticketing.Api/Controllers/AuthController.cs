using CA.Ticketing.Api.Extensions;
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
        /// Authenticate user with email instead of username
        /// </summary>
        /// <param name="loginModel">loginModel</param>
        /// <returns>AuthenticatedUser</returns>
        [Route(ApiRoutes.Authentication.EmailLogin)]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticatedUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> EmailLogin(EmailLoginDto loginModel)
        {
            var authUser = await _accountsService.EmailAuthenticate(loginModel);
            return Ok(authUser);
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
    }
}
