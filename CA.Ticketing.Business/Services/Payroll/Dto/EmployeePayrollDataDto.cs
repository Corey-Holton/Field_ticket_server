using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Payroll.Dto
{
    public class EmployeePayrollDataDto
    {
        [JsonIgnore]
        public DateTime ExecutionTime { get; set; }

        public string Employee { get; set; }

        public double RegularHours { get; set; }

        public double OvertimeHours { get; set; }

        public double TotalHours { get; set; }

        public double Mileage { get; set; }

        public double TotalMileage { get; set; }

        public double TotalAmount { get; set; }
    }
}
