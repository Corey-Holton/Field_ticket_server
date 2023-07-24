using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentChargeDto : EntityDtoBase<int>
    {
        public int EquipmentId { get; set; }

        public int ChargeId { get; set; }
        
        public double Value { get; set; }
    }
}
