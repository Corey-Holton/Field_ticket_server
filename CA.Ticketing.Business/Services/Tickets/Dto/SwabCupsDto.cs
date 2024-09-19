using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class SwabCupsDto : EntityDtoBase
    {
        public string FieldTicketId { get; set; }

        public double Number { get; set; }

        public double Size { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }
    }
}
