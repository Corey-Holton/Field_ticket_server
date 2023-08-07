using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using System.Security.Claims;

namespace CA.Ticketing.Business.Authentication
{
    public class ContextUser : IContextUser
    {
        public string Id { get; set; }

        public string TicketIdentifier { get; set; }

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

            return this;
        }
    }
}
