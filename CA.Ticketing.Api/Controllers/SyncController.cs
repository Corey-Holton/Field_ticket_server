using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Sync;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [ApiController]
    public class SyncController: ControllerBase
    {
        private readonly ISyncProcessor _syncProcessor;

        private readonly IDataSyncService _dataSyncService;

        public SyncController(ISyncProcessor syncProcessor, IDataSyncService? dataSyncService = null)
        {
            _syncProcessor = syncProcessor;
            if (dataSyncService != null )
            {
                _dataSyncService = dataSyncService;
            }
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Status()
        {
            var result = _dataSyncService.GetServerStatus();
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Sync.GetOrUpdateGeneric)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string entityType, string dateTimeLastModified)
        {
            var lastModifiedDate = DateTime.ParseExact(dateTimeLastModified, "yyyyMMddHHmmssffff", null);
            var typeRequested = TypeExtensions.GetTypeFromString(entityType);

            var methodInfo = typeof(SyncProcessor).GetMethod(nameof(SyncProcessor.GetEntities))!
                .MakeGenericMethod(typeRequested);
            var (Entities, LastModifiedDate) = await (Task<(IEnumerable<object> Entities,DateTime LastModifiedDate)>)methodInfo.Invoke(this, new object[] { lastModifiedDate, false })!;
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

            await (Task<DateTime?>)methodInfo.Invoke(this, new object[] { entities, false })!;

            return Ok();
        }
    }
}
