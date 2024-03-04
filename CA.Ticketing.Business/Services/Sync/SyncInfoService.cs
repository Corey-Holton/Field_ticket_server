using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Sync
{
    public class SyncInfoService : EntityServiceBase, ISyncInfoService
    {
        public SyncInfoService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task UpdateSync(string serverName, DateTime dateTime)
        {
            var server = await _context.ServerSyncHistory.SingleOrDefaultAsync(x => x.ServerName == serverName);
            if (server == null) {
                server = new SyncServerInfo()
                {
                    ServerName = serverName,
                    LastSyncDate = dateTime,
                };
                _context.ServerSyncHistory.Add(server);
            }
            server.LastSyncDate = dateTime;
            await _context.SaveChangesAsync();
        }
    }
}
