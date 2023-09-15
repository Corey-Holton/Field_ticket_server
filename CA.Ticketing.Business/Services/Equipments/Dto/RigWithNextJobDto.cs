using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class RigWithNextJobDto
    {
        public EquipmentDto Rig { get; set; }
        public int DaysUntilNextJob { get; set; }
    }
}
