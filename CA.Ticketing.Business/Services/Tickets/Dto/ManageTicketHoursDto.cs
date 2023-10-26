namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class ManageTicketHoursDto
    {
        public string TicketId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        public string Description { get; set; }
    }
}
