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
    }
}
