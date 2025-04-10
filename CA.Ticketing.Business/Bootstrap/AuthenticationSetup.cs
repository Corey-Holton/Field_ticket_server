﻿using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class AuthenticationSetup
    {
        public static void RegisterAuthentication(this IServiceCollection service, IConfiguration configuration)
        {
            var securitySettings = configuration
                .GetSection(nameof(SecuritySettings)).Get<SecuritySettings>();

            var key = Encoding.ASCII.GetBytes(securitySettings.Secret);

            service.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(new string[] { RoleNames.Admin, RoleNames.Manager, RoleNames.ToolPusher }));
                options.AddPolicy(Policies.ApplicationManagers, policy => policy.RequireRole(new string[] { RoleNames.Admin, RoleNames.Manager, RoleNames.ToolPusher, RoleNames.Customer }));
                options.AddPolicy(Policies.Limited, policy => policy.RequireRole(new string[] { RoleNames.Admin, RoleNames.Customer, RoleNames.ToolPusher, RoleNames.Customer }));
                options.AddPolicy(Policies.CompanyUsers, policy => policy.RequireRole(new string[] { RoleNames.Admin, RoleNames.Manager, RoleNames.ToolPusher, RoleNames.Customer }));
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
                x.TokenValidationParameters = AuthenticationExtensions.GetTokenValidationParameters(true, key, securitySettings.Issuer);
                x.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = c =>
                    {
                        var accessToken = c.Request.Query["access_token"];
                        var path = c.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chat"))
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
