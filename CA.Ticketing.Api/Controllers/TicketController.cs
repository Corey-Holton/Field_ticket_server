using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Customers.Dto;
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
        /// Get a list of tickets corresponding to location name
        /// </summary>
        /// <returns>List of tickets</returns>
        [Route(ApiRoutes.Tickets.ListByLocation)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLocation(string search)
        {
            var tickets = await _ticketService.GetByLocation(search);
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
        /// <param name="ticket">TicketDetailsDto</param>
        /// <returns>Ticket Id</returns>
        [Route(ApiRoutes.Tickets.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(TicketDetailsDto ticket)
        {
            var ticketId = await _ticketService.Create(ticket);
            return Ok(ticketId);
        }

        /// <summary>
        /// Update a ticket
        /// </summary>
        /// <param name="ticket">TicketDetailsDto</param>
        [Route(ApiRoutes.Tickets.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(TicketDetailsDto ticket)
        {
            await _ticketService.Update(ticket);
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
    }
}
