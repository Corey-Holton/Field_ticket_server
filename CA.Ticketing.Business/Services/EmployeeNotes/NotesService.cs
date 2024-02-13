using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Migrations;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using static CA.Ticketing.Common.Constants.ApiRoutes;


namespace CA.Ticketing.Business.Services.EmployeeNotes
{
    public class NotesService : EntityServiceBase, INotesService
    {
        private readonly IRemovalService _removalService;

 
        public NotesService(
            CATicketingContext context, 
            IRemovalService removalService, 
            IMapper mapper) : base(context, mapper)
        {
            _removalService = removalService;
      
        }

        public async Task<string> Create(EmployeeNoteDto entity)
        {
            var employee = await _context.Employees
                .SingleAsync(x => x.Id == entity.EmployeeId);

            if(entity.TicketId != null)
            {
                var noteExist = await _context.EmployeeNotes
              .SingleOrDefaultAsync(x => x.EmployeeId == entity.EmployeeId && x.TicketId == entity.TicketId);

                if (noteExist != null)
                {
                    throw new Exception("Note Already Exist");
                }
            }
            
            var note = new EmployeeNote()
            {
                EmployeeId = employee.Id,
                TicketId = entity.TicketId,
                NoteContent = entity.NoteContent,
                CreatedBy = employee.Id,
                UpdatedBy = employee.Id
            };

            _context.EmployeeNotes.Add(note);
            await _context.SaveChangesAsync();
            return note.Id;
        }

        public async Task Delete(string noteId)
        {
            var note = await _context.EmployeeNotes
                .SingleOrDefaultAsync(x => x.Id == noteId);

            if (note == null)
            {
                throw new Exception("Note Does Not Exist");
            }

            _removalService.Remove(note);

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<EmployeeNoteDto>> GetAllByEmployeeId(string id)
        {
            var notes = await _context.EmployeeNotes
                .Include(x => x.FieldTicket) 
                .Where(x => x.EmployeeId == id)
                .ToListAsync();

            return notes.Select(x => _mapper.Map<EmployeeNoteDto>(x));
        }

        public async Task Update(EmployeeNoteDto entity)
        {
            var note = await _context.EmployeeNotes
               .SingleOrDefaultAsync(x => x.Id == entity.Id);

            if (note == null)
            {
                throw new Exception("Note Does Not Exist");
            }
            entity.Id = null;
            _mapper.Map(entity, note);
            await _context.SaveChangesAsync();
        }


        private async Task<EmployeeNote> GetNote(string? ticketId, string? employeeId)
        {
            var note = await _context.EmployeeNotes
                .Include(x => x.FieldTicket)
                .SingleOrDefaultAsync(x => x.EmployeeId == employeeId && x.TicketId == ticketId);

            if (note == null)
            {
                throw new Exception("Note Does Not Exist");
            }

            return note!;
        }
    }
}
