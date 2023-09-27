using AutoMapper;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
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
                .ForMember(x => x.Total, dest => dest.MapFrom((src, dest) => 
                {
                    var ticketsTotal = src.Tickets.Sum(x => x.Total);
                    if (!src.InvoiceLateFees.Any())
                    {
                        return ticketsTotal;
                    }

                    foreach (var invoiceLateFee in src.InvoiceLateFees)
                    {
                        ticketsTotal += ticketsTotal * BusinessConstants.InvoiceLateFee / 100;
                    }

                    return ticketsTotal;
                }))
                .ForMember(x => x.Tickets, dest => dest.MapFrom(src => src.Tickets.OrderBy(x => x.CreatedDate)))
                .ForMember(x => x.LateFees, dest => dest.MapFrom(src => src.InvoiceLateFees.OrderBy(x => x.CreatedDate)));

            CreateMap<InvoiceLateFee, InvoiceLateFeeDto>()
                .ForMember(x => x.AppliedOn, dest => dest.MapFrom(src => src.CreatedDate));

            CreateMap<Invoice, InvoiceIdentifierDto>();

            CreateMap<Invoice, Invoice>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
