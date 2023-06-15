using CA.Ticketing.Common.Authentication;
using System.Security.Claims;

namespace CA.Ticketing.Business.Authentication
{
    public class ContextUser : IContextUser
    {
        public string Id { get; set; }

        public IContextUser FromClaimsIdentity(ClaimsIdentity identity)
        {
            if (string.IsNullOrEmpty(identity.Name))
            {
                throw new ApplicationException("Impossible to retrieve user id from context user.");
            }

            Id = identity.Name;

            return this;
        }
    }
}
