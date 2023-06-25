using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class SetCustomerPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
