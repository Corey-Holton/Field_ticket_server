using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Customers)]
    public class Customer : IdentityModelWithAddress<int>
    {
        public string Name { get; set; }

        public int NetTerm { get; set; }

        public virtual ICollection<CustomerLocation> Locations { get; set; } = new List<CustomerLocation>();

        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public virtual ICollection<FieldTicket> Tickets { get; set; } = new List<FieldTicket>();
    }
}
