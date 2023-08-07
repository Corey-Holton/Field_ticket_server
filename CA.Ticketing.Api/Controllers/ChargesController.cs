using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Charges;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class ChargesController : BaseController
    {
        private readonly IChargesService _chargesService;

        public ChargesController(IChargesService chargesService)
        {
            _chargesService = chargesService;
        }

        /// <summary>
        /// Get a list of Charges
        /// </summary>
        /// <returns>List of Charges</returns>
        [Route(ApiRoutes.Charges.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChargeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var charges = await _chargesService.GetAll();
            return Ok(charges);
        }

        ///<summary>
        /// Edit a charge
        /// </summary>
        /// <param name="charge">ChargeDto</param>
        [Route(ApiRoutes.Charges.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(ChargeDto charge)
        {
            await _chargesService.Update(charge);
            return Ok();
        }
    }
}
