using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketSpecificationDto : EntityDtoBase
    {
        public string Charge { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public double Quantity { get; set; }

        public double Rate { get; set; }

        public double Total { get; set; }

        public bool AllowUoMChange { get; set; }

        public bool AllowRateAdjustment { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
