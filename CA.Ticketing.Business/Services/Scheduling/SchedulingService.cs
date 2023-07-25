using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CA.Ticketing.Common.Constants.ApiRoutes;

namespace CA.Ticketing.Business.Services.Scheduling
{
    public class SchedulingService : EntityServiceBase, ISchedulingService
    {

        public SchedulingService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<SchedulingDetailsDto>> GetAll()
        {
            var scheduling = await _context.Scheduling
                                     .Include(s => s.Customer)
                                     .Include(s => s.Equipment)
                                     .ToListAsync();

            return scheduling.Select(x => _mapper.Map<SchedulingDetailsDto>(x));

        }

        public async Task<int> Create(SchedulingDto entity)
        {
            var scheduling = _mapper.Map<CA.Ticketing.Persistance.Models.Scheduling>(entity);

            _context.Scheduling.Add(scheduling);
            await _context.SaveChangesAsync();
            return scheduling.Id;

        }

        public async Task Delete(int id)
        {
            var scheduling = await GetScheduling(id);

            _context.Scheduling.Remove(scheduling);

            await _context.SaveChangesAsync();
        }
  

        public async Task<SchedulingDetailsDto> GetById(int id)
        {
            var scheduling = await GetScheduling(id);
            return _mapper.Map<SchedulingDetailsDto>(scheduling);
        }

        public async Task Update(SchedulingDto entity)
        {
            var scheduling = await GetScheduling(entity.Id);

            _mapper.Map(entity, scheduling);

            await _context.SaveChangesAsync();
        }

        private async Task<CA.Ticketing.Persistance.Models.Scheduling> GetScheduling(int id)
        {
            var scheduling = await _context.Scheduling
                         .Include(s => s.Customer)
                         .Include(s => s.Equipment)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (scheduling == null)
            {
                throw new KeyNotFoundException(nameof(CA.Ticketing.Persistance.Models.Scheduling));
            }

            return scheduling!;
        }

    }
}
