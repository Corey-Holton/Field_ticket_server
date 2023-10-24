using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.Settings)]
    public class Setting : IdentityModel
    {
        public double TaxRate { get; set; }

        public double OvertimePercentageIncrease { get; set; }

        public double MileageCost { get; set; }
    }
}
