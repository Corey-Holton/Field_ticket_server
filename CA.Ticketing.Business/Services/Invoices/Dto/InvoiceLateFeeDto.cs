using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Constants;

namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class InvoiceLateFeeDto : EntityDtoBase
    {
        public double PercentageFeeApplied { get; set; } = BusinessConstants.InvoiceLateFee;

        public DateTime AppliedOn { get; set; }

        public bool SentToCustomer { get; set; }
    }
}
