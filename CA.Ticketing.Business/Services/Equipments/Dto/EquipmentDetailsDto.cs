using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentDetailsDto : EquipmentDto
    {
        public int Year { get; set; }

        public string VinNumber { get; set; }

        public string PermitNumber { get; set; }

        public DateTime? PermitExpirationDate { get; set; }

        public DateTime? LastMaintenance { get; set; }
    }
}
