namespace CA.Ticketing.Business.Services.Sync.Dto
{
    public class SyncInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ServerName { get; set; }

        public string LastSyncDate { get; set; }
    }
}
