using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.TicketSpecifications.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDetailsDto : EntityDtoBase<int>
    {
        public string TicketIdentifier { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Description { get; set; }

        public string ServiceType { get; set; }

        public int EquipmentId { get; set; }

        public int CustomerId { get; set; }

        public int LocationId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Mileage { get; set; }

        public bool Signature { get; set; }

        public List<CreateTicketSpecificationDto> TicketSpecifications { get; set; }
    }
}
