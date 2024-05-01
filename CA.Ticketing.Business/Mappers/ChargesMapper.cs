using AutoMapper;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class ChargesMapper : Profile
    {
        public ChargesMapper()
        {
            CreateMap<Charge, ChargeDto>();

            CreateMap<ChargeDto, Charge>()
                .ForMember(x => x.Order, dest => dest.Ignore())
                .ForMember(x => x.Name, dest => dest.Ignore())
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<Charge, Charge>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
