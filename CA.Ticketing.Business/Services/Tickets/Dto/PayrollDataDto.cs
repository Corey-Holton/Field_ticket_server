using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees.Dto;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class PayrollDataDto : EntityDtoBase
    {
        public string FieldTicketId { get; set; }

        public string? EmployeeId { get; set; }

        public EmployeeDto? Employee { get; set; }

        public string Name { get; set; }

        public double YardHours { get; set; }

        public double RoustaboutHours { get; set; }
    }
}
