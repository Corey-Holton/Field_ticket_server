using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class ManageTicketDto : EntityDtoBase<int>
    {
        public ServiceType ServiceType { get; set; }

        public int EquipmentId { get; set; }

        public int? CustomerId { get; set; }

        public int? CustomerLocationId { get; set; }

        public DateTime ExecutionDate { get; set; }
    }
}
