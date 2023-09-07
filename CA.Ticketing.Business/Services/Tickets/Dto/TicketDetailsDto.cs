namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDetailsDto : TicketDto
    {
        public string Description { get; set; }

        public int EquipmentId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? CustomerId { get; set; }

        public int? LocationId { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        public IEnumerable<TicketSpecificationDto> TicketSpecifications { get; set; } = new List<TicketSpecificationDto>();

        public IEnumerable<PayrollDataDto> PayrollData { get; set; } = new List<PayrollDataDto>();

        public bool HasEmployeeSignature { get; set; }
    }
}
