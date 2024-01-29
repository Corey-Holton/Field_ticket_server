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

        public async Task Delete(string ticketId, string employeeId)
        {
            var note = await GetNote(ticketId, employeeId);

            _removalService.Remove(note);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeNoteDto>> GetAll()
        {
            var notes = await _context.EmployeeNotes
                .ToListAsync();
            return notes.Select(x => _mapper.Map<EmployeeNoteDto>(x));
        }

        public async Task<EmployeeNoteDto> GetByEmployeeIdInTicket(string ticketId, string employeeId)
        {
            var note = await GetNote(ticketId, employeeId);

            return _mapper.Map<EmployeeNoteDto>(note);
        }

        public async Task<IEnumerable<EmployeeNoteDto>> GetAllByEmployeeId(string id)
        {
            var notes = await _context.EmployeeNotes
                .Where(x => x.EmployeeId == id)
                .ToListAsync();

            return notes.Select(x => _mapper.Map<EmployeeNoteDto>(x));
        }

        public async Task Update(EmployeeNoteDto entity)
        {
            var note = await GetNote(entity.TicketId,entity.EmployeeId);

            if(note  == null)
            {
                await Create(entity);
                return;
            }

            _mapper.Map(entity, note);
            await _context.SaveChangesAsync();
        }


        private async Task<EmployeeNote> GetNote(string? ticketId, string? employeeId)
        {
            var note = await _context.EmployeeNotes
                .Where(x => x.TicketId == ticketId)
                .SingleOrDefaultAsync(x => x.EmployeeId == employeeId);

            return note!;
        }
    }
}
