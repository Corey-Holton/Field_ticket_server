using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Equipments;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Authorize(Policy = Policies.ApplicationManagers)]
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
        [Authorize(Policy = Policies.CompanyUsers)]
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
        [Authorize(Policy = Policies.CompanyUsers)]
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
        [Authorize(Policy = Policies.CompanyUsers)]
        public async Task<IActionResult> GetById(string equipmentId)
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
        public async Task<IActionResult> Delete(string equipmentId)
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
        public async Task<IActionResult> GetEquipmentCharges(string equipmentId)
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
        public async Task<IActionResult> GetRigsNotWorking(DateTime? today)
        {
            var rigs = await _equipmentService.GetRigsWithJobData(today ?? DateTime.Today);
            return Ok(rigs);
        }

        ///<summary>
        /// List equipment files
        /// </summary>
        /// <param name="equipmentId">Equipment Id</param>
        [Route(ApiRoutes.Equipment.FilesList)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentFileDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.CompanyUsers)]
        public async Task<IActionResult> GetEquipmentFiles(string equipmentId)
        {
            var files = await _equipmentService.GetFilesList(equipmentId);
            return Ok(files);
        }

        /// <summary>
        /// Equipment File Upload
        /// </summary>
        /// <param name="equipmentId">Equipment Id</param>
        /// <param name="fileBytes">File Upload</param>
        /// /// <param name="fileName">File Name</param>
        [Route(ApiRoutes.Equipment.Upload)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FileUpload(string equipmentId, IFormFile fileBytes, string fileName)
        {
            await _equipmentService.UploadFile(fileBytes.OpenReadStream(), equipmentId, fileName, fileBytes.FileName, fileBytes.ContentType);
            return Ok();
        }

        /// <summary>
        /// Equipment File Download
        /// </summary>
        /// <param name="fileId">File Id</param>
        [Route(ApiRoutes.Equipment.Download)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.CompanyUsers)]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var (FileBytes, FileDto) = await _equipmentService.DownloadFile(fileId);
            var stream = new MemoryStream(FileBytes);
            return File(stream, FileDto.ContentType, $"{FileDto.FileName}");
        }

        /// <summary>
        /// Equipment File Delete
        /// </summary>
        /// <param name="fileId">File Id</param>
        [Route(ApiRoutes.Equipment.DeleteFile)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            await _equipmentService.DeleteFile(fileId);
            return Ok();
        }
    }
}
