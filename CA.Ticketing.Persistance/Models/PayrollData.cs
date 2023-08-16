using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.PayrollData)]
    public class PayrollData : IdentityModel<int>
    {
        [ForeignKey(nameof(FieldTicket))]
        public int FieldTicketId { get; set; }

        public virtual FieldTicket FieldTicket { get; set; }

        [ForeignKey(nameof(Employee))]
        public int? EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        public string Name { get; set; }
    }
}
