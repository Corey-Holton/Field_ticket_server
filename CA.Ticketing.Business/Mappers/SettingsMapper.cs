using AutoMapper;
using CA.Ticketing.Business.Services.Settings.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class SettingsMapper : Profile
    {
        public SettingsMapper()
        {
            CreateMap<Setting, SettingDto>();

            CreateMap<SettingDto, Setting>();

            CreateMap<ApplicationUser, ProfileDto>();

            CreateMap<ProfileDto, ApplicationUser>();

            CreateMap<Setting, Setting>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
