using AutoMapper;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
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
                .ForMember(x => x.Invoice, dest => dest.MapFrom(src => src.Invoice))
                .ForMember(x => x.LocationName, dest => dest.MapFrom(src => src.Location != null ? src.Location.DisplayName : "None"))
                .ForMember(x => x.CustomerName, dest => dest.MapFrom(src => src.Customer != null ? src.Customer.Name : "None"));

            CreateMap<FieldTicket, TicketDetailsDto>()
                .IncludeBase<FieldTicket, TicketDto>();

            CreateMap<FieldTicket, TicketInfoDto>();

            CreateMap<TicketSpecification, TicketSpecificationDto>()
                .ForMember(x => x.Total, dest => dest.MapFrom(src => src.Quantity * src.Rate))
                .ForMember(x => x.IsReadOnly, dest => dest.MapFrom(src => ChargesInfo.ReadonlyCharges.Contains(src.Charge)));

            CreateMap<TicketSpecificationDto, TicketSpecification>()
                .ForMember(x => x.FieldTicketId, dest => dest.Ignore())
                .ForMember(x => x.AllowRateAdjustment, dest => dest.Ignore())
                .ForMember(x => x.AllowUoMChange, dest => dest.Ignore())
                .ForMember(x => x.Charge, dest => dest.Ignore());

            CreateMap<PayrollData, PayrollDataDto>();

            CreateMap<PayrollDataDto, PayrollData>();

            CreateMap<ManageTicketDto, FieldTicket>()
                .ForMember(x => x.LocationId, dest => dest.MapFrom(src => src.CustomerLocationId));

            CreateMap<ManageTicketHoursDto, FieldTicket>();

            CreateMap<Charge, TicketSpecification>()
                .ForMember(x => x.Id, dest => dest.Ignore())
                .ForMember(x => x.Charge, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Rate, dest => dest.MapFrom(src => src.DefaultRate));

            CreateMap<EquipmentCharge, TicketSpecification>()
                .ForMember(x => x.Id, dest => dest.Ignore())
                .ForMember(x => x.Charge, dest => dest.MapFrom(src => src.Charge.Name))
                .ForMember(x => x.AllowRateAdjustment, dest => dest.MapFrom(src => src.Charge.AllowRateAdjustment))
                .ForMember(x => x.AllowUoMChange, dest => dest.MapFrom(src => src.Charge.AllowUoMChange))
                .ForMember(x => x.UoM, dest => dest.MapFrom(src => src.Charge.UoM));
        }
    }
}
