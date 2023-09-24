using CA.Ticketing.Business.Services.Customers.Dto;

namespace CA.Ticketing.Business.Services.Scheduling.Dto
{
    public class SchedulingDtoExtended : SchedulingDto
    {
        public CustomerLocationDto? CustomerLocation { get; set; }
    }
}
