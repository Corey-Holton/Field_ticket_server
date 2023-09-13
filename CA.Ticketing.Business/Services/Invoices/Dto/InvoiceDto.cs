using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class InvoiceDto : EntityDtoBase<int>
    {
        public string InvoiceId { get; set; }

        public string Customer { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool SentToCustomer { get; set; }

        public bool Paid { get; set; }

        public double Total { get; set; }

        public List<TicketInfoDto> Tickets { get; set; }
    }
}
