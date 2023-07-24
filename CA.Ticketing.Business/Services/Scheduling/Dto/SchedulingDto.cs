using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Scheduling.Dto
{
    public class SchedulingDto : EntityDtoBase<int>
    {
        public DateTime? ArrangeDateTime { get; set; }
        public DateTime? Duration { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int RigId { get; set; }
        public string EquipmentName { get; set; }
        public double locationX { get; set; }
        public double locationY { get; set; }
    }
}
