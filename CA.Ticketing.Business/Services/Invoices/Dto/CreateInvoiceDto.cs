namespace CA.Ticketing.Business.Services.Invoices.Dto
{
    public class CreateInvoiceDto
    {
        public int CustomerId { get; set; }

        public int[] TicketIds { get; set; }
    }
}
