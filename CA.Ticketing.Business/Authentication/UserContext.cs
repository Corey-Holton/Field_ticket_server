using CA.Ticketing.Common.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CA.Ticketing.Business.Authentication
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IContextUser? User
        {
            get
            {
                if (_contextAccessor == null || _contextAccessor.HttpContext == null)
                {
                    return null;
                }

                var User = _contextAccessor.HttpContext.User;
                if (User.Identity == null || !User.Identity.IsAuthenticated)
                {
                    return null;
                }

                var Identity = (ClaimsIdentity)User.Identity;
                if (Identity != null)
                {
                    var AppUser = new ContextUser().FromClaimsIdentity(Identity);
                    return AppUser;
                }

                return null;
            }
        }
    }
}
