using CA.Ticketing.Common.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class CreateEmployeeLoginDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string TicketIdentifier { get; set; }
    }
}
