using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerLoginDto : BaseRedirectUrlRequest
    {
        public string CustomerContactId { get; set; }
    }
}
