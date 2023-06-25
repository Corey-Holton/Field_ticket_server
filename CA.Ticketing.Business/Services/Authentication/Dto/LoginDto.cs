using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
