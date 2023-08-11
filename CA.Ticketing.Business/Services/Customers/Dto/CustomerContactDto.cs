using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerContactDto : EntityDtoBase<int>
    {
        public int CustomerId { get; set; }

        public int? CustomerLocationId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool InviteSent { get; set; }

        public DateTime? InviteSentOn { get; set; }

        public bool HasLogin { get; set; }
    }
}
