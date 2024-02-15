using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Sync;
using CA.Ticketing.Business.Services.Sync.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CA.Ticketing.Api.Controllers
{
    [ApiController]
    public class SyncController: ControllerBase
    {
        private readonly ISyncProcessor _syncProcessor;

        private readonly IDataSyncService _dataSyncService;

        private readonly ISyncInfoService _syncInfoService;

        public SyncController(ISyncProcessor syncProcessor, ISyncInfoService syncInfoService, IDataSyncService? dataSyncService = null)
        {
            _syncProcessor = syncProcessor;
            if (dataSyncService != null )
            {
                _dataSyncService = dataSyncService;
            }
            _syncInfoService = syncInfoService;
        }

        [HttpGet]
        [Route(ApiRoutes.Sync.Health)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Sync.Status)]
        [ProducesResponseType(typeof(ServerStatus), StatusCodes.Status200OK)]
        public IActionResult Status()
        {
            var result = _dataSyncService.GetServerStatus();
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Sync.Run)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Run()
        {
            await _dataSyncService.RunSync();
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Sync.GetOrUpdateGeneric)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string entityType, string dateTimeLastModified)
        {
            var lastModifiedDate = DateTime.ParseExact(dateTimeLastModified, "yyyyMMddHHmmssfffffff", null);
            var typeRequested = TypeExtensions.GetTypeFromString(entityType);

            var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.GetEntities))!
                .MakeGenericMethod(typeRequested);
            var (Entities, LastModifiedDate) = await (Task<(IEnumerable<object> Entities,DateTime LastModifiedDate)>)methodInfo.Invoke(_syncProcessor, new object[] { lastModifiedDate, false })!;
            return Ok(Entities);
        }

        [HttpPost]
        [Route(ApiRoutes.Sync.GetOrUpdateGeneric)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(string entityType, [FromBody] IEnumerable<object> entities)
        {
            var typeRequested = TypeExtensions.GetTypeFromString(entityType);

            var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.UpdateEntities))!
                .MakeGenericMethod(typeRequested);

            await (Task<DateTime?>)methodInfo.Invoke(_syncProcessor, new object[] { entities, false })!;

            return Ok();
        }

        [HttpPost]
        [Route(ApiRoutes.Sync.History)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSyncHistory(string id, string dateTimeLastModified)
        {

            await _syncInfoService.UpdateSync(id, dateTimeLastModified);
            return Ok();

        }
    }
}
