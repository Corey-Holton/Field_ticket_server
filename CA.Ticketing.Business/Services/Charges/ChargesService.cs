using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Charges.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Charges
{
    public class ChargesService : EntityServiceBase, IChargesService
    {
        public ChargesService(CATicketingContext context, IMapper mapper) : base (context, mapper)
        {

        }

        public async Task<IEnumerable<ChargeDto>> GetAll()
        {
            var charges = await _context.Charges
                .ToListAsync();
            return charges.Select(x => _mapper.Map<ChargeDto>(x));
        }

        public async Task<int> Create(ChargeDto entity)
        {
            var charge = _mapper.Map<Charge>(entity);

            _context.Charges.Add(charge);
            await _context.SaveChangesAsync();
            return charge.Id;
        }

        public async Task Update(ChargeDto entity)
        {
            var charge = await GetCharge(entity.Id);

            _mapper.Map(entity, charge);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var charge = await GetCharge(id);
            _context.Charges.Remove(charge);
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
