using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using System.Security.Claims;

namespace CA.Ticketing.Business.Authentication
{
    public class ContextUser : IContextUser
    {
        public string Id { get; set; }

        public string TicketIdentifier { get; set; }

        public ApplicationRole Role { get; set; }

        public string? CustomerContactId { get; set; }

        public IContextUser FromClaimsIdentity(ClaimsIdentity identity)
        {
            if (string.IsNullOrEmpty(identity.Name))
            {
                throw new ApplicationException("Impossible to retrieve user id from context user.");
            }

            Id = identity.Name;

            var ticketIdentifierClaim = identity.Claims.FirstOrDefault(x => x.Type == CAClaims.TicketIdentifier);
            if (ticketIdentifierClaim != null)
            {
                TicketIdentifier = ticketIdentifierClaim.Value;
            }

            var roleClaim = identity.FindFirst(ClaimTypes.Role);

            if (roleClaim != null)
            {
                if (Enum.TryParse(typeof(ApplicationRole), roleClaim.Value, out var role))
                {
                    Role = (ApplicationRole)role!;
                }
            }

            var customerContactClaim = identity.FindFirst(CAClaims.CustomerContactId);

            if (customerContactClaim != null && !string.IsNullOrEmpty(customerContactClaim.Value))
            {
                CustomerContactId = customerContactClaim.Value;
            }

            return this;
        }
    }
}
