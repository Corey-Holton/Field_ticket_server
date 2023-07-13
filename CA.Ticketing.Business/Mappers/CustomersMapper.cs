using AutoMapper;
using CA.Ticketing.Business.Services.Authentication.Dto;
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

            CreateMap<CustomerLocationDto, CustomerLocation>();

            CreateMap<CustomerContactDto, CustomerContact>();

            CreateMap<CustomerContact, CustomerContactDto>();

            CreateMap<Customer, CustomerDetailsDto>()
                .IncludeBase<Customer, CustomerDto>();

            CreateMap<CustomerDto, Customer>()
                .ForMember(x => x.Locations, dest => dest.Ignore());

            CreateMap<CustomerContact, CreateCustomerContactLoginDto>()
                .ForMember(x => x.CustomerContactId, dest => dest.MapFrom(src => src.Id));
        }
    }
}
