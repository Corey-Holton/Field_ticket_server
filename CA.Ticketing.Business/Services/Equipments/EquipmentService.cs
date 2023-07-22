using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Equipments
{
    public class EquipmentService : EntityServiceBase, IEquipmentService
    {
        public EquipmentService(CATicketingContext context, IMapper mapper ) : base (context, mapper)
        {
            
        }

        public async Task<IEnumerable<EquipmentDto>> GetAll()
        {
            var equipment = await _context.Equipment
                .ToListAsync();
            return equipment.Select(x => _mapper.Map<EquipmentDto>(x));
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllByCategory(int equipmentCategory)
        {
            var equipment = await _context.Equipment
                .Where(x => (int)x.Category == equipmentCategory)
                .ToListAsync();
            return equipment.Select(x => _mapper.Map<EquipmentDto>(x));
        }


        public async Task<EquipmentDetailsDto> GetById(int id)
        {
            var equipment = await GetEquipment(id);
            return _mapper.Map<EquipmentDetailsDto>(equipment);
        }

        public async Task<int> Create(EquipmentDetailsDto entity)
        {
            var equipment = _mapper.Map<Equipment>(entity);

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task Update(EquipmentDetailsDto entity)
        {
            var equipment = await GetEquipment(entity.Id);

            _mapper.Map(entity, equipment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var equipment = await GetEquipment(id);

            _context.Equipment.Remove(equipment);

            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateEquipmentCharge(EquipmentChargeDto entity)
        {
            var equipmentCharge = _mapper.Map<EquipmentCharge>(entity);

            _context.EquipmentCharges.Add(equipmentCharge);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateEquipmentCharge(EquipmentChargeDto entity)
        {
            var equipmentCharge = await GetEquipmentCharge(entity.Id);

            _mapper.Map(entity, equipmentCharge);

            await _context.SaveChangesAsync();
        }

        private async Task<Equipment> GetEquipment(int id)
        {
            var equipment = await _context.Equipment
                .SingleOrDefaultAsync(x => x.Id == id);

            if (equipment == null)
            {
                throw new KeyNotFoundException(nameof(Equipment));
            }

            return equipment!;
        }

        private async Task<EquipmentCharge> GetEquipmentCharge(int id)
        {
            var equipmentCharge = await _context.EquipmentCharges
                .SingleOrDefaultAsync(x => x.Id == id);

            if (equipmentCharge == null)
            {
                throw new KeyNotFoundException(nameof(EquipmentCharge));
            }

            return equipmentCharge!;
        }
    }
}
