using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Employees.Dto
{
    public class ResetEmployeePasswordDto
    {
        [Required]
        public string Id { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
