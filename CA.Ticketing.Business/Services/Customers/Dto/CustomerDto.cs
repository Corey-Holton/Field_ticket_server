using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDto : EntityDtoBase
    {
        public string Name { get; set; }

        public string City { get; set; }

        public int NetTerm { get; set; }
    }
}
