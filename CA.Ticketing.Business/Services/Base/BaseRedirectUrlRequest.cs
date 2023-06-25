using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Base
{
    public class BaseRedirectUrlRequest
    {
        [Required]
        public string RedirectUrl { get; set; }
    }
}
