using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentDto : EntityDtoBase<int>
    {
        public string Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
