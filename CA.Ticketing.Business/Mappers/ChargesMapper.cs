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

            CreateMap<ChargeDto, Charge>();

            CreateMap<Charge, Charge>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
