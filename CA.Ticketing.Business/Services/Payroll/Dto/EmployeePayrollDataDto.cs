namespace CA.Ticketing.Business.Services.Payroll.Dto
{
    public class EmployeePayrollDataDto
    {
        public string Employee { get; set; }

        public double Jobs { get; set; }

        public double TotalJobs { get; set; }

        public double RegularHours { get; set; }

        public double OvertimeHours { get; set; }

        public double RigHours { get; set; }

        public double TravelHours { get; set; }

        public double YardHours { get; set; }

        public double RoustaboutHours { get; set; }

        public double TotalHours { get; set; }

        public double Mileage { get; set; }

        public double TotalMileage { get; set; }

        public double TotalAmount { get; set; }
        public double TotalQuantity { get; set; }
    }
}
