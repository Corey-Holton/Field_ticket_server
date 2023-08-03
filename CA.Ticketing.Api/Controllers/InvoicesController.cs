using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Invoices;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
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
        /// Get a list of Invoices by start and end date
        /// </summary>
        /// <returns>List of invoices</returns>
        [Route(ApiRoutes.Invoices.ListByDates)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDates(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceService.GetByDates(startDate, endDate);
            return Ok(invoices);
        }

        /// <summary>
        /// Get a list of invoces by searching customer name
        /// </summary>
        /// <returns>List of invoices</returns>
        [Route(ApiRoutes.Invoices.ListByCustomer)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCustomer(string search)
        {
            var invoices = await _invoiceService.GetByCustomer(search);
            return Ok(invoices);
        }


        /// <summary>
        /// Get a ticket by Id
        /// </summary>
        /// <returns>Invoice Details</returns>
        [Route(ApiRoutes.Invoices.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(InvoiceDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _invoiceService.GetById(invoiceId);
            return Ok(invoice);
        }

        /// <summary>
        /// Create an invoice
        /// </summary>
        /// <returns>Invoice Id</returns>
        [Route(ApiRoutes.Invoices.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateInvoiceDto invoice)
        {
            var invoiceId = await _invoiceService.Create(invoice);
            return Ok(invoiceId);
        }

        /// <summary>
        /// Update an invoice
        /// </summary>
        [Route(ApiRoutes.Invoices.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(CreateInvoiceDto invoice)
        {
            await _invoiceService.Update(invoice);
            return Ok();
        }

        /// <summary>
        /// Delete an invoice
        /// </summary>
        [Route(ApiRoutes.Invoices.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            await _invoiceService.Delete(invoiceId);
            return Ok();
        }
    }
}
