using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.SwabCups)]
    public class SwabCups : IdentityModel
    {
        [ForeignKey(nameof(FieldTicket))]
        public string FieldTicketId { get; set; }

        [JsonIgnore]
        public virtual FieldTicket FieldTicket { get; set; }

        public double Number {  get; set; }

        public double Size { get; set; }

        public string Description {  get; set; }

        public double Amount { get; set; }
    }
}
