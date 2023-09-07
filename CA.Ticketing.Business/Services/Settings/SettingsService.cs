using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Settings.Dto;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Settings
{
    public class SettingsService : EntityServiceBase, ISettingsService
    {
        private readonly IUserContext _userContext;

        public SettingsService(CATicketingContext context, IMapper mapper, IUserContext userContext) : base(context, mapper)
        {
            _userContext = userContext;
        }

        public async Task<SettingDto> Get()
        {
            var setting = await _context.Settings.FirstAsync();
            return _mapper.Map<SettingDto>(setting);
        }

        public async Task Update(SettingDto settingDto)
        {
            var setting = await _context.Settings.FirstAsync();
            _mapper.Map(settingDto, setting);
            await _context.SaveChangesAsync();
        }

        public async Task<ProfileDto> GetProfile()
        {
            var userId = _userContext.User!.Id;
            var user = await _context.Users.SingleAsync(x => x.Id == userId);
            return _mapper.Map<ProfileDto>(user);
        }

        public async Task UpdateProfile(ProfileDto profile)
        {
            var userId = _userContext.User!.Id;
            var user = await _context.Users.SingleAsync(x => x.Id == userId);

            _mapper.Map(profile, user);

            await _context.SaveChangesAsync();
        }
    }
}
