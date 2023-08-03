using AutoMapper;
using CA.Ticketing.Business.Services.TicketSpecifications.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class TicketSpecificationsMapper : Profile
    {
        public TicketSpecificationsMapper()
        {
            CreateMap<CreateTicketSpecificationDto, TicketSpecification>();
        }
    }
}
