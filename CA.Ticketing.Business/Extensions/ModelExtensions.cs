using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models;
using System.Reflection.Metadata.Ecma335;

namespace CA.Ticketing.Business.Extensions
{
    public static class ModelExtensions
    {
        public static bool IsServiceType(this FieldTicket ticket, ServiceType serviceType)
        {
            if (ticket.ServiceTypesSelection.Contains(","))
            {
                return false;
            }

            return ticket.ServiceTypes.First().Equals(serviceType);
        }

        public static bool IsTaxable(this FieldTicket ticket) =>
            !ticket.IsServiceType(ServiceType.PAndA) && !ticket.IsServiceType(ServiceType.Yard);

        public static bool IsRigWork(this FieldTicket ticket) => ticket.ServiceTypes.Any(x => x < ServiceType.Yard);

        public static bool IsNotRigWork(this FieldTicket ticket) => ticket.ServiceTypes.Any(x => x >= ServiceType.Yard);

        public static bool IsNotRigWork(this ManageTicketDto ticket) => ticket.ServiceTypes.Any(x => x >= ServiceType.Yard);

        public static bool IsRigWork(this ManageTicketDto ticket) => ticket.ServiceTypes.Any(x => x < ServiceType.Yard);
    }
}
