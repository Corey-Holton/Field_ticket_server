using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public class IdentityModel<TKey> : IIdentityModel<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
