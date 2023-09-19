using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public class IdentityModel : IIdentityModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
