using AutoMapper;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class CustomersMapper : Profile
    {
        public CustomersMapper()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<CustomerLocation, CustomerLocationDto>()
                .ForMember(x => x.LocationType, dest => dest.MapFrom(src => src.LocationType.GetLocationType()));

            CreateMap<CustomerContact, CustomerContactDto>();

            CreateMap<Customer, CustomerDetailsDto>()
                .IncludeBase<Customer, CustomerDto>(); 
        }
    }
}
