using System.Security.Claims;

namespace CA.Ticketing.Common.Authentication
{
    public interface IContextUser
    {
        string Id { get; set; }

        IContextUser FromClaimsIdentity(ClaimsIdentity identity);
    }
}
