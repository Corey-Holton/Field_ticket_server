using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class AddCustomerLoginDto
    {
        public int customerContactId { get; set; }

        public string redirectUrl { get; set; }
    }
}
