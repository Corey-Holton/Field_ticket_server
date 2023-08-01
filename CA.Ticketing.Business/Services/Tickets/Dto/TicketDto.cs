using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDto : EntityDtoBase<int>
    {
        public string TicketIdentifier { get; set; }

        public string EquipmentName { get; set; }

        public string CustomerName { get; set; }
        
        public DateTime ExecutionDate { get; set; }

        public string ServiceType { get; set; }

        public bool Signature { get; set; }

        public bool Invoice { get; set; }
    }
}
