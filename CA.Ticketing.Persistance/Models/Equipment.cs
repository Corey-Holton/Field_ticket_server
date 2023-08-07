using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Equipment)]
    public class Equipment : IdentityModel<int>
    {
        public EquipmentCategory Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Year { get; set; }

        public string VinNumber { get; set; }

        public string PermitNumber { get; set; }

        public DateTime? PermitExpirationDate { get; set; }

        public DateTime? LastMaintenance { get; set; }

        public double FuelConsumption { get; set; }

        public virtual ICollection<EquipmentCharge> Charges { get; set; } = new List<EquipmentCharge>();
    }
}
