using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Buffers;

namespace CA.Ticketing.Api.Extensions
{
    public class UtcTimeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var jsonSerialierSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                jsonSerialierSettings.Converters.Add(new StringEnumConverter());
                result.Formatters.Add(new NewtonsoftJsonOutputFormatter(
                jsonSerialierSettings,
                context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>(),
                context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>().Value,
                context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value));
            }
        }
    }
}
