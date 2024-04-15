using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketTypeDetailsDto : TicketDto
    {
        public string Description { get; set; }

        public string EquipmentId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? LocationId { get; set; }

        public double Mileage { get; set; }

        public double CompanyHours { get; set; }

        public string SendEmailTo { get; set; }

        public IEnumerable<TicketSpecificationDto> TicketSpecificationsData { get; set; } = new List<TicketSpecificationDto>(); 

        public IEnumerable<TicketSpecificationDto> AllTicketSpecifications { get; set; } = new List<TicketSpecificationDto>();

        public IEnumerable<PayrollDataDto> PayrollData { get; set; } = new List<PayrollDataDto>();

        public IEnumerable<WellRecordDto> WellRecord { get; set; } = new List<WellRecordDto>();

        public IEnumerable<SwabCupsDto> SwabCups { get; set; } = new List<SwabCupsDto>();

        public string OtherText { get; set; }

        public bool IsSentToCustomer { get; set; }

        public DateTime? SentToCustomerOn { get; set; }
    }
}
