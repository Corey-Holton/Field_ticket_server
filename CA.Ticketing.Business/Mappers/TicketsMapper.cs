using AutoMapper;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class TicketsMapper : Profile
    {
        public TicketsMapper() 
        {
            CreateMap<FieldTicket, TicketDto>()
                .ForMember(x => x.ServiceType, dest => dest.MapFrom(src => src.ServiceType.GetServiceType()))
                .ForMember(x => x.CustomerName, dest => dest.MapFrom(src => src.Customer.Name));

            CreateMap<TicketDetailsDto, FieldTicket>();
        }
    }
}
