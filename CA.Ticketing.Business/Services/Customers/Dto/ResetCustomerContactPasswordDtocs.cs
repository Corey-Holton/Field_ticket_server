using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class ResetCustomerContactPasswordDto
    {
        [Required]
        public string CustomerContactId { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
