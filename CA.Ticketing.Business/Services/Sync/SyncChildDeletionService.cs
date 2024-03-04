using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace CA.Ticketing.Business.Services.Sync
{
    public class SyncChildDeletionService : BackgroundService
    {
        private readonly ILogger<SyncChildDeletionService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SyncChildDeletionService(IServiceProvider serviceProvider, ILogger<SyncChildDeletionService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromDays(1));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await SyncDataCleanup();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopped");
            }
        }
        public async Task SyncDataCleanup()
        {
            using var scope = _serviceProvider.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CATicketingContext>();

            var syncProcessor = services.GetRequiredService<ISyncProcessor>();

            foreach (var entityType in TypeExtensions.SyncHistory)
            {
                var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.DeleteMarkedDbEntities))!
                    .MakeGenericMethod(entityType);
                try
                {

                    await (Task)methodInfo.Invoke(syncProcessor, new object[] { DateTime.UtcNow.AddDays(-1) })!;
                }
                catch (DbUpdateException exc)
                {
                    _logger.LogError(exc, $"Db update error: {exc?.InnerException?.Message}");
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, $"Error: {exc?.InnerException?.Message}");
                }
            }
        }
    }
}
