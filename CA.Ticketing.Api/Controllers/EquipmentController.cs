using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.Equipments;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class EquipmentController : BaseController
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        /// <summary>
        /// Get a list of equipment
        /// </summary>
        /// <returns>List of Equipment</returns>
        [Route(ApiRoutes.Equipment.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var equipment = await _equipmentService.GetAll();
            return Ok(equipment);
        }

        /// <summary>
        /// Get a list of equipment by category
        /// </summary>
        /// <param name="equipmentCategory"></param>
        /// <returns>List of Equipment based on category</returns>
        [Route(ApiRoutes.Equipment.ListCategory)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCategory(int equipmentCategory)
        {
            var equipment = await _equipmentService.GetAllByCategory(equipmentCategory);
            return Ok(equipment);
        }

        /// <summary>
        /// Get Equipment by id
        /// </summary>
        /// <param name="equipmentId">Equipment Id</param>
        /// <returns>EquipmentDetailsDto</returns>
        [Route(ApiRoutes.Equipment.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(EquipmentDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int equipmentId)
        {
            var equipment = await _equipmentService.GetById(equipmentId);
            return Ok(equipment);
        }

        /// <summary>
        /// Create an Equipment
        /// </summary>
        /// <param name="equipment">EquipmentDetailsDto</param>
        /// <returns>Equipment Id</returns>
        [Route(ApiRoutes.Equipment.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(EquipmentDetailsDto equipment)
        {
            var equipmentId = await _equipmentService.Create(equipment);
            return Ok(equipmentId);
        }

        ///<summary>
        /// Edit an equipment
        /// </summary>
        /// <param name="equipment">EquipmentDetailsDto</param>
        [Route(ApiRoutes.Equipment.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(EquipmentDetailsDto equipment)
        {
            await _equipmentService.Update(equipment);
            return Ok();
        }

        ///<summary>
        /// Delete an equipment
        /// </summary>
        /// <param name="equipmentId">Equipment Id</param>
        [Route(ApiRoutes.Equipment.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int equipmentId)
        {
            await _equipmentService.Delete(equipmentId);
            return Ok();
        }

        /// <summary>
        /// Get all equipment charges for related equipment 
        /// </summary>
        /// <returns>List of equipment charges</returns>
        [Route(ApiRoutes.Equipment.ListEquipmentCharges)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentChargeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEquipmentCharges(int equipmentId)
        {
            var equipmentCharges = await _equipmentService.GetEquipmentCharges(equipmentId);
            return Ok(equipmentCharges);
        }

        ///<summary>
        /// Update an equipment charge
        /// </summary>
        /// <param name="equipmentCharges"></param>
        [Route(ApiRoutes.Equipment.UpdateEquipmentCharges)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEquipmentCharges(IEnumerable<EquipmentChargeDto> equipmentCharges)
        {
            await _equipmentService.UpdateEquipmentCharges(equipmentCharges);
            return Ok();
        }

        ///<summary>
        /// List of equipment with permit expiration date in next 30 days
        /// </summary>
        [Route(ApiRoutes.Equipment.PermitExpirationDate)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentDetailsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExpiringPermitEquipment()
        {
            var equipment = await _equipmentService.GetExpiringPermitEquipment();
            return Ok(equipment);
        }

        ///<summary>
        /// List of rigs that are not currently working
        /// </summary>
        [Route(ApiRoutes.Equipment.RigsNotWorking)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RigWithNextJobDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRigsNotWorking()
        {
            var rigs = await _equipmentService.GetRigsWithJobData();
            return Ok(rigs);
        }
    }
}
