using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using CA.Ticketing.Business.Services.Scheduling;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Api.Extensions;

namespace CA.Ticketing.Api.Controllers
{
    public class SchedulingController : BaseController
    {
        private readonly ISchedulingService _schedulingService;
        public SchedulingController(ISchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        /// <summary>
        /// Get a list of scheduling
        /// </summary>
        /// <returns>List of Scheduling</returns>
        [Route(ApiRoutes.Scheduling.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SchedulingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var scheduling = await _schedulingService.GetAll();
            return Ok(scheduling);
        }

        /// <summary>
        /// Create a Scheduling
        /// </summary>
        /// <param name="scheduling">SchedulingDto</param>
        /// <returns>Scheduling Id</returns>
        [Route(ApiRoutes.Scheduling.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(SchedulingDto scheduling)
        {
            var shedulingId = await _schedulingService.Create(scheduling);
            return Ok(shedulingId);
        }

        ///<summary>
        /// Edit a scheduling
        /// </summary>
        /// <param name="scheduling">SchedulingDto</param>
        [Route(ApiRoutes.Scheduling.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(SchedulingDto scheduling)
        {
            await _schedulingService.Update(scheduling);
            return Ok();
        }

        ///<summary>
        /// Delete a Scheduling
        /// </summary>
        /// <param name="schedulingId">Scheduling Id</param>
        [Route(ApiRoutes.Scheduling.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string schedulingId)
        {
            await _schedulingService.Delete(schedulingId);
            return Ok();
        }

    }
}
