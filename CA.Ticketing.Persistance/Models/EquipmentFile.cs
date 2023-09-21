using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    public class EquipmentFile : IdentityModel, IFileEntity
    {
        [ForeignKey(nameof(Equipment))]
        public string EquipmentId { get; set; }

        [JsonIgnore]
        public virtual Equipment Equipment { get; set; }

        public string FileName { get; set; }

        public string FileIndicator { get; set; }

        public string ContentType { get; set; }

        public string DisplayName { get; set; }

        [NotMapped]
        public byte[]? FileBytes { get; set; }
    }
}
