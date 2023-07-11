using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDetailsDto : CustomerDto
    {
        public List<CustomerLocationDto> Locations { get; set; }
    }
}
