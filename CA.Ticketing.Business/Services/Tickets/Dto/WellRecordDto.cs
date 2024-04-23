using CA.Ticketing.Business.Services.Base;
using Newtonsoft.Json;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;

namespace CA.Ticketing.Business.Services.Tickets.Dto
{
    public class WellRecordDto : EntityDtoBase
    {
        public string FieldTicketId { get; set; }

        public WellRecordType WellRecordType { get; set; }

        public string RecordType => WellRecordType.GetWellRecordType();

        public int Amount => WellRecordType.GetWellRecordAmount();

        public string? Pump_Number { get; set; }

        public string? Pulled { get; set; }

        public string? Ran { get; set; }

        public string? SizeW { get; set; }

        public string? SizeL { get; set; }

        public string? SizeH { get; set; }

    }
}
