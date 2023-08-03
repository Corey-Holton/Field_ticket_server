using CA.Ticketing.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.TicketSpecifications.Dto
{
    public class CreateTicketSpecificationDto : EntityDtoBase<int>
    {
        public int Quantity { get; set; }

        public int EquipmentChargeId { get; set; }

        public int FieldTicketId { get; set; }

    }
}
