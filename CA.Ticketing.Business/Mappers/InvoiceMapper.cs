using AutoMapper;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDto>();

            CreateMap<CreateInvoiceDto, Invoice>();
        }
    }
}
