using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDto : EntityDtoBase
    {
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int NetTerm { get; set; } = 0;
    }
}
