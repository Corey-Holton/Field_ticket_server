using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.EmployeeNotes
{
    public class NotesService : EntityServiceBase, INotesService
    {
        public NotesService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<string> Create(EmployeeNoteDataDto entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<EmployeeNoteDataDto> GetById(string EmployeeId)
        {
            var note =  await _context.EmployeeNotes.FirstOrDefaultAsync(x => x.EmployeeId == EmployeeId);
            return _mapper.Map<EmployeeNoteDataDto>(note); 
        }

        public Task Update(EmployeeNoteDataDto entity)
        {
            throw new NotImplementedException();
        }
    }
}
