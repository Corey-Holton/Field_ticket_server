using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace CA.Ticketing.Business.Extensions
{
    public static class AuthenticationExtensions
    {
        public static async Task<ApplicationUser?> FindByCustomerContactIdAsync(this UserManager<ApplicationUser> userManager, string customerContactId)
        {
            return await userManager.Users.FirstOrDefaultAsync(x => x.CustomerContactId == customerContactId);
        }

        public static TokenValidationParameters GetTokenValidationParameters(bool validateLifetime, byte[] key, string issuer)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = false,
                RequireAudience = false,
                RequireSignedTokens = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidateLifetime = validateLifetime
            };
        }
    }
}
