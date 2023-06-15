namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public interface IIdentityModel<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
