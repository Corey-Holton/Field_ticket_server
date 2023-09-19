namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public interface ISyncedEntity
    {
        DateTime CreatedDate { get; set; }

        DateTime LastModifiedDate { get; set; }

        DateTime? DeletedDate { get; set; }
    }
}
