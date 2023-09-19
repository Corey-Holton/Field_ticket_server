namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public interface IIdentityModel : ISyncedEntity
    {
        string Id { get; set; }
    }
}
