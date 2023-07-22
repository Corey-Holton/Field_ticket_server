namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class EmployeeDetailsDto : EmployeeDto
    {
        public string? Email { get; set; }

        public string Phone { get; set; }

        public DateTime? DoB { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public string EmployeeNumber { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public double PayRate { get; set; }

        public bool HasLogin { get; set; }

        public string Username { get; set; }

        public string SSN { get; set; }
    }
}
