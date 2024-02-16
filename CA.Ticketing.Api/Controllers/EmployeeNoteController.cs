using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.EmployeeNotes;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Authorize(Policy = Policies.CompanyUsers)]
    public class EmployeeNoteController : BaseController
    {
        private readonly INotesService _notesService;

        public EmployeeNoteController(INotesService notesService)
        {
            _notesService = notesService;
        }

        [Route(ApiRoutes.EmployeeNotes.GetAllByEmployeeId)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeNoteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByEmployeeId(string employeeId)
        {
            var notes = await _notesService.GetAllByEmployeeId(employeeId);
            return Ok(notes);
        }

        [Route(ApiRoutes.EmployeeNotes.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(EmployeeNoteDto note)
        {
            var noteId = await _notesService.Create(note);
            return Ok(noteId);
        }


        [Route(ApiRoutes.EmployeeNotes.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(EmployeeNoteDto note)
        {
            await _notesService.Update(note);
            return Ok();
        }


        [Route(ApiRoutes.EmployeeNotes.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string noteId)
        {
            await _notesService.Delete(noteId);
            return Ok();
        }
    }
}
