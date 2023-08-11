using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDetailsDto : TicketDto
    {
        public string Description { get; set; }

        public int EquipmentId { get; set; }

        public int LocationId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Mileage { get; set; }


    }
}
