using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.CustomerLocations)]
    public class CustomerLocation : IdentityModel
    {
        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public string Lease { get; set; }

        public string Well { get; set; }

        public string Field { get; set; }

        public string County { get; set; }

        public double? Lattitude { get; set; }

        public double? Longitude { get; set; }

        public WellType WellType { get; set; }

        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();

        public virtual ICollection<FieldTicket> FieldTickets { get; set; } = new List<FieldTicket>();

        [NotMapped]
        public string DisplayName 
        {
            get
            {
                return $"{Lease} {Well}";
            } 
        }
    }
}