using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.CustomerContacts)]
    public class CustomerContact : IdentityModel<int>
    {
        [ForeignKey(nameof(CustomerLocation))]
        public int CustomerLocationId { get; set; }

        public virtual CustomerLocation CustomerLocation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool InviteSent { get; set; }

        public DateTime? InviteSentOn { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
