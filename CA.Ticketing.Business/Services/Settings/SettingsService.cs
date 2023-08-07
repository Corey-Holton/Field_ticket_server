using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Settings.Dto;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Settings
{
    public class SettingsService : EntityServiceBase, ISettingsService
    {
        public SettingsService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
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
    }
}
