using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.CustomerLocations)]
    public class CustomerLocation : IdentityModelWithAddress<int>
    {
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public LocationType LocationType { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
    }
}