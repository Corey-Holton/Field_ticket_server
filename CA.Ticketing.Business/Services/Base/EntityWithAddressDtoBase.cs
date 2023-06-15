namespace CA.Ticketing.Business.Services.Base
{
    public class EntityWithAddressDtoBase<TKey> : EntityDtoBase<TKey> where TKey : IEquatable<TKey>
    {
        public string Address { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }
    }
}
