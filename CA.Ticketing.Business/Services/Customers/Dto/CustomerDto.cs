using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDto : EntityDtoBase<int>
    {
        public string Name { get; set; }
        public int NetTerm { get; set; }
    }
}
