using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class ResetCustomerContactPasswordDto
    {
        [Required]
        public int Id { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
