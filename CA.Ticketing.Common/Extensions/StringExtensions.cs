using CA.Ticketing.Common.Constants;

namespace CA.Ticketing.Common.Extensions
{
    public class StringExtensions
    {
        public static IEnumerable<string> GetRoleNames()
        {
            return new string[4] { RoleNames.Admin, RoleNames.Manager, RoleNames.Customer, RoleNames.ToolPusher };
        }
    }
}
