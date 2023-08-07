using CA.Ticketing.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class CreateTicketDto
    {
        public int RigId { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
