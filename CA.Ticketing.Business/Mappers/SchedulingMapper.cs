using AutoMapper;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class SchedulingMapper : Profile
    {
        public SchedulingMapper()
        {
            CreateMap<SchedulingDto, Scheduling>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<Scheduling, SchedulingDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerLocationName, opt => opt.MapFrom(src => src.CustomerLocation != null ? src.CustomerLocation.DisplayName : string.Empty))
                .ForMember(dest => dest.CustomerContactName, opt => opt.MapFrom(src => src.CustomerContact != null ? src.CustomerContact.DisplayName : string.Empty))
                .ForMember(dest => dest.CustomerContactPhone, opt => opt.MapFrom(src => src.CustomerContact != null ? src.CustomerContact.PhoneNumber : string.Empty))
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name));

            CreateMap<Scheduling, SchedulingDtoExtended>()
                .IncludeBase<Scheduling, SchedulingDto>()
                .ForMember(x => x.CustomerLocation, dest => dest.MapFrom(src => src.CustomerLocation));

            CreateMap<Scheduling, Scheduling>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }

    }
}
