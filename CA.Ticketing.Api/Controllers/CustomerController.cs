using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Customers;
using CA.Ticketing.Business.Services.Customers.Dto;
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
        [ProducesResponseType(typeof(IEnumerable<CustomerDetailsDto>), StatusCodes.Status200OK)]
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
        public async Task<IActionResult> GetById(string customerId)
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
        public async Task<IActionResult> Delete(string customerId)
        {
            await _customerService.Delete(customerId);
            return Ok();
        }

        ///<summary>
        /// Add a Customer Location
        /// </summary>
        /// <param name="customerLocation">CustomerLocationDto</param>
        [Route(ApiRoutes.Customers.AddLocation)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddLocation(CustomerLocationDto customerLocation)
        {
            var locationId =await _customerService.AddLocation(customerLocation);
            return Ok(locationId);
        }

        /// <summary>
        /// Edit an existing Location
        /// </summary>
        /// <param name="customerLocation">CustomerLocationDto</param>
        [Route(ApiRoutes.Customers.UpdateLocation)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLocation(CustomerLocationDto customerLocation)
        {
            await _customerService.UpdateLocation(customerLocation);
            return Ok();
        }

        /// <summary>
        /// Delete an existing Location
        /// </summary>
        /// <param name="customerLocationId">Customer Location Id</param>
        [Route(ApiRoutes.Customers.DeleteLocation)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLocation(string customerLocationId)
        {
            await _customerService.DeleteLocation(customerLocationId);
            return Ok();
        }

        /// <summary>
        /// Add a contact for an existing Location
        /// </summary>
        /// <param name="customerContact">Customer Contact Details</param>
        [Route(ApiRoutes.Customers.AddContact)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddContact(CustomerContactDto customerContact)
        {
            var customerId = await _customerService.AddContact(customerContact);
            return Ok(customerId);
        }

        /// <summary>
        /// Update an existing contact
        /// </summary>
        /// <param name="customerContact">Update Contact Details</param>
        [Route(ApiRoutes.Customers.UpdateContact)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateContact(CustomerContactDto customerContact)
        {
            await _customerService.UpdateContact(customerContact);
            return Ok();
        }

        /// <summary>
        /// Delete an existing contact
        /// </summary>
        /// <param name="contactId">Contact Id</param>
        [Route(ApiRoutes.Customers.DeleteContact)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteContact(string contactId)
        {
            await _customerService.DeleteContact(contactId);
            return Ok();
        }

        ///<summary>
        /// Invite customer to setup password and access application
        /// </summary>
        /// <param name="loginDto">Customer Contact Login Dto</param>
        [Route(ApiRoutes.Customers.AddLogin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddLogin(CustomerLoginDto loginDto)
        {
            await _customerService.AddLogin(loginDto);
            return Ok();
        }

        ///<summary>
        /// Resend invite for customer login
        /// </summary>
        /// <param name="customerLoginDto">Customer Contact Login Dto</param>
        [Route(ApiRoutes.Customers.ResendInvite)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResendCustomerInvite(CustomerLoginDto customerLoginDto)
        {
            await _customerService.ResendInvitation(customerLoginDto);
            return Ok();
        }

        ///<summary>
        /// Reset customer contact password
        /// </summary>
        /// <param name="resetCustomerContactPasswordDto">resetCustomerContactPasswordDto</param>
        [Route(ApiRoutes.Customers.ResetPassword)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordDto)
        {
            await _customerService.ResetPassword(resetCustomerContactPasswordDto);
            return Ok();
        }

        ///<summary>
        ///Delete customer contact login
        ///</summary>
        ///<param name="customerContactId">customerContactId</param>
        [Route(ApiRoutes.Customers.DeleteLogin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLogin(string customerContactId)
        {
            await _customerService.DeleteLogin(customerContactId);
            return Ok();
        }
    }
}
