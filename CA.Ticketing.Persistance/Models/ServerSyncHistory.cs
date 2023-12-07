using CA.Ticketing.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.ServerSyncHistory)]
    public class ServerSyncHistory
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ServerName { get; set; }
        public DateTime? LastSyncDate { get; set; }

    }
}
