using CA.Ticketing.Business.Services.Sync.Dto;

namespace CA.Ticketing.Business.Services.Sync
{
    public interface IDataSyncService
    {
        ServerStatus GetServerStatus();
    }
}
