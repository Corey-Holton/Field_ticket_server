using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Invoices)]
    public class Invoice : IdentityModel
    {
        public string InvoiceId { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? SentToCustomer { get; set; }

        public bool Paid { get; set; }

        [JsonIgnore]
        public virtual ICollection<FieldTicket> Tickets { get; set; } = new List<FieldTicket>();
    }
}
