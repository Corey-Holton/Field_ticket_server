using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Invoices;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Authorize(Policy = Policies.ApplicationManagers)]
    public class InvoicesController : BaseController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Get a list of all Invoices
        /// </summary>
        /// <returns>List of invoices</returns>
        [Route(ApiRoutes.Invoices.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _invoiceService.GetAll();
            return Ok(invoices);
        }

        /// <summary>
        /// Create an invoice
        /// </summary>
        /// <returns>Invoice Id</returns>
        [Route(ApiRoutes.Invoices.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateInvoiceDto invoice)
        {
            var invoiceId = await _invoiceService.Create(invoice);
            return Ok(invoiceId);
        }

        /// <summary>
        /// Mark ticket as paid
        /// </summary>
        [Route(ApiRoutes.Invoices.MarkAsPaid)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsPaid(string invoiceId)
        {
            await _invoiceService.MarkAsPaid(invoiceId);
            return Ok();
        }

        /// <summary>
        /// Send to Customer
        /// </summary>
        [Route(ApiRoutes.Invoices.SendToCustomer)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendToCustomer(string invoiceId)
        {
            await _invoiceService.SendToCustomer(invoiceId);
            return Ok();
        }

        /// <summary>
        /// Update an invoice
        /// </summary>
        [Route(ApiRoutes.Invoices.Download)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(string invoiceId)
        {
            var (InvoiceId, InvoiceBytes) = await _invoiceService.Download(invoiceId);
            var stream = new MemoryStream(InvoiceBytes);

            return File(stream, "application/pdf", $"Invoice_{InvoiceId}.pdf");
        }

        /// <summary>
        /// Delete an invoice
        /// </summary>
        [Route(ApiRoutes.Invoices.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string invoiceId)
        {
            await _invoiceService.Delete(invoiceId);
            return Ok();
        }
    }
}
