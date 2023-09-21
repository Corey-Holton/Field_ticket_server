using CA.Ticketing.Business.Services.Sync.Dto;
using Microsoft.Extensions.Hosting;

namespace CA.Ticketing.Business.Services.Sync
{
    public interface IDataSyncService : IHostedService
    {
        ServerStatus GetServerStatus();
    }
}
