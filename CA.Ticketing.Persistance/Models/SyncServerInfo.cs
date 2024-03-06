using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
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
    public class SyncServerInfo : IdentityModel
    {
        public string ServerName { get; set; }
        public DateTime? LastSyncDate { get; set; }

    }
}
