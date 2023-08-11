using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Scheduling.Dto
{
    public class SchedulingDto : EntityDtoBase<int>
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int? CustomerLocationId { get; set; }

        public string CustomerLocationName { get; set; }

        public string Description { get; set; }

        public int EquipmentId { get; set; }

        public string EquipmentName { get; set; }
    }
}
