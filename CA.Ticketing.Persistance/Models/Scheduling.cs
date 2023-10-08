using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Scheduling)]

    public class Scheduling : IdentityModel
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(CustomerLocation))]
        public string? CustomerLocationId { get; set; }

        [JsonIgnore]
        public virtual CustomerLocation? CustomerLocation { get; set; }

        [ForeignKey(nameof(CustomerContact))]
        public string? CustomerContactId { get; set; }

        [JsonIgnore]
        public virtual CustomerContact? CustomerContact { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(Equipment))]
        public string EquipmentId { get; set; }

        [JsonIgnore]
        public virtual Equipment Equipment { get; set; }

    }
}
