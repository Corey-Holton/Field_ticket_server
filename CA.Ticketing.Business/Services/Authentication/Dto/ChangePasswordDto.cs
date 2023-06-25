using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class ChangePasswordDto
    {
        [PasswordPropertyText]
        [Required]
        public string CurrentPassword { get; set; }

        [PasswordPropertyText]
        [Required]
        public string NewPassword { get; set; }
    }
}
