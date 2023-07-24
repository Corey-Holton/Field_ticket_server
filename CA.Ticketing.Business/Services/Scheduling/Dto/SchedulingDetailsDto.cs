using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Scheduling.Dto
{
    public class SchedulingDetailsDto
    {
        public DateTime? ArrangeDateTime { get; set; }
        public DateTime? Duration { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string EquipmentName { get; set; }
        public double locationX { get; set; }
        public double locationY { get; set; }
    }
}
