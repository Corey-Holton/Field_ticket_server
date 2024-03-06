using CA.Ticketing.Business.Services.Invoices;
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
            var serverConfiguration = GetServerConfiguration(configuration);

            if (!serverConfiguration.IsMainServer)
            {
                services.AddSingleton<IDataSyncService, DataSyncService>();
                services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<IDataSyncService>());
                services.AddSingleton<SyncChildDeletionService>();
                services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<SyncChildDeletionService>());
                return;
            }

            services.AddSingleton<InvoiceLateFeeService>();
            services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<InvoiceLateFeeService>());;
            services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<ServerSyncHistoryService>());
        }

        public static async Task<IApplicationBuilder> InitiateDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            var serverConfiguration = GetServerConfiguration(configuration);

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var seeder = services.GetRequiredService<DatabaseInitializer>();
            await seeder.InitializeAsync(serverConfiguration.IsMainServer);

            return app;
        }

        private static ServerConfiguration GetServerConfiguration(IConfiguration configuration)
        {
            return configuration
                .GetSection(nameof(ServerConfiguration)).Get<ServerConfiguration>();
        }
    }
}
