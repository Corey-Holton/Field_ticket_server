using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.EmployeeNotes.Dto
{
    public class EmployeeNoteDto : EntityDtoBase
    {
        public string EmployeeId {  get; set; }
        public string? TicketId { get; set; }

        public string? TicketName { get; set; }
        public string NoteContent { get; set; }
    }
}
