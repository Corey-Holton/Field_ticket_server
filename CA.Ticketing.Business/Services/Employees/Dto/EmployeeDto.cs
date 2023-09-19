using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class EmployeeDto : EntityDtoBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitleDisplay { get; set; }

        public DateTime? DoB { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }
    }
}
