using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentChargeDto : EntityDtoBase
    {
        public string Name { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public double Rate { get; set; }
    }
}
