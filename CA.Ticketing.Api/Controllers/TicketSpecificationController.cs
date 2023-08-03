using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.TicketSpecifications;
using CA.Ticketing.Business.Services.TicketSpecifications.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class TicketSpecificationController : BaseController
    {
        private readonly ITicketSpecificationService _ticketSpecificationService;

        public TicketSpecificationController(ITicketSpecificationService ticketSpecificationService)
        {
            _ticketSpecificationService = ticketSpecificationService;
        }

        /// <summary>
        /// Create a Ticket Specification
        /// </summary>
        /// <param name="ticketSpecification">Ticket Specification DTO</param>
        /// <returns>Ticket Specification Id</returns>
        [Route(ApiRoutes.TicketSpecifications.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateTicketSpecificationDto ticketSpecification)
        {
            var ticketSpecificationId = await _ticketSpecificationService.Create(ticketSpecification);
            return Ok(ticketSpecificationId);
        }

        /// <summary>
        /// Update an existing Ticket Specification
        /// </summary>
        /// <param name="ticketSpecification">Ticket Specification DTO</param>
        [Route(ApiRoutes.TicketSpecifications.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(CreateTicketSpecificationDto ticketSpecification)
        {
            await _ticketSpecificationService.Update(ticketSpecification);
            return Ok();
        }

        /// <summary>
        /// Delete an existing Ticket Specification
        /// </summary>
        /// <param name="ticketSpecificationId">Ticket Specification Id</param>
        [Route(ApiRoutes.TicketSpecifications.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int ticketSpecificationId)
        {
            await _ticketSpecificationService.Delete(ticketSpecificationId);
            return Ok();
        }
    }
}
