using AutoMapper;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;


namespace CA.Ticketing.Business.Mappers
{
    public class EquipmentMapper : Profile
    {
        public EquipmentMapper() 
        {
            CreateMap<Equipment, EquipmentDto>()
                 .ForMember(x => x.Category, dest => dest.MapFrom(src => src.Category.GetCatgeoryName()));

            CreateMap<Equipment, EquipmentDetailsDto>()
                .IncludeBase<Equipment, EquipmentDto>();

            CreateMap<EquipmentDetailsDto, Equipment>();
        }
    }
}
