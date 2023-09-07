using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Settings;
using CA.Ticketing.Business.Services.Settings.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Get Settings
        /// </summary>
        /// <returns>Settings Data</returns>
        [Route(ApiRoutes.Settings.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(SettingDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _settingsService.Get();
            return Ok(settings);
        }

        ///<summary>
        /// Edit settings
        /// </summary>
        /// <param name="settingDto">SettingDto</param>
        [Route(ApiRoutes.Settings.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(SettingDto settingDto)
        {
            await _settingsService.Update(settingDto);
            return Ok();
        }

        ///<summary>
        /// Get Profile
        /// </summary>
        [Route(ApiRoutes.Settings.GetProfile)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _settingsService.GetProfile();
            return Ok(profile);
        }

        ///<summary>
        /// Update Profile
        /// </summary>
        /// <param name="profileDto">ProfileDto</param>
        [Route(ApiRoutes.Settings.UpdateProfile)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
        {
            await _settingsService.UpdateProfile(profileDto);
            return Ok();
        }
    }
}
