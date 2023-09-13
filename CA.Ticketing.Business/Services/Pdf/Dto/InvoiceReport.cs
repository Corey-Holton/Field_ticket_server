using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Pdf.Dto
{
    public class InvoiceReport
    {
        private readonly Invoice _invoice;

        public InvoiceReport(Invoice invoice)
        {
            _invoice = invoice;
        }

        public string CustomerName => _invoice.Customer.Name;

        public string CustomerAddress => _invoice.Customer.Address;


    }
}
