using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.FieldTickets)]
    public class FieldTicket : IdentityModel<int>
    {
        public string TicketId { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Description { get; set; }

        public ServiceType ServiceType { get; set; }

        public WellType WellType { get; set; }

        public double TaxRate { get; set; }

        [ForeignKey(nameof(Equipment))]
        public int? EquipmentId { get; set; }

        public virtual Equipment? Equipment { get; set; }

        [ForeignKey(nameof(Customer))]
        public int? CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(CustomerLocation))]
        public int? LocationId { get; set; }

        public virtual CustomerLocation? Location { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        public bool HasCustomerSignature { get; set; }

        [ForeignKey(nameof(Invoice))]
        public int? InvoiceId { get; set; }

        public virtual Invoice? Invoice { get; set; }

        public string FileIndicatorGenerated { get; set; }

        public string FileIndicatorCustomer { get; set; }

        public virtual ICollection<TicketSpecification> TicketSpecifications { get; set; } = new List<TicketSpecification>();

        public virtual ICollection<PayrollData> PayrollData { get; set; } = new List<PayrollData>();

        [NotMapped]
        public bool IsInvoiced => InvoiceId != null;

        [NotMapped]
        public double Total => TicketSpecifications.Sum(x => x.Total);
    }
}
