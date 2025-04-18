﻿using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Tickets;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

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
        [ProducesResponseType(typeof(ListResult<TicketDto>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAll(int index, int size, string sorting, string order, string searchString )
        {
            var returnObj = await _ticketService.GetAll(index, size, sorting, order, searchString);
            return Ok(returnObj);
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
        /// Update well other text
        /// </summary>
        /// <param name="details">ManageWellOtherDetailsDto</param>
        [Route(ApiRoutes.Tickets.UpdateOther)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOtherDetails(ManageWellOtherDetailsDto details)
        {
            await _ticketService.UpdateOtherDetails(details);
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
        /// Get Special Ticket Specification data
        /// </summary>
        /// <returns>Special Ticket Specification Details</returns>
        [Route(ApiRoutes.Tickets.GetSpecialTicketSpec)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<List<TicketSpecificationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecialTicketSpec(string ticketId)
        {
            var ticketSpecData = await _ticketService.GetSpecialTicketSpec(ticketId);
            return Ok(ticketSpecData);
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
        /// Update Well Record Specification
        /// </summary>
        /// <param name="wellRecordDto">WellRecordDto</param>
        [Route(ApiRoutes.Tickets.UpdateWellRecord)]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateTicketSpecResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWellRecord(WellRecordDto wellRecordDto)
        {
            await _ticketService.UpdateWellRecord(wellRecordDto);
            return Ok();
        }

        /// <summary>
        /// Create Well Record
        /// </summary>
        /// <param name="wellRecordDto">WellRecordDto</param>
        [Route(ApiRoutes.Tickets.AddWellRecord)]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateTicketSpecResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWellRecord(WellRecordDto wellRecordDto)
        {
            await _ticketService.AddWellRecord(wellRecordDto);
            return Ok();
        }

        /// <summary>
        /// Remove Well Record
        /// </summary>
        /// <param name="wellRecordDto">WellRecord Id</param>
        [Route(ApiRoutes.Tickets.DeleteWellRecord)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveWellRecord(WellRecordDto wellRecordDto)
        {
            await _ticketService.RemoveWellRecord(wellRecordDto);
            return Ok();
        }

        /// <summary>
        /// Update Swab Charge
        /// </summary>
        /// <param name="swabCupDto">swabCupDto</param>
        [Route(ApiRoutes.Tickets.UpdateSwabCharge)]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateTicketSpecResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSwabCharge(SwabCupsDto swabCupDto)
        {
            await _ticketService.UpdateSwabCharge(swabCupDto);
            return Ok();
        }

        /// <summary>
        /// Create Swab Charge
        /// </summary>
        /// <param name="swabCupDto">swabCupDto</param>
        [Route(ApiRoutes.Tickets.AddSwabCharge)]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateTicketSpecResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddSwabCharge(SwabCupsDto swabCupDto)
        {
            await _ticketService.AddSwabCharge(swabCupDto);
            return Ok();
        }

        /// <summary>
        /// Remove Swab Charge
        /// </summary>
        /// <param name="swabCupsDto">swabCupsDto</param>
        [Route(ApiRoutes.Tickets.DeleteSwabCharge)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveSwabCup(SwabCupsDto swabCupsDto)
        {
            await _ticketService.RemoveSwabCharge(swabCupsDto);
            return Ok();
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
        /// Download Ticket
        /// </summary>
        /// <param name="ticketIds">Ticket Ids</param>
        [Route(ApiRoutes.Tickets.DownloadMultiple)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadTickets([FromBody]string[] ticketIds)
        {
            using var compressedFileStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {
                foreach (var ticketId in ticketIds)
                {
                    var zipEntry = zipArchive.CreateEntry($"Ticket_{ticketId}.pdf");
                    var ticketBytes = await _ticketService.DownloadTicket(ticketId);
                    using var ticketStream = new MemoryStream(ticketBytes);
                    using var zipEntryStream = zipEntry.Open();
                    ticketStream.CopyTo(zipEntryStream);
                }
            }

            var zipFileResult = compressedFileStream.ToArray();
            var resultStream = new MemoryStream(zipFileResult);
            
            return File(resultStream, "application/zip", $"Tickets.zip");
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
