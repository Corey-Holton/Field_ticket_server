using AutoMapper;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class SchedulingMapper : Profile
    {
        public SchedulingMapper()
        {

            CreateMap<SchedulingDto, Scheduling>();  

            CreateMap<Scheduling, SchedulingDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.Equipment.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name));


        }

    }
}
