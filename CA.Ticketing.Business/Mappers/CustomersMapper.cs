using AutoMapper;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class CustomersMapper : Profile
    {
        public CustomersMapper()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<Customer, CustomerDetailsDto>()
                .IncludeBase<Customer, CustomerDto>();

            CreateMap<CustomerDetailsDto, Customer>()
                .ForMember(x => x.Locations, dest => dest.Ignore())
                .ForMember(x => x.Contacts, dest => dest.Ignore());

            CreateMap<CustomerLocation, CustomerLocationDto>();

            CreateMap<CustomerLocationDto, CustomerLocation>();

            CreateMap<CustomerContactDto, CustomerContact>()
                .ForMember(x => x.InviteSent, dest => dest.Ignore())
                .ForMember(x => x.InviteSentOn, dest => dest.Ignore());

            CreateMap<CustomerContact, CustomerContactDto>()
                .ForMember(x => x.HasLogin, dest => dest.MapFrom(src => src.ApplicationUser != null));

            CreateMap<CustomerContact, CreateCustomerContactLoginDto>()
                .ForMember(x => x.CustomerContactId, dest => dest.MapFrom(src => src.Id));
        }
    }
}
