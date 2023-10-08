using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Sync.Dto
{
    public class ServerStatus
    {
        public bool IsOnline { get; set; } = false;

        public bool InitialSyncInProgress { get; set; } = true;

        public bool SyncInProgress { get; set; } = false;

        public DateTime? LastSyncDate { get; set; }

        [JsonIgnore]
        public DateTime NextSyncOn { get; set; }

        public int NextSyncIn => (int)Math.Ceiling((NextSyncOn - DateTime.UtcNow).TotalMinutes);
    }
}
