using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerLoginDto : BaseRedirectUrlRequest
    {
        public int CustomerContactId { get; set; }
    }
}
