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
    [Table(TableNames.Scheduling)]

    public class Scheduling : IdentityModel<int>
    {
        public DateTime? ArrangeDateTime { get; set; }
        public DateTime? Duration { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set;}
        public string Description { get; set; }
        [ForeignKey(nameof(Equipment))]
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

    }
}
