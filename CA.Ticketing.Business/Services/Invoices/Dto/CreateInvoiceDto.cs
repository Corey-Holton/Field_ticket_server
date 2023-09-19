namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class CreateInvoiceDto
    {
        public string CustomerId { get; set; }

        public string[] TicketIds { get; set; }
    }
}
