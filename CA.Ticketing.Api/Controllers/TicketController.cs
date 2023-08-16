using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Tickets;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Get a list of all tickets
        /// </summary>
        /// <returns>List of tickets</returns>
        [Route(ApiRoutes.Tickets.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAll();
            return Ok(tickets);
        }

        /// <summary>
        /// Get a list of tickets between start and end date
        /// </summary>
        /// <returns>List of tickets</returns>
        [Route(ApiRoutes.Tickets.ListByDates)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await _ticketService.GetByDates(startDate, endDate);
            return Ok(tickets);
        }

        /// <summary> 
        /// Get ticket details
        /// </summary>
        /// <returns>Ticket Details</returns>
        [Route(ApiRoutes.Tickets.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(TicketDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int ticketId)
        {
            var ticket = await _ticketService.GetById(ticketId);
            return Ok(ticket);
        }

        /// <summary>
        /// Create a ticket
        /// </summary>
        /// <returns>Ticket Id</returns>
        /// <param name="manageTicketDto">Manage Ticket Dto</param>
        [Route(ApiRoutes.Tickets.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(ManageTicketDto manageTicketDto)
        {
            var ticketId = await _ticketService.Create(manageTicketDto);
            return Ok(ticketId);
        }

        /// <summary>
        /// Update ticket details
        /// </summary>
        /// <param name="ticket">ManageTicketDto</param>
        [Route(ApiRoutes.Tickets.UpdateDetails)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(ManageTicketDto ticket)
        {
            await _ticketService.Update(ticket);
            return Ok();
        }

        /// <summary>
        /// Update ticket hours
        /// </summary>
        /// <param name="ticket">ManageTicketHoursDto</param>
        [Route(ApiRoutes.Tickets.UpdateHours)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateHours(ManageTicketHoursDto ticket)
        {
            await _ticketService.UpdateHours(ticket);
            return Ok();
        }

        /// <summary>
        /// Delete an existing Ticket
        /// </summary>
        [Route(ApiRoutes.Tickets.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int ticketId)
        {
            await _ticketService.Delete(ticketId);
            return Ok();
        }

        /// <summary>
        /// Add Payroll data
        /// </summary>
        /// <param name="payrollDataDto">PayrollDataDto</param>
        [Route(ApiRoutes.Tickets.AddPayrollEntry)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddPayroll(PayrollDataDto payrollDataDto)
        {
            await _ticketService.AddPayroll(payrollDataDto);
            return Ok();
        }

        /// <summary>
        /// Remove Payroll data
        /// </summary>
        /// <param name="payrollDataId">PayrollData Id</param>
        [Route(ApiRoutes.Tickets.DeletePayrollEntry)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemovePayroll(int payrollDataId)
        {
            await _ticketService.RemovePayroll(payrollDataId);
            return Ok();
        }

        /// <summary>
        /// Update Ticket Specification
        /// </summary>
        /// <param name="ticketSpecificationDto">TicketSpecificationDto</param>
        [Route(ApiRoutes.Tickets.UpdateSpecifications)]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateTicketSpecResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSpecs(TicketSpecificationDto ticketSpecificationDto)
        {
            var result = await _ticketService.UpdateTicketSpecification(ticketSpecificationDto);
            return Ok(result);
        }
    }
}
