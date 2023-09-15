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
            var equipmentUnfiltered = await _context.Equipment.ToListAsync();
            var equipment = new List<Equipment>();
            foreach( var item in equipmentUnfiltered)
            {
                if (DayTimeExtensions.IsWithinMonth(item.PermitExpirationDate)) 
                    equipment.Add(item);
            }
            return equipment.Select(x => _mapper.Map<EquipmentDetailsDto>(x));
        }

        public async Task<IEnumerable<RigWithNextJobDto>> GetRigsNotWorking()
        {
            var allRigs = await _context.Equipment
                .Where(x => (int)x.Category == 1)
                .ToListAsync();

            var scheduling = await _context.Scheduling
                                     .Include(s => s.Equipment)
                                     .ToListAsync();

            var rigsNotWorking = allRigs.Select(rig =>
            {
                var nextJob = scheduling
                    .Where(schedule =>
                        schedule.EquipmentId == rig.Id &&
                        schedule.StartTime >= DateTime.Now)
                    .OrderBy(schedule => schedule.StartTime)
                    .FirstOrDefault();

                int daysUntilNextJob = nextJob != null
                    ? ((TimeSpan)(nextJob.StartTime - DateTime.Now)).Days
                    : -1;

                var rigDto = _mapper.Map<EquipmentDto>(rig);

                return new RigWithNextJobDto
                {
                    Rig = rigDto,
                    DaysUntilNextJob = daysUntilNextJob
                };
            }).ToList();

            return rigsNotWorking;
        }
    }
}
