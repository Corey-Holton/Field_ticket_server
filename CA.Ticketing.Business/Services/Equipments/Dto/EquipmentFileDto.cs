using CA.Ticketing.Business.Services.Base;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentFileDto : EntityDtoBase
    {
        [JsonIgnore]
        public string FileName { get; set; }

        [JsonIgnore]
        public string ContentType { get; set; }

        public string DisplayName { get; set; }
    }
}
