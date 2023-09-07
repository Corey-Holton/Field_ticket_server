using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Invoices)]
    public class Invoice : IdentityModel<int>
    {
        public string InvoiceIdentifier { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? SentToCustomer { get; set; }

        public bool Paid { get; set; }

        public virtual ICollection<FieldTicket> Tickets { get; set; } = new List<FieldTicket>();
    }
}
