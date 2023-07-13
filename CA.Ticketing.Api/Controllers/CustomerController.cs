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

        /// <summary>
        /// Get Customer by id
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>CustomerDetailsDto</returns>
        [Route(ApiRoutes.Customers.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int customerId)
        {
            var customer = await _customerService.GetById(customerId);
            return Ok(customer);
        }

        /// <summary>
        /// Create a Customer
        /// </summary>
        /// <param name="customer">CustomerDetailsDto</param>
        /// <returns>Customer Id</returns>
        [Route(ApiRoutes.Customers.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CustomerDetailsDto customer)
        {
            var customerId = await _customerService.Create(customer);
            return Ok(customerId);
        }

        ///<summary>
        /// Edit a Customer
        /// </summary>
        /// <param name="customer">CustomerDetailsDto</param>
        [Route(ApiRoutes.Customers.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(CustomerDetailsDto customer)
        {
            await _customerService.Update(customer);
            return Ok();
        }

        ///<summary>
        /// Delete a Customer
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        [Route(ApiRoutes.Customers.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int customerId)
        {
            await _customerService.Delete(customerId);
            return Ok();
        }
    }
}
