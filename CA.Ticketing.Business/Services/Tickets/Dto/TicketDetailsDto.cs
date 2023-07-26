using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDetailsDto : EntityDtoBase<int>
    {
        public string TicketIdentifier { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Description { get; set; }

        public ServiceType ServiceType { get; set; }

        public int EquipmentId { get; set; }

        public int CustomerId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Mileage { get; set; }
    }
}
