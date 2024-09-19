using CA.Ticketing.Persistance.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Models
{
    public class TicketType : IdentityModel
    {
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Charge> IncludedCharges { get; set; } = new List<Charge>();

        [JsonIgnore]
        public virtual ICollection<Charge> SpecialCharges { get; set; } = new List<Charge>();
    }
}
