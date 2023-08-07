using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.PayrollData)]
    public class PayrollData : IdentityModel<int>
    {
        [ForeignKey(nameof(Employee))]
        public int? EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        public string Name { get; set; }

        public double RigHours { get; set; }

        public double TravelHours { get; set; }

        public double RoustaboutHours { get; set; }
    }
}
