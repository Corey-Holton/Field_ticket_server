using AutoMapper;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Persistance.Models;


namespace CA.Ticketing.Business.Mappers
{
    public class EquipmentMapper : Profile
    {
        public EquipmentMapper() 
        {
            CreateMap<Equipment, EquipmentDto>();

            CreateMap<Equipment, EquipmentDetailsDto>()
                .IncludeBase<Equipment, EquipmentDto>();

            CreateMap<EquipmentDetailsDto, Equipment>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<EquipmentCharge, EquipmentChargeDto>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Charge.Name))
                .ForMember(x => x.UoM, dest => dest.MapFrom(src => src.Charge.UoM));

            CreateMap<EquipmentChargeDto, EquipmentCharge>()
                .ForMember(x => x.Id, dest => dest.Ignore())
                .ForMember(x => x.EquipmentId, dest => dest.Ignore())
                .ForMember(x => x.ChargeId, dest => dest.Ignore());

            CreateMap<EquipmentFile, EquipmentFileDto>();

            CreateMap<Equipment, Equipment>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<EquipmentCharge, EquipmentCharge>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<EquipmentFile, EquipmentFile>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
