using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class InvoiceDetailsDto : InvoiceDto
    {
        public List<TicketDto> Tickets { get; set; }
    }
}
