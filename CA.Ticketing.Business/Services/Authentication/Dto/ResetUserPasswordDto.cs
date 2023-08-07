using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class ResetUserPasswordDto
    {
        public string UserId { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
