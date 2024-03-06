using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.EmployeeNotes)]
    public class EmployeeNote : IdentityModel
    {

        [ForeignKey(nameof(Employee))]
        public string EmployeeId { get; set; }

        [JsonIgnore]
        public virtual Employee? Employee { get; set; }

        [ForeignKey(nameof(FieldTicket))]
        public string? TicketId { get; set; }
        
        [JsonIgnore]
        public virtual FieldTicket? FieldTicket { get; set; }

        public string NoteContent { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set;}
    }
}
