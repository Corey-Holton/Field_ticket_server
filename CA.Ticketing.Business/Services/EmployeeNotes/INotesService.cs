using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
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
        Task<EmployeeNoteDto> GetByEmployeeId(string id);
        Task<string> Create(EmployeeNoteDto entity);
        Task Update(EmployeeNoteDto entity);
        Task Delete(string id);
    }
}
