using CA.Ticketing.Common.Constants;
using CA.Ticketing.Desktop.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CA.Ticketing.Desktop.Services
{
    public class ServerConnector
    {
        private readonly HttpClient _httpClient;

        public ServerConnector()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BusinessConstants.LocalServer.ApiBaseUrl)
            };
        }

        public async Task<bool> IsServerAvailable()
        {
            var healthCheckResponse = await _httpClient.GetAsync(ApiRoutes.Sync.Health);
            return healthCheckResponse.IsSuccessStatusCode;
        }

        public async Task<ServerStatus?> GetServerStatus()
        {
            var serverStatusResponse = await _httpClient.GetAsync(ApiRoutes.Sync.Status);
            serverStatusResponse.EnsureSuccessStatusCode();
            var responseString = await serverStatusResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServerStatus>(responseString);
        }

        public async Task RunSync()
        {
            var serverSyncResponse = await _httpClient.PostAsync(ApiRoutes.Sync.Run, null);
            serverSyncResponse.EnsureSuccessStatusCode();
        }
    }
}
