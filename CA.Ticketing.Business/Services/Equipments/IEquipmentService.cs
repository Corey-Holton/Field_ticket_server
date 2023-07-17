using CA.Ticketing.Business.Services.Equipments.Dto;

namespace CA.Ticketing.Business.Services.Equipments
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAll();

        Task<EquipmentDetailsDto> GetById(int equipmentId);

        Task<int> Create(EquipmentDetailsDto entity);

        Task Update(EquipmentDetailsDto entity);

        Task Delete(int id);

        Task<int> CreateEquipmentCharge(EquipmentChargeDto entity);

        Task UpdateEquipmentCharge(EquipmentChargeDto entity);
    }
}
