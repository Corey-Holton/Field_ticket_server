using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class TicketDto : EntityDtoBase
    {
        public string TicketId { get; set; }

        public string? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string LocationName { get; set; }
        
        public DateTime ExecutionDate { get; set; }

        public ServiceType ServiceType { get; set; }

        public bool IsInvoiced { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public InvoiceIdentifierDto? Invoice { get; set; }

        public double Total { get; set; }

        public bool HasCustomerSignature { get; set; }
    }
}
