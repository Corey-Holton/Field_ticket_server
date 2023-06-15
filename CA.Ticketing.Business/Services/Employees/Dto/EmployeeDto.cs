using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class EmployeeDto : EntityWithAddressDtoBase<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Email { get; set; }

        public string Phone { get; set; }

        public DateTime? DoB { get; set; }

        public JobTitle JobTitle { get; set; }

        public string JobTitleDisplay { get; set; }

        public EmployeeStatus Status { get; set; }

        public string EmployeeNumber { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public double PayRate { get; set; }
    }
}
