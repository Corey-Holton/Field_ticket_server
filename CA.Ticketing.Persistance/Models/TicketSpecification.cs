using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.TicketSpecification)]
    public class TicketSpecification : IdentityModel<int>
    {
        public int Quantity { get; set; }

        [ForeignKey(nameof(EquipmentCharge))]
        public int EquipmentChargeId { get; set; }

        public virtual EquipmentCharge EquipmentCharge { get; set; }

        [ForeignKey(nameof(FieldTicket))]
        public int FieldTicketId { get; set; }

        public virtual FieldTicket FieldTicket { get; set; }
    }
}
