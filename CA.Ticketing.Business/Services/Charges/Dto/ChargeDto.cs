using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Charges.Dto
{
    public class ChargeDto : EntityDtoBase<int>
    {
        public string Name { get; set; }

        public string ChargeType { get; set; }
    }
}
