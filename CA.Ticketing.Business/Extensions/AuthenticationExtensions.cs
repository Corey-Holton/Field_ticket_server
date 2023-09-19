using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Extensions
{
    public static class AuthenticationExtensions
    {
        public static async Task<ApplicationUser?> FindByCustomerContactIdAsync(this UserManager<ApplicationUser> userManager, string customerContactId)
        {
            return await userManager.Users.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId);
        }
    }
}
