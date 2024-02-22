using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Sync
{
    public interface ISyncInfoService
    {
        Task<SyncServerInfo> UpdateSync(string syncId, DateTime dateTime);
    }
}
