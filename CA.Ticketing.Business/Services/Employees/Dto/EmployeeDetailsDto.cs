using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class EmployeeDetailsDto : EmployeeDto
    {
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public string EmployeeNumber { get; set; }

        public double PayRate { get; set; }

        public bool HasLogin { get; set; }

        public string SSN { get; set; }

        public int? AssignedRigId { get; set; }

        public JobTitle JobTitle { get; set; }
    }
}
