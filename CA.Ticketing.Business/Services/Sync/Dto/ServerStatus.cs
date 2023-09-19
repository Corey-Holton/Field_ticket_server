namespace CA.Ticketing.Business.Services.Sync.Dto
{
    public class ServerStatus
    {
        public bool IsOnline { get; set; }

        public DateTime? LastSyncDate { get; set; }
    }
}
