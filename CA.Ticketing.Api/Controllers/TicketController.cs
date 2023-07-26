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
    }
}
