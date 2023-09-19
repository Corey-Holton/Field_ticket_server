using CA.Ticketing.Business.Services.Base;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class CreateCustomerContactLoginDto : BaseRedirectUrlRequest
    {
        [Required]
        public string CustomerContactId { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
