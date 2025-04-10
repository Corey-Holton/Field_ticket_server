﻿using AutoMapper;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class NoteMapper : Profile
    {
        public NoteMapper()
        {
            CreateMap<EmployeeNote, EmployeeNoteDto>()
               .ForMember(x => x.TicketName, dest => dest.MapFrom(src => src.FieldTicket.TicketId));

            CreateMap<EmployeeNoteDto, EmployeeNote>()
             .ForMember(x => x.Id, dest => dest.Ignore());
            
            CreateMap<EmployeeNote, EmployeeNote>();
        }
    }
}
