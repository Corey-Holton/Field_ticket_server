using CA.Ticketing.Business.Services.Scheduling.Dto;

namespace CA.Ticketing.Business.Services.Scheduling
{
    public interface ISchedulingService
    {
        Task<IEnumerable<SchedulingDto>> GetAll();

        Task<int> Create(SchedulingDto entity);

        Task Update(SchedulingDto entity);

        Task Delete(int id);

    }
}
