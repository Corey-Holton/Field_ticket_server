using CA.Ticketing.Business.Services.Equipments.Dto;

namespace CA.Ticketing.Business.Services.Equipments
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAll();

        Task<IEnumerable<EquipmentDto>> GetAllByCategory(int equipmentCategory);

        Task<EquipmentDetailsDto> GetById(int equipmentId);

        Task<int> Create(EquipmentDetailsDto entity);

        Task Update(EquipmentDetailsDto entity);

        Task Delete(int id);

        Task<IEnumerable<EquipmentChargeDto>> GetEquipmentCharges(int id);

        Task<int> CreateEquipmentCharge(EquipmentChargeDto entity);

        Task UpdateEquipmentCharge(EquipmentChargeDto entity);
    }
}
