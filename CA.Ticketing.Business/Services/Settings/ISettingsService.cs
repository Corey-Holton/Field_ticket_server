using CA.Ticketing.Business.Services.Settings.Dto;

namespace CA.Ticketing.Business.Services.Settings
{
    public interface ISettingsService
    {
        Task<SettingDto> Get();

        Task Update(SettingDto setting);

        Task<ProfileDto> GetProfile();

        Task UpdateProfile(ProfileDto profile);
    }
}
