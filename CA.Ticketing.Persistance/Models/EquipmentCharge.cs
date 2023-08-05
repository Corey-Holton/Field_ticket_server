using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.EquipmentCharges)]
    public class EquipmentCharge : IdentityModel<int>
    {
        public int EquipmentId { get; set; }

        public virtual Equipment Equipment { get; set; }

        public int ChargeId { get; set; }

        public virtual Charge Charge { get; set; }

        public string ItemName { get; set; }

        public double Rate { get; set; }
    }
}
