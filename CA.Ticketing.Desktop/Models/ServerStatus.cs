using System;

namespace CA.Ticketing.Desktop.Models
{
    public class ServerStatus
    {
        public bool IsOnline { get; set; }

        public bool InitialSyncInProgress { get; set; }

        public bool SyncInProgress { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public int NextSyncIn { get; set; }
    }
}
