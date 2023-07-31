using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.FieldTickets)]
    public class FieldTicket : IdentityModel<int>
    {
        public string TicketIdentifier { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Description { get; set; }

        public ServiceType ServiceType { get; set; }

        [ForeignKey(nameof(Equipment))]
        public int EquipmentId { get; set; }

        public virtual Equipment Equipment { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Mileage { get; set; }

        [ForeignKey(nameof(CustomerLocation))]
        public int LocationId { get; set; }

        public virtual CustomerLocation Location { get; set; }

        public bool Signature { get; set; }

        [ForeignKey(nameof(Invoice))]
        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
    }
}
