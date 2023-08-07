using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.TicketSpecification)]
    public class TicketSpecification : IdentityModel<int>
    {
        [ForeignKey(nameof(FieldTicket))]
        public int FieldTicketId { get; set; }

        public virtual FieldTicket FieldTicket { get; set; }

        public string Charge { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public int Quantity { get; set; }

        public double Rate { get; set; }

        public double Total { get; set; }
    }
}
