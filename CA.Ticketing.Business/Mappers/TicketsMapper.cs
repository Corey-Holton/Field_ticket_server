﻿using AutoMapper;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Linq;

namespace CA.Ticketing.Business.Mappers
{
    public class TicketsMapper : Profile
    {
        public TicketsMapper() 
        {
            CreateMap<FieldTicket, TicketDto>()
                .ForMember(x => x.Invoice, dest => dest.MapFrom(src => src.Invoice))
                .ForMember(x => x.LocationName, dest => dest.MapFrom(src => src.Location != null ? src.Location.DisplayName : "None"))
                .ForMember(x => x.ServiceType, dest => dest.MapFrom(src => src.ServiceTypes.First()))
                .ForMember(x => x.CustomerName, dest => dest.MapFrom(src => src.Customer != null ? src.Customer.Name : "None"))
                .ForMember(x => x.TicketType, dest => dest.MapFrom(src => src.TicketType != null ? src.TicketType.Name : "Base"));

            CreateMap<FieldTicket, TicketDetailsDto>()
                .IncludeBase<FieldTicket, TicketDto>()
                .AfterMap((fieldTicket, ticketDto) => 
                { 
                    ticketDto.PayrollData = ticketDto.PayrollData.OrderBy(x => x.JobTitle).ToList();
                });

            CreateMap<FieldTicket, TicketTypeDetailsDto>()
                .IncludeBase<FieldTicket, TicketDto>()
                .AfterMap((fieldTicket, ticketDto) =>
                {
                    ticketDto.PayrollData = ticketDto.PayrollData.OrderBy(x => x.JobTitle).ToList();
                    ticketDto.WellRecord = ticketDto.WellRecord.OrderBy(x => x.WellRecordType).ToList();
                });

            CreateMap<FieldTicket, TicketInfoDto>();

            CreateMap<(bool isAdmin, TicketSpecification ticketSpec), TicketSpecificationDto>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.ticketSpec.Id))
                .ForMember(x => x.Charge, dest => dest.MapFrom(src => src.ticketSpec.Charge))
                .ForMember(x => x.UoM, dest => dest.MapFrom(src => src.ticketSpec.UoM))
                .ForMember(x => x.Quantity, dest => dest.MapFrom(src => src.ticketSpec.Quantity))
                .ForMember(x => x.Rate, dest => dest.MapFrom(src => src.ticketSpec.Rate))
                .ForMember(x => x.AllowRateAdjustment, dest => dest.MapFrom(src => src.ticketSpec.AllowRateAdjustment))
                .ForMember(x => x.AllowUoMChange, dest => dest.MapFrom(src => src.ticketSpec.AllowUoMChange))
                .ForMember(x => x.Total, dest => dest.MapFrom(src => src.ticketSpec.Quantity * src.ticketSpec.Rate))
                .ForMember(x => x.IsReadOnly, dest => dest.MapFrom(src => !src.isAdmin && ChargesInfo.ReadonlyCharges.Contains(src.ticketSpec.Charge)));

            CreateMap<TicketSpecificationDto, TicketSpecification>()
                .ForMember(x => x.FieldTicketId, dest => dest.Ignore())
                .ForMember(x => x.AllowRateAdjustment, dest => dest.Ignore())
                .ForMember(x => x.AllowUoMChange, dest => dest.Ignore())
                .ForMember(x => x.Charge, dest => dest.Ignore());

            CreateMap<PayrollData, PayrollDataDto>()
                .ForMember(x => x.JobTitle, dest => dest.MapFrom(src => src.Employee != null ? src.Employee.JobTitle : Common.Enums.JobTitle.Other))
                .ForMember(x => x.DisplayEmployeeId, dest => dest.MapFrom(src => src.Employee != null ? src.Employee.EmployeeNumber : "0000"));

            CreateMap<PayrollDataDto, PayrollData>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<WellRecord, WellRecordDto>();

            CreateMap<WellRecordDto, WellRecord>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<SwabCups, SwabCupsDto>();

            CreateMap<SwabCupsDto, SwabCups>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<ManageTicketDto, FieldTicket>()
                .ForMember(x => x.Id, dest => dest.Ignore())
                .ForMember(x => x.SendEmailTo, dest => dest.MapFrom(src => !string.IsNullOrEmpty(src.SendEmailTo) ? src.SendEmailTo : string.Empty))
                .ForMember(x => x.CustomerId, dest => dest.MapFrom(src => !string.IsNullOrEmpty(src.CustomerId) ? src.CustomerId : null))
                .ForMember(x => x.LocationId, dest => dest.MapFrom(src => !string.IsNullOrEmpty(src.CustomerLocationId) ? src.CustomerLocationId : null));

            CreateMap<ManageWellOtherDetailsDto, FieldTicket>()
                .ForMember(x => x.Id, dest => dest.Ignore())
                .ForMember(x => x.OtherText, dest => dest.MapFrom(src => !string.IsNullOrEmpty(src.OtherText) ? src.OtherText : string.Empty));

            CreateMap<ManageTicketHoursDto, FieldTicket>()
                .ForMember(x => x.StartTime, dest => dest.MapFrom((src, dest) => 
                {  
                    return new DateTime(dest.ExecutionDate.Year, dest.ExecutionDate.Month, dest.ExecutionDate.Day, src.StartTime.Hour, src.StartTime.Minute, 0);
                }))
                .ForMember(x => x.EndTime, dest => dest.MapFrom((src, dest) =>
                {
                    return new DateTime(dest.ExecutionDate.Year, dest.ExecutionDate.Month, dest.ExecutionDate.Day, src.EndTime.Hour, src.EndTime.Minute, 0);
                }))
                .ForMember(x => x.TicketId, dest => dest.Ignore())
                .ForMember(x => x.Id, dest => dest.Ignore());

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

            CreateMap<FieldTicket, FieldTicket>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<TicketSpecification, TicketSpecification>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<PayrollData, PayrollData>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<WellRecord, WellRecord>()
                .ForMember(x => x.Id, dest => dest.Ignore());

            CreateMap<SwabCups, SwabCups>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
