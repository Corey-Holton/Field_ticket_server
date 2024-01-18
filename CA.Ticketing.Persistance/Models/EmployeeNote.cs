using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.EmployeeNotes)]
    public class EmployeeNote : IdentityModel
    {

        [ForeignKey(nameof(Employee))]
        public string EmployeeId { get; set; }

        [ForeignKey(nameof(FieldTicket))]
        public string? TicketId { get; set; }

        public string NoteContent { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set;}
    }
}
