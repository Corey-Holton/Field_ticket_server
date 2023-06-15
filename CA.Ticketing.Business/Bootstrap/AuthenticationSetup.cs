using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class AuthenticationSetup
    {
        public static void RegisterAuthentication(this IServiceCollection service, IConfiguration configuration)
        {
            var securitySettings = configuration
                .GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();

            var key = Encoding.ASCII.GetBytes(securitySettings.Secret);

            service.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy => policy.RequireRole(new string[] { RoleNames.Admin }));
                options.AddPolicy(Policies.Customer, policy => policy.RequireRole(new string[] { RoleNames.Customer }));
            });

            service.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireAudience = false,
                    RequireSignedTokens = false
                };
                x.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = c =>
                    {
                        var accessToken = c.Request.Query["access_token"];

                        var path = c.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chat")))
                        {
                            c.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
