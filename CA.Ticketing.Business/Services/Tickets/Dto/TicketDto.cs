using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDto : EntityDtoBase<int>
    {
        public string TicketId { get; set; }

        public string CustomerName { get; set; }

        public string LocationName { get; set; }
        
        public DateTime ExecutionDate { get; set; }

        public string ServiceType { get; set; }

        public bool IsInvoiced { get; set; }

        public double Total { get; set; }

        public bool HasCustomerSignature { get; set; }
    }
}
