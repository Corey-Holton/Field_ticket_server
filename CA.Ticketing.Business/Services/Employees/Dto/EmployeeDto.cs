using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class EmployeeDto : EntityDtoBase<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public JobTitle JobTitle { get; set; }

        public string JobTitleDisplay { get; set; }

        public EmployeeStatus Status { get; set; }
    }
}
