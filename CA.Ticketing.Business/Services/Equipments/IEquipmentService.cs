﻿using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments.Dto;

namespace CA.Ticketing.Business.Services.Equipments
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAll();

        Task<IEnumerable<EquipmentDto>> GetAllByCategory(int equipmentCategory);

        Task<EntityDtoBase?> GetByEmployeeAssigned();
        Task<EquipmentDetailsDto> GetById(string equipmentId);

        Task<string> Create(EquipmentDetailsDto entity);

        Task Update(EquipmentDetailsDto entity);

        Task Delete(string id);

        Task<IEnumerable<EquipmentChargeDto>> GetEquipmentCharges(string id);

        Task UpdateEquipmentCharges(IEnumerable<EquipmentChargeDto> charges);

        Task<IEnumerable<EquipmentDetailsDto>> GetExpiringPermitEquipment();

        Task<IEnumerable<RigWithNextJobDto>> GetRigsWithJobData(DateTime today);

        Task UploadFile(Stream fileStream, string equipmentId, string assignedName, string fileName, string contentType);

        Task<IEnumerable<EquipmentFileDto>> GetFilesList(string equipmentId);

        Task<(byte[] FileBytes, EquipmentFileDto FileDto)> DownloadFile(string fileId);

        Task DeleteFile(string fileId);
    }
}
