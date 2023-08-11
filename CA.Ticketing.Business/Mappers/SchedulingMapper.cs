using AutoMapper;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class SchedulingMapper : Profile
    {
        public SchedulingMapper()
        {
            CreateMap<SchedulingDto, Scheduling>();

            CreateMap<Scheduling, SchedulingDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerLocationName, opt => opt.MapFrom(src => src.CustomerLocation != null ? src.CustomerLocation.DisplayName : string.Empty))
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name));
        }

    }
}
