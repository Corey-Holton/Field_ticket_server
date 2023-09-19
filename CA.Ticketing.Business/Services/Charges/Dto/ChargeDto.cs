using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Charges.Dto
{
    public class ChargeDto : EntityDtoBase
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public UnitOfMeasure UoM { get; set; }

        public double DefaultRate { get; set; }

        public bool IsRigSpecific { get; set; }

        public bool IncludeInTicketSpecs { get; set; }

        public bool AllowUoMChange { get; set; }

        public bool AllowRateAdjustment { get; set; }
    }
}
