using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Customers;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Common;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get a list of Customers
        /// </summary>
        /// <returns>List of Customers</returns>
        [Route(ApiRoutes.Customers.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAll();
            return Ok(customers);
        }
    }
}
