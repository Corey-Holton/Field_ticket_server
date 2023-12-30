using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Sync.Dto
{
    public class ServerStatus
    {
        private bool _initialSyncInProgress = true;

        public bool IsOnline { get; set; } = false;

        public bool InitialSyncInProgress
        {
            get
            {
                if (IsOnline || !_initialSyncInProgress)
                {
                    return _initialSyncInProgress;
                }

                if (!LastSyncDate.HasValue)
                {
                    return true;
                }

                return (DateTime.UtcNow - LastSyncDate.Value).TotalDays > 30;
            }
            set
            {
                _initialSyncInProgress = value;
            }
        }

        public bool SyncInProgress { get; set; } = false;

        public DateTime? LastSyncDate { get; set; }

        [JsonIgnore]
        public DateTime NextSyncOn { get; set; }

        public int NextSyncIn => (int)Math.Ceiling((NextSyncOn - DateTime.UtcNow).TotalMinutes);
    }
}
