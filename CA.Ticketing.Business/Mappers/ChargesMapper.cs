using AutoMapper;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class ChargesMapper : Profile
    {
        public ChargesMapper()
        {
            CreateMap<Charge, ChargeDto>()
                .ForMember(x => x.ChargeType, dest => dest.MapFrom(src => src.ChargeType.GetChargeType()));

            CreateMap<ChargeDto, Charge>();
        }
    }
}
