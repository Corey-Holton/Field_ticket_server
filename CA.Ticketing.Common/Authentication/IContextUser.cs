using CA.Ticketing.Common.Enums;
using System.Security.Claims;

namespace CA.Ticketing.Common.Authentication
{
    public interface IContextUser
    {
        string Id { get; set; }

        string TicketIdentifier { get; set; }

        ApplicationRole Role { get; set; }

        int? CustomerContactId { get; set; }

        IContextUser FromClaimsIdentity(ClaimsIdentity identity);
    }
}
