using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.FieldTickets)]
    public class FieldTicket : IdentityModel, IFileEntity
    {
        public string TicketId { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public ServiceType ServiceType { get; set; }

        public double TaxRate { get; set; }

        [ForeignKey(nameof(Equipment))]
        public string? EquipmentId { get; set; }

        [JsonIgnore]
        public virtual Equipment? Equipment { get; set; }

        [ForeignKey(nameof(Customer))]
        public string? CustomerId { get; set; }

        [JsonIgnore]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(CustomerLocation))]
        public string? LocationId { get; set; }

        [JsonIgnore]
        public virtual CustomerLocation? Location { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        [ForeignKey(nameof(Invoice))]
        public string? InvoiceId { get; set; }

        [JsonIgnore]
        public virtual Invoice? Invoice { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? SignedOn { get; set; }

        public string SignedBy { get; set; } = string.Empty;

        public string EmployeePrintedName { get; set; } = string.Empty;

        public string EmployeeSignature { get; set; } = string.Empty;

        public DateTime? CustomerSignedOn { get; set; }

        public string CustomerPrintedName { get; set; } = string.Empty;

        public string CustomerSignedBy { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<TicketSpecification> TicketSpecifications { get; set; } = new List<TicketSpecification>();

        [JsonIgnore]
        public virtual ICollection<PayrollData> PayrollData { get; set; } = new List<PayrollData>();

        [NotMapped]
        public bool IsInvoiced => InvoiceId != null;

        [NotMapped]
        public double Total => TicketSpecifications.Sum(x => x.Quantity * x.Rate);

        [NotMapped]
        public bool HasCustomerSignature => CustomerSignedOn.HasValue;

        [NotMapped]
        public bool HasEmployeeSignature => SignedOn.HasValue;

        [NotMapped]
        public byte[]? FileBytes { get; set; }
    }
}
