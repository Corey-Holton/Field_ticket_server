using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class CreateInvoiceDto : InvoiceDto
    {
        public int[]? ticketIds { get; set; } 
    }
}
