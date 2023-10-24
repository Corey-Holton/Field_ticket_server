using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class PayrollDataDto : EntityDtoBase
    {
        [JsonIgnore]
        public JobTitle? JobTitle { get; set; }

        public string Labor => JobTitle?.GetJobTitle() ?? string.Empty;

        public string FieldTicketId { get; set; }

        public string? EmployeeId { get; set; }

        public string DisplayEmployeeId { get; set; }

        public EmployeeDto? Employee { get; set; }

        public string Name { get; set; }

        public double RigHours { get; set; }

        public double TravelHours { get; set; }

        public double YardHours { get; set; }

        public double RoustaboutHours { get; set; }
    }
}
