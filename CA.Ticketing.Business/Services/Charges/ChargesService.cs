using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Charges
{
    public class ChargesService : EntityServiceBase, IChargesService
    {
        public ChargesService(CATicketingContext context, IMapper mapper) : base (context, mapper) { }

        public async Task<IEnumerable<ChargeDto>> GetAll()
        {
            var charges = await _context.Charges
                .ToListAsync();
            return charges.Select(x => _mapper.Map<ChargeDto>(x));
        }

        public async Task Update(ChargeDto entity)
        {
            var charge = await GetCharge(entity.Id);
            _mapper.Map(entity, charge);
            await _context.SaveChangesAsync();
            await VerifyRigCharges(charge);
        }

        private async Task VerifyRigCharges(Charge charge)
        {
            var equipmentCharges = await _context.EquipmentCharges
                .Where(x => x.ChargeId == charge.Id)
                .ToListAsync();

            if (!charge.IsRigSpecific && equipmentCharges.Any())
            {
                _context.EquipmentCharges.RemoveRange(equipmentCharges);
            }

            if (charge.IsRigSpecific && !equipmentCharges.Any())
            {
                var allRigs = await _context.Equipment
                    .Where(x => x.Category == EquipmentCategory.Rig)
                    .ToListAsync();

                foreach (var rig in allRigs)
                {
                    _context.EquipmentCharges.Add(new EquipmentCharge 
                    { 
                        ChargeId = charge.Id,
                        EquipmentId = rig.Id,
                        Rate = charge.DefaultRate
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Charge> GetCharge(int id)
        {
            var charge = await _context.Charges
                .SingleOrDefaultAsync(x => x.Id == id);
            if (charge == null)
            {
                throw new KeyNotFoundException(nameof(Charge));
            }

            return charge;
        }
    }
}
