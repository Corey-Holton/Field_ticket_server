using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Customers)]
    public class Customer : IdentityModelWithAddress
    {
        public string Name { get; set; }

        public int NetTerm { get; set; }

        public string SendInvoiceTo { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<CustomerLocation> Locations { get; set; } = new List<CustomerLocation>();

        [JsonIgnore]
        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();

        [JsonIgnore]
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        [JsonIgnore]
        public virtual ICollection<FieldTicket> Tickets { get; set; } = new List<FieldTicket>();

        [JsonIgnore]
        public virtual ICollection<Scheduling> ScheduledJobs { get; set; } = new List<Scheduling>();
    }
}
