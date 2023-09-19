using CA.Ticketing.Common.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.SyncData)]
    public class SyncData
    {
        public string Id { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public string Changes { get; set; }
    }
}
