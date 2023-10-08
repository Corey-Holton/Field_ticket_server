using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.CustomerLocations)]
    public class CustomerLocation : IdentityModel
    {
        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        public string Lease { get; set; }

        public string Well { get; set; }

        public string Field { get; set; }

        public string County { get; set; }

        public double? Lattitude { get; set; }

        public double? Longitude { get; set; }

        public WellType WellType { get; set; }

        [JsonIgnore]
        public virtual ICollection<FieldTicket> FieldTickets { get; set; } = new List<FieldTicket>();

        [JsonIgnore]
        public virtual ICollection<Scheduling> ScheduledJobs { get; set; } = new List<Scheduling>();

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