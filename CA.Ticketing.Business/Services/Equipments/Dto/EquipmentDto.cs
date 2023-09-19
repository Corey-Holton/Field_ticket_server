using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentDto : EntityDtoBase
    {
        public EquipmentCategory Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? PermitExpirationDate { get; set; }

        public DateTime? LastMaintenance { get; set; }
    }
}
