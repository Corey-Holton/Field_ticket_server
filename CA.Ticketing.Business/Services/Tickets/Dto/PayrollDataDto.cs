using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Employees.Dto;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class PayrollDataDto : EntityDtoBase<int>
    {
        public int FieldTicketId { get; set; }

        public int? EmployeeId { get; set; }

        public EmployeeDto? Employee { get; set; }

        public string Name { get; set; }
    }
}
