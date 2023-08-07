using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Employees)]
    public class Employee : IdentityModelWithAddress<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? DoB { get; set; }

        public JobTitle JobTitle { get; set; }

        public string EmployeeNumber { get; set; }

        public string SSN { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public double PayRate { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey(nameof(AssignedRig))]
        public int? AssignedRigId { get; set; }

        public virtual Equipment? AssignedRig { get; set; }
    }
}
