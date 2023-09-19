using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.PayrollData)]
    public class PayrollData : IdentityModel
    {
        [ForeignKey(nameof(FieldTicket))]
        public string FieldTicketId { get; set; }

        public virtual FieldTicket FieldTicket { get; set; }

        [ForeignKey(nameof(Employee))]
        public string? EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        public string Name { get; set; }

        public double YardHours { get; set; }

        public double RoustaboutHours { get; set; }
    }
}
