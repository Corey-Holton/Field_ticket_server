using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Tickets;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Authorize(Policy = Policies.CompanyUsers)]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetById(string ticketId)
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
        public async Task<IActionResult> Delete(string ticketId)
        {
            await _ticketService.Delete(ticketId);
            return Ok();
        }

        /// <summary>
        /// Get Payroll data
        /// </summary>
        /// <returns>Payroll Details</returns>
        [Route(ApiRoutes.Tickets.GetPayrollData)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<List<PayrollDataDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayrollData(string ticketId)
        {
            var payrollData = await _ticketService.GetPayrollData(ticketId);
            return Ok(payrollData);
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
        /// Update Payroll data
        /// </summary>
        /// <param name="payrollDataDto">PayrollDataDto</param>
        [Route(ApiRoutes.Tickets.UpdatePayrollEntry)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePayroll(PayrollDataDto payrollDataDto)
        {
            await _ticketService.UpdatePayrollData(payrollDataDto);
            return Ok();
        }

        /// <summary>
        /// Remove Payroll data
        /// </summary>
        /// <param name="payrollDataId">PayrollData Id</param>
        [Route(ApiRoutes.Tickets.DeletePayrollEntry)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemovePayroll(string payrollDataId)
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

        /// <summary>
        /// Preview Ticket
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        [Route(ApiRoutes.Tickets.Preview)]
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> PreviewTicket(string ticketId)
        {
            var previewResult = await _ticketService.PreviewTicket(ticketId);
            return Ok(previewResult);
        }

        /// <summary>
        /// Add Employee Signature
        /// </summary>
        /// <param name="signatureBaseDto">SignatureBaseDto</param>
        [Route(ApiRoutes.Tickets.EmployeeSignature)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EmployeeSignature(SignatureBaseDto signatureBaseDto)
        {
            await _ticketService.EmployeeSignature(signatureBaseDto);
            return Ok();
        }

        /// <summary>
        /// Add Customer Signature
        /// </summary>
        /// <param name="customerSignatureDto">CustomerSignatureDto</param>
        [Route(ApiRoutes.Tickets.CustomerSignature)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> CustomerSignature(CustomerSignatureDto customerSignatureDto)
        {
            await _ticketService.CustomerSignature(customerSignatureDto);
            return Ok();
        }

        /// <summary>
        /// Customer Ticket Upload
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="ticketPdf">Ticket Upload</param>
        [Route(ApiRoutes.Tickets.CustomerUpload)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> CustomerUpload(string ticketId, IFormFile ticketPdf)
        {
            await _ticketService.UploadTicket(ticketPdf.OpenReadStream(), ticketId);

            return Ok();
        }

        /// <summary>
        /// Download Ticket
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        [Route(ApiRoutes.Tickets.Download)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadTicket(string ticketId)
        {
            var ticketBytes = await _ticketService.DownloadTicket(ticketId);
            var stream = new MemoryStream(ticketBytes);

            return File(stream, "application/pdf", $"Ticket_{ticketId}.pdf");
        }

        /// <summary>
        /// Reset Ticket Signatures
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        [Route(ApiRoutes.Tickets.Reset)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetSignatures(string ticketId)
        {
            await _ticketService.ResetSignatures(ticketId);
            return Ok();
        }

        /// <summary>
        /// Send to customer
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="redirectUrl">Redirect Url</param>
        [Route(ApiRoutes.Tickets.SendToCustomer)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendToCustomer(string ticketId, string redirectUrl)
        {
            await _ticketService.SendToClient(ticketId, redirectUrl);
            return Ok();
        }
    }
}
