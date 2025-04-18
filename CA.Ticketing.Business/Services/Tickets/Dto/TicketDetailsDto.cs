﻿namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDetailsDto : TicketDto
    {
        public string Description { get; set; }

        public string EquipmentId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? LocationId { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        public string SendEmailTo { get; set; }

        public IEnumerable<TicketSpecificationDto> TicketSpecificationsLeft { get; set; } = new List<TicketSpecificationDto>();

        public IEnumerable<TicketSpecificationDto> TicketSpecificationsRight { get; set; } = new List<TicketSpecificationDto>();

        public IEnumerable<PayrollDataDto> PayrollData { get; set; } = new List<PayrollDataDto>();

        public bool IsSentToCustomer { get; set; }

        public DateTime? SentToCustomerOn { get; set; }
    }
}
