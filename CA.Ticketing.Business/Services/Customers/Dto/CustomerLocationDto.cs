using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerLocationDto : EntityDtoBase<int>
    {
        public int CustomerId { get; set; }

        public string DisplayName { get; set; }

        public string Lease { get; set; }

        public string Field { get; set; }

        public string Well { get; set; }

        public string County { get; set; }

        public WellType WellType { get; set; }

        public double? Lattitude { get; set; }

        public double? Longitude { get; set; }
    }
}
