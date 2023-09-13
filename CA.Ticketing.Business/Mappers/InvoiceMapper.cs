using AutoMapper;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(x => x.Customer, dest => dest.MapFrom(src => src.Customer.Name))
                .ForMember(x => x.SentToCustomer, dest => dest.MapFrom(src => src.SentToCustomer.HasValue))
                .ForMember(x => x.Total, dest => dest.MapFrom(src => src.Tickets.Sum(x => x.Total)))
                .ForMember(x => x.Tickets, dest => dest.MapFrom(src => src.Tickets));

            CreateMap<Invoice, InvoiceIdentifierDto>();
        }
    }
}
