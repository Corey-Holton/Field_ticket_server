using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.EmployeeNotes
{
    public interface INotesService
    {
        Task<IEnumerable<EmployeeNoteDto>> GetAll();
        Task<EmployeeNoteDto> GetByEmployeeIdInTicket(string ticketId, string employeeId);
        Task<IEnumerable<EmployeeNoteDto>> GetAllByEmployeeId(string employeeId);
        Task<string> Create(EmployeeNoteDto entity);
        Task Update(EmployeeNoteDto entity);
        Task Delete(string noteId);
        Task UpdateById(EmployeeNoteDto entity);
    }
}
