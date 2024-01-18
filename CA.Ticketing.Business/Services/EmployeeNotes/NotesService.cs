using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;


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

            var note = new EmployeeNote()
            {
                EmployeeId = employee.Id,
                TicketId = entity.TicketId,
                NoteContent = entity.NoteContent,
                CreatedBy = employee.DisplayName,
                UpdatedBy = employee.DisplayName
            };

            _context.EmployeeNotes.Add(note);
            await _context.SaveChangesAsync();
            return note.Id;
        }

        public async Task Delete(string id)
        {
            var note = await _context.EmployeeNotes.SingleAsync(x => x.Id == id);

            _removalService.Remove(note);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeNoteDto>> GetAll()
        {
            var notes = await _context.EmployeeNotes
                .ToListAsync();
            return notes.Select(x => _mapper.Map<EmployeeNoteDto>(x));
        }

        public async Task<EmployeeNoteDto> GetByEmployeeId(string id)
        {
            var note = await _context.EmployeeNotes
                .SingleOrDefaultAsync(x => x.EmployeeId == id);

            return _mapper.Map<EmployeeNoteDto>(note);

        }

        public async Task Update(EmployeeNoteDto entity)
        {
            var note = await GetNote(entity.Id);
            _mapper.Map(entity, note);
            await _context.SaveChangesAsync();
        }

        private async Task<EmployeeNote> GetNote(string? Id)
        {
            var note = await _context.EmployeeNotes
                .SingleOrDefaultAsync(x => x.Id == Id);

            if (note == null)
            {
                throw new KeyNotFoundException(nameof(EmployeeNote));
            }

            return note!;
        }
    }
}
