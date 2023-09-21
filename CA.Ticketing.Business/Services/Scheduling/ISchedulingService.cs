using CA.Ticketing.Business.Services.Scheduling.Dto;

namespace CA.Ticketing.Business.Services.Scheduling
{
    public interface ISchedulingService
    {
        Task<IEnumerable<SchedulingDto>> GetAll();

        Task<string> Create(SchedulingDto entity);

        Task Update(SchedulingDto entity);

        Task Delete(string id);

        Task<IEnumerable<SchedulingDto>> GetUserJobs();
    }
}
