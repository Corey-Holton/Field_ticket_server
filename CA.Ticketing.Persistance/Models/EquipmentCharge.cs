using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.EquipmentCharges)]
    public class EquipmentCharge : IdentityModel
    {
        [ForeignKey(nameof(Equipment))]
        public string EquipmentId { get; set; }

        [JsonIgnore]
        public virtual Equipment Equipment { get; set; }

        [ForeignKey(nameof(Charge))]
        public string ChargeId { get; set; }

        [JsonIgnore]
        public virtual Charge Charge { get; set; }

        public double Rate { get; set; }
    }
}
