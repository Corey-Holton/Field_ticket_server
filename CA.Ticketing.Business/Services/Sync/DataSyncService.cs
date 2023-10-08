using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Sync.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Models;
using CA.Ticketing.Common.Setup;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Timers;

namespace CA.Ticketing.Business.Services.Sync
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly System.Timers.Timer _syncTimer;

        private readonly System.Timers.Timer _serverCheckTimer;

        private readonly double _syncInterval = TimeSpan.FromMinutes(5).TotalMilliseconds;

        private readonly double _serverCheckInterval = TimeSpan.FromSeconds(15).TotalMilliseconds;

        private bool _syncInProgress = false;

        private readonly HttpClient _httpClient;

        private readonly ILogger<DataSyncService> _logger;

        private TaskCompletionSource<bool> _syncTaskCompletionSource;

        private Task<bool> _syncTaskCompleted;

        private readonly ServerStatus _serverStatus = new();

        public DataSyncService(IServiceProvider serviceProvider,
            IOptions<ServerConfiguration> serverConfigurationOptions,
            ILogger<DataSyncService> logger)
        {
            _serviceProvider = serviceProvider;

            _syncTimer = new(_syncInterval)
            {
                AutoReset = false
            };
            _syncTimer.Elapsed += SyncProcess;

            _serverCheckTimer = new(_serverCheckInterval)
            {
                AutoReset = false
            };
            _serverCheckTimer.Elapsed += CheckServerStatus;

            _httpClient = new()
            {
                BaseAddress = new Uri(serverConfigurationOptions.Value.MainServerUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("ClientId", serverConfigurationOptions.Value.ClientId);

            _logger = logger;

            SetTask(true);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SetServerStatus();
                await CheckServerHealth();
                _ = Task.Run(() => SyncProcess(null, null));
            }
            catch
            {
                _serverStatus.IsOnline = false;
            }
            finally
            {
                _serverCheckTimer.Start();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_serverStatus.SyncInProgress)
            {
                await _syncTaskCompleted;
            }

            if (_syncTimer != null)
            {
                _syncTimer.Stop();
                _syncTimer.Dispose();
            }

            if (_serverCheckTimer != null)
            {
                _serverCheckTimer.Stop();
                _serverCheckTimer.Dispose();
            }
        }

        public ServerStatus GetServerStatus() => _serverStatus;

        public async Task RunSync()
        {
            if (!_serverStatus.IsOnline)
            {
                return;
            }

            if (_serverStatus.SyncInProgress)
            {
                await _syncTaskCompleted;
                return;
            }

            _syncTimer.Stop();
            SyncProcess(null, null);
        }

        private async Task SetServerStatus()
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CATicketingContext>();

            var syncData = await context.SyncData.FirstOrDefaultAsync();

            if (syncData == null)
            {
                var syncDataTypeInfoList = TypeExtensions.SyncEntities
                    .Select(x => new SyncDataTypeInfo(x))
                    .ToList();
                syncData = new SyncData { Changes = JsonConvert.SerializeObject(syncDataTypeInfoList) };

                context.SyncData.Add(syncData);
                await context.SaveChangesAsync();
            }

            _serverStatus.LastSyncDate = syncData.LastSyncDate;
        }

        private void SyncProcess(object? sender, ElapsedEventArgs? e)
        {
            if (!_serverStatus.IsOnline || _serverStatus.SyncInProgress)
            {
                _syncTimer.Start();
                return;
            }
            try
            {
                _serverStatus.SyncInProgress = true;
                SetTask();
                var processTask = ExecuteSyncProcess();
                processTask.Wait();
                if (_serverStatus.InitialSyncInProgress)
                {
                    _serverStatus.InitialSyncInProgress = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error during sync process");
            }
            finally
            {
                _serverStatus.NextSyncOn = DateTime.UtcNow.AddMilliseconds(_syncInterval);
                _syncTimer.Start();
                _serverStatus.SyncInProgress = false;
                _syncTaskCompletionSource.SetResult(true);
            }
        }

        private async Task ExecuteSyncProcess()
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CATicketingContext>();

            var syncData = await context.SyncData.FirstAsync();

            var syncDataChanges = JsonConvert.DeserializeObject<IEnumerable<SyncDataTypeInfo>>(syncData.Changes)!
                .ToDictionary(x => TypeExtensions.GetTypeFromString(x.EntityType), x => x);

            var syncProcessor = services.GetRequiredService<ISyncProcessor>();

            foreach (var entityType in TypeExtensions.SyncEntities)
            {
                if (!syncDataChanges.ContainsKey(entityType))
                {
                    syncDataChanges.Add(entityType, new SyncDataTypeInfo(entityType));
                }

                await SendEntities(entityType, syncDataChanges, syncProcessor);

                await GetEntities(entityType, syncDataChanges, syncProcessor);
            }

            syncData.Changes = JsonConvert.SerializeObject(syncDataChanges.Select(x => x.Value));
            syncData.LastSyncDate = DateTime.UtcNow;

            _serverStatus.LastSyncDate = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }

        private async Task SendEntities(Type entityType, Dictionary<Type, SyncDataTypeInfo> syncDataChanges, ISyncProcessor syncProcessor)
        {
            if (entityType == typeof(IdentityRole) || entityType == typeof(IdentityUserRole<string>))
            {
                return;
            }

            var syncData = syncDataChanges[entityType];
            var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.GetEntities))!
                .MakeGenericMethod(entityType);

            var (entities, lastModifiedDate) = await (Task<(IEnumerable<object> Entities, DateTime LastModifiedDate)>)methodInfo.Invoke(syncProcessor, new object[] { syncData.PostLastModifiedDate, true })!;

            syncData.PostLastModifiedDate = lastModifiedDate;

            using StringContent requestBody = new(JsonConvert.SerializeObject(entities), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync($"api/sync/{entityType.Name.ToLower()}", requestBody);

            response.EnsureSuccessStatusCode();
        }

        private async Task GetEntities(Type entityType, Dictionary<Type, SyncDataTypeInfo> syncDataChanges, ISyncProcessor syncProcessor)
        {
            var syncData = syncDataChanges[entityType];

            using var serverDataResponse = await _httpClient.GetAsync($"api/sync/{entityType.Name.ToLower()}?dateTimeLastModified={syncData.GetLastModifiedDate:yyyyMMddHHmmssfffffff}");
            serverDataResponse.EnsureSuccessStatusCode();
            var entitiesToUpdateString = await serverDataResponse.Content.ReadAsStringAsync();
            var entitiesToUpdate = JsonConvert.DeserializeObject(entitiesToUpdateString);

            var updateMethodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.UpdateEntities))!
                .MakeGenericMethod(entityType);

            var lastUpdated = await (Task<DateTime?>)updateMethodInfo.Invoke(syncProcessor, new object[] { entitiesToUpdate!, true })!;

            if (lastUpdated != null)
            {
                syncData.GetLastModifiedDate = lastUpdated.Value;
            }
        }

        private void CheckServerStatus(object? sender, ElapsedEventArgs e)
        {
            if (_serverStatus.SyncInProgress)
            {
                _serverCheckTimer.Start();
                return;
            }

            try
            {
                var checkServerTask = CheckServerHealth();
                checkServerTask.Wait();
            }
            catch
            {
                _serverStatus.IsOnline = false;
            }
            finally
            {
                _serverCheckTimer.Start();
            }
        }

        private async Task CheckServerHealth()
        {
            using var serverDataResponse = await _httpClient.GetAsync(ApiRoutes.Sync.Health);
            serverDataResponse.EnsureSuccessStatusCode();
            _serverStatus.IsOnline = true;
        }

        private void SetTask(bool isCompleted = false)
        {
            _syncTaskCompletionSource = new(isCompleted);
            _syncTaskCompleted = _syncTaskCompletionSource.Task;
        }
    }
}
