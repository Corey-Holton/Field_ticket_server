using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class UpdateUserDto : EntityDtoBase
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public ApplicationRole Role { get; set; }

        public string TicketIdentifier { get; set; }
    }
}
