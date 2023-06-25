using CA.Ticketing.Business.Services.Base;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class ResendInviteDto : BaseRedirectUrlRequest
    {
        [Required]
        public int CustomerContactId { get; set; }
    }
}
