using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.CustomerContacts)]
    public class CustomerContact : IdentityModel
    {
        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(CustomerLocation))]
        public string? CustomerLocationId { get; set; }

        [JsonIgnore]
        public virtual CustomerLocation? CustomerLocation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool InviteSent { get; set; }

        public DateTime? InviteSentOn { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
