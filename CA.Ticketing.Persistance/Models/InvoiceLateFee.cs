using CA.Ticketing.Persistance.Models.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    public class InvoiceLateFee : IdentityModel
    {
        [ForeignKey(nameof(Invoice))]
        public string InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }

        public bool SentToCustomer { get; set; }
    }
}
