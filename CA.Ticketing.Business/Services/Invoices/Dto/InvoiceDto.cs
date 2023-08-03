using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class InvoiceDto : EntityDtoBase<int>
    {
        public string InvoiceIdentifier { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool Paid { get; set; }
    }
}
