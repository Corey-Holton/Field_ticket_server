using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Charges)]
    public class Charge : IdentityModel
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public double DefaultRate { get; set; }

        public bool IsRigSpecific { get; set; }

        public bool IncludeInTicketSpecs { get; set; }

        public bool AllowUoMChange { get; set; }

        public bool AllowRateAdjustment { get; set; }

        public bool AutoCalculated { get; set; }

        [JsonIgnore]
        public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

        [JsonIgnore]
        public virtual ICollection<TicketType> SpecialChargesTicketTypes { get; set; } = new List<TicketType>();
    }
}
