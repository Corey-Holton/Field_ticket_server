using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Sync
{
    public class ServerSyncHistoryService : BackgroundService, IServerSyncHistoryService
    {
        private readonly ILogger<ServerSyncHistoryService> _logger;

        private readonly IServiceProvider _serviceProvider;
        public ServerSyncHistoryService(IServiceProvider serviceProvider, ILogger<ServerSyncHistoryService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await SyncOldestAsync();

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await SyncOldestAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopped");
            }
        }
        public async Task SyncOldestAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CATicketingContext>();

            var oldestModified = await context.ServerSyncHistory
                .OrderBy(x => x.LastSyncDate)
                .FirstAsync();

            var syncProcessor = services.GetRequiredService<ISyncProcessor>();

            foreach (var entityType in TypeExtensions.SyncEntities)
            {
                var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.DeleteMarkedDbEntities))!
                .MakeGenericMethod(entityType);

                await (Task)methodInfo.Invoke(syncProcessor, new object[] { oldestModified })!;
            }

        }

    }
}
