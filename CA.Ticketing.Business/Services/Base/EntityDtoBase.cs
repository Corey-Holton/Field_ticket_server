namespace CA.Ticketing.Business.Services.Base
{
    public class EntityDtoBase<TKey> where TKey : IEquatable<TKey>
    {
        public TKey? Id { get; set; }
    }
}
