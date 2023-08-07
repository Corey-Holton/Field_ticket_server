using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class AddEmployeeLoginDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

        [Required]
        public string TicketIdentifier { get; set; }
    }
}
