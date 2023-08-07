using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IAccountsService _accountsService;

        public UsersController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        /// <summary>
        /// List Users
        /// </summary>
        /// <returns>List of Users</returns>
        [Route(ApiRoutes.Users.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var users = await _accountsService.GetUsers();
            return Ok(users);
        }

        ///<summary>
        /// Create User
        /// </summary>
        /// <param name="createUserDto">CreateUserDto</param>
        [Route(ApiRoutes.Users.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateUserDto createUserDto)
        {
            var result = await _accountsService.CreateUser(createUserDto);
            return Ok(result);
        }

        ///<summary>
        /// Update User
        /// </summary>
        /// <param name="updateUserDto">UpdateUserDto</param>
        [Route(ApiRoutes.Users.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateUserDto updateUserDto)
        {
            await _accountsService.UpdateUser(updateUserDto);
            return Ok();
        }

        ///<summary>
        /// Delete User
        /// </summary>
        /// <param name="userId">User Id</param>
        [Route(ApiRoutes.Users.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string userId)
        {
            await _accountsService.DeleteUser(userId);
            return Ok();
        }

        ///<summary>
        /// Reset User password
        /// </summary>
        /// <param name="resetUserPasswordDto">ResetUserPasswordDto</param>
        [Route(ApiRoutes.Users.ResetPassword)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetUserPassword(ResetUserPasswordDto resetUserPasswordDto)
        {
            await _accountsService.ResetUserPassword(resetUserPasswordDto);
            return Ok();
        }
    }
}
