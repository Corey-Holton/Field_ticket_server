using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.TicketSpecification)]
    public class TicketSpecification : IdentityModel
    {
        [ForeignKey(nameof(FieldTicket))]
        public string FieldTicketId { get; set; }

        public virtual FieldTicket FieldTicket { get; set; }

        public string Charge { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public double Quantity { get; set; }

        public double Rate { get; set; }

        public bool AllowUoMChange { get; set; }

        public bool AllowRateAdjustment { get; set; }
    }
}
