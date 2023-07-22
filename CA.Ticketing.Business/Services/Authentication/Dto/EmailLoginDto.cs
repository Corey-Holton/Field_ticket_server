using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class EmailLoginDto
    {
        [Required]
        public string Email { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
