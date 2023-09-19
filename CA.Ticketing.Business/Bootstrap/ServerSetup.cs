using CA.Ticketing.Business.Services.Sync;
using CA.Ticketing.Common.Setup;
using CA.Ticketing.Persistance.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class ServerSetup
    {
        public static void RegisterServer(this IServiceCollection services, IConfiguration configuration)
        {
            var securitySettings = configuration
                .GetSection(nameof(ServerConfiguration)).Get<ServerConfiguration>();

            if (!securitySettings.IsMainServer)
            {
                services.AddSingleton<DataSyncService>();
                services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<DataSyncService>());
            }
        }
        public static async Task<IApplicationBuilder> InitiateDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var seeder = services.GetRequiredService<DatabaseInitializer>();
            await seeder.InitializeAsync();
            return app;
        }
    }
}
