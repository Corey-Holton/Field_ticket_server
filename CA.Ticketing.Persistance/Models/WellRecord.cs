using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using CA.Ticketing.Common.Enums;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.WellRecords)]
    public class WellRecord : IdentityModel
    {
        [ForeignKey(nameof(FieldTicket))]
        public string FieldTicketId { get; set; }

        [JsonIgnore]
        public virtual FieldTicket FieldTicket { get; set; }

        public WellRecordType WellRecordType { get; set; }

        public string Pump_Number { get; set; } = string.Empty;

        public string Pulled { get; set; } = string.Empty;

        public string Ran { get; set; } = string.Empty;

        public string SizeW { get; set; } = string.Empty;

        public string SizeL { get; set; } = string.Empty;

        public string SizeH { get; set; } = string.Empty;

        [NotMapped]
        public string Size
        {
            get
            {
                string returnSize = SizeW ?? "";
                if (SizeL != string.Empty)
                {
                    returnSize = returnSize + " x " + SizeL;
                }
                if (SizeH != string.Empty)
                {
                    returnSize = returnSize + " x " + SizeH;
                }
                return returnSize;
            }
        }
    }
}
