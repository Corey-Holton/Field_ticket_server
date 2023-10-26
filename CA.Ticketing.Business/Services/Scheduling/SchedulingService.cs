using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Scheduling
{
    public class SchedulingService : EntityServiceBase, ISchedulingService
    {
        private readonly IUserContext _userContext;

        public SchedulingService(CATicketingContext context, IMapper mapper, IUserContext userContext) : base(context, mapper)
        {
            _userContext = userContext;
        }

        public async Task<IEnumerable<SchedulingDto>> GetAll()
        {
            
            var scheduling = await _context.Scheduling
                                     .Include(s => s.Customer)
                                     .Include(s => s.CustomerLocation)
                                     .Include(s => s.Equipment)
                                     .AsSplitQuery()
                                     .ToListAsync();

            return scheduling.Select(x => _mapper.Map<SchedulingDto>(x));
        }

        public async Task<IEnumerable<SchedulingDtoExtended>> GetUserJobs(DateTime today)
        {
            var user = await _context.Users
                .Include(x => x.Employee)
                .SingleAsync(x => x.Id == _userContext.User!.Id);

            if (user.Employee == null || string.IsNullOrEmpty(user.Employee.AssignedRigId))
            {
                return new List<SchedulingDtoExtended>();
            } 

            var scheduling = await _context.Scheduling
                                     .Include(s => s.Customer)
                                     .Include(s => s.CustomerLocation)
                                     .Include(s => s.Equipment)
                                     .Include(s => s.CustomerContact)
                                     .Where(s => s.EquipmentId == user.Employee.AssignedRigId && s.EndTime >= today)
                                     .AsSplitQuery()
                                     .ToListAsync();

            return scheduling.Select(x => _mapper.Map<SchedulingDtoExtended>(x));
        }

        public async Task<string> Create(SchedulingDto entity)
        {

            var scheduling = _mapper.Map<Persistance.Models.Scheduling>(entity);

            _context.Scheduling.Add(scheduling);
            await _context.SaveChangesAsync();
            return scheduling.Id;

        }

        public async Task Delete(string id)
        {
            var scheduling = await GetScheduling(id);

            _context.Scheduling.Remove(scheduling);

            await _context.SaveChangesAsync();
        }
  
        public async Task Update(SchedulingDto entity)
        {
            var scheduling = await GetScheduling(entity.Id);

            _mapper.Map(entity, scheduling);

            await _context.SaveChangesAsync();
        }

        private async Task<Persistance.Models.Scheduling> GetScheduling(string? id)
        {
            var scheduling = await _context.Scheduling
                         .Include(s => s.Customer)
                         .Include(s => s.Equipment)
                         .Include(s => s.CustomerLocation)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (scheduling == null)
            {
                throw new KeyNotFoundException(nameof(Persistance.Models.Scheduling));
            }

            return scheduling!;
        }

    }
}
