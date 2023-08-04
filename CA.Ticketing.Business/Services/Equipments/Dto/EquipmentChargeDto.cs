using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentChargeDto : EntityDtoBase<int>
    {
        public int EquipmentId { get; set; }

        public int ChargeId { get; set; }
        
        public double Rate { get; set; }

        public string ItemName { get; set; }
    }
}
