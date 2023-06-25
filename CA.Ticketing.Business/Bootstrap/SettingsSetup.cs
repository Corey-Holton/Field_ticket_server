using CA.Ticketing.Common.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class SettingsSetup
    {
        public static void RegisterSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SecuritySettings>(configuration.GetSection(nameof(SecuritySettings)));
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        }
    }
}
