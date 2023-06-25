using CA.Ticketing.Business.Services.Base;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class ResetPasswordDto : BaseRedirectUrlRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
