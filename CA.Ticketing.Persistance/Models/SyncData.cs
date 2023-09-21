using CA.Ticketing.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.SyncData)]
    public class SyncData
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime? LastSyncDate { get; set; }

        public string Changes { get; set; }
    }
}
