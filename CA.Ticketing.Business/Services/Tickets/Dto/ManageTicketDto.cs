using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Common.Enums;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class ManageTicketDto : EntityDtoBase
    {
        public ServiceType? ServiceType { get; set; }

        [JsonIgnore]
        public string ServiceTypesOrdered
        {
            get
            {
                return string.Join(",", ServiceTypes.OrderBy(x => (int)x));
            }
        }

        public ServiceType[] ServiceTypes { get; set; } = Array.Empty<ServiceType>();

        public string TicketId { get; set; }

        public string EquipmentId { get; set; }

        public string? CustomerId { get; set; }

        public string? CustomerLocationId { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string? SendEmailTo { get; set; }
    }
}
