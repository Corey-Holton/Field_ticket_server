using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class AddCustomerContactPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
