using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Customers)]
    public class Customer : IdentityModel<int>
    {
        public string Name { get; set; }

        public int NetTerm { get; set; }

        public virtual ICollection<CustomerLocation> Locations { get; set; } = new List<CustomerLocation>();
    }
}
