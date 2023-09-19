using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Scheduling.Dto
{
    public class SchedulingDto : EntityDtoBase
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string? CustomerLocationId { get; set; }

        public string CustomerLocationName { get; set; }

        public string Description { get; set; }

        public string EquipmentId { get; set; }

        public string EquipmentName { get; set; }
    }
}
