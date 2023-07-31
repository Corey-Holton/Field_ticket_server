using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Invoices)]
    public class Invoice : IdentityModel<int>
    {
        public string InvoiceIdentifier { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool Paid { get; set; }

        public virtual ICollection<FieldTicket> Tickets { get; set; } = new List<FieldTicket>();
    }
}
