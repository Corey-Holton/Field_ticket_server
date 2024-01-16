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
        Task<EmployeeNoteDataDto> GetById(string id);
        Task<string> Create(EmployeeNoteDataDto entity);
        Task Update(EmployeeNoteDataDto entity);
        Task Delete(string id);
    }
}
