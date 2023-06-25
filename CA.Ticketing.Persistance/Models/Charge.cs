using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Charges)]
    public class Charge : IdentityModel<int>
    {
        public string Name { get; set; }

        public ChargeType ChargeType { get; set; }
    }
}
