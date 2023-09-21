namespace CA.Ticketing.Common.Models
{
    public class SyncDataTypeInfo
    {
        public SyncDataTypeInfo()
        {
        }

        public SyncDataTypeInfo(Type type)
        {
            EntityType = type.Name;
            PostLastModifiedDate = DateTime.MinValue;
            GetLastModifiedDate = DateTime.MinValue;
        }

        public string EntityType { get; set; }

        public DateTime PostLastModifiedDate { get; set; }

        public DateTime GetLastModifiedDate { get; set; }
    }
}
