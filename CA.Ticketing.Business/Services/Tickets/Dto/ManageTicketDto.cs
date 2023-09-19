using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class ManageTicketDto : EntityDtoBase
    {
        public ServiceType ServiceType { get; set; }

        public string EquipmentId { get; set; }

        public string? CustomerId { get; set; }

        public string? CustomerLocationId { get; set; }

        public DateTime ExecutionDate { get; set; }
    }
}
