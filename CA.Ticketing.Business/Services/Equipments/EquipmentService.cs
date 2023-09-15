using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using CA.Ticketing.Common.Extensions;

namespace CA.Ticketing.Business.Services.Equipments
{
    public class EquipmentService : EntityServiceBase, IEquipmentService
    {
        public EquipmentService(CATicketingContext context, IMapper mapper ) : base (context, mapper) {}

        public async Task<IEnumerable<EquipmentDto>> GetAll()
        {
            return (await _context.Equipment
                .ToListAsync()).Select(x => _mapper.Map<EquipmentDto>(x));
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllByCategory(int equipmentCategory)
        {
            return (await _context.Equipment
                .Where(x => (int)x.Category == equipmentCategory)
                .ToListAsync()).Select(x => _mapper.Map<EquipmentDto>(x));
        }


        public async Task<EquipmentDetailsDto> GetById(int id)
        {
            var equipment = await GetEquipment(id);
            return _mapper.Map<EquipmentDetailsDto>(equipment);
        }

        public async Task<int> Create(EquipmentDetailsDto entity)
        {
            var equipment = _mapper.Map<Equipment>(entity);
            
            if (equipment.Category == EquipmentCategory.Rig)
            {
                var allEquipmentCharges = await _context.Charges
                    .Where(x => x.IsRigSpecific)
                    .ToListAsync();

                foreach (var equipmentCharge in allEquipmentCharges)
                {
                    equipment.Charges.Add(new EquipmentCharge
                    {
                        ChargeId = equipmentCharge.Id,
                        Rate = equipmentCharge.DefaultRate
                    });
                }
            }
            
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment.Id;
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

        public async Task<IEnumerable<EquipmentChargeDto>> GetEquipmentCharges(int id)
        {
            var equipmentCharges = await _context.EquipmentCharges
                .Include(x => x.Charge)
                .Where(x => x.EquipmentId == id)
                .ToListAsync();
            return equipmentCharges.Select(x => _mapper.Map<EquipmentChargeDto>(x));
        }

        public async Task UpdateEquipmentCharges(IEnumerable<EquipmentChargeDto> chargesDto)
        {
            var chargesIds = chargesDto
                .Select(x => x.Id)
                .ToList();

            var charges = await _context.EquipmentCharges
                .Where(x => chargesIds.Contains(x.Id))
                .ToListAsync();

            foreach (var chargeDto in chargesDto)
            {
                var charge = charges.Single(x => x.Id == chargeDto.Id);
                _mapper.Map(chargeDto, charge);
            }
            
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

        public async Task<IEnumerable<EquipmentDetailsDto>> GetExpiringPermitEquipment()
        {
            return (await _context.Equipment.ToListAsync())
                .Where(x => DayTimeExtensions.IsWithinMonth(x.PermitExpirationDate))
                .Select(x => _mapper.Map<EquipmentDetailsDto>(x));
        }

        public async Task<IEnumerable<RigWithNextJobDto>> GetRigsWithJobData()
        {
            return (await _context.Equipment
                .Where(x => x.Category == EquipmentCategory.Rig)
                .ToListAsync())
                .Select(rig =>
                {
                    var nextJob = _context.Scheduling
                        .OrderBy(schedule => schedule.StartTime)
                        .FirstOrDefault(x => x.EquipmentId == rig.Id && x.StartTime > DateTime.UtcNow);

                    var lastJob = _context.FieldTickets
                        .OrderByDescending(x => x.ExecutionDate)
                        .FirstOrDefault(x => x.EquipmentId == rig.Id);

                    var rigDto = _mapper.Map<EquipmentDto>(rig);

                    return new RigWithNextJobDto
                    {
                        Rig = rigDto,
                        DaysUntilNextJob = (nextJob?.StartTime - DateTime.UtcNow)?.Days ?? -1,
                        DaysSinceLastJob = (DateTime.UtcNow - lastJob?.ExecutionDate)?.Days ?? -1
                    };
                });
        }
    }
}
