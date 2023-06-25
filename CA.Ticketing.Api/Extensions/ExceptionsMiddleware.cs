using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Common.Models;
using System.Net;

namespace CA.Ticketing.Api.Extensions
{
    /// <summary>
    ///     Exception Middleware register extension.
    /// </summary>
    public static class ErrorLoggingMiddlewareExtensions
    {
        /// <summary>
        ///     Register Exception Middleware.
        /// </summary>
        /// <param name="builder"> Application builder </param>
        /// <returns> Application builder </returns>
        public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }

    /// <summary>
    ///     Handle unhandled application exceptions.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        ///     Initialize new instance of <see cref="ExceptionMiddleware" />.
        /// </summary>
        /// <param name="next"> Next request handler </param>
        /// <param name="logger"> Application logger </param>
        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        ///     Invoke middleware.
        /// </summary>
        /// <param name="httpContext"> HttpContext </param>
        /// <returns> Async task </returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                _logger.LogError(ex, "Application exception: {message}.", message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        ///     Make user response with custom message.
        /// </summary>
        /// <param name="context"> HttpContext </param>
        /// <param name="exception"> Unhandled exception </param>
        /// <returns> Async task </returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var errorMessage = exception.Message;

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                StatusCode = context.Response.StatusCode,
                ErrorMessage = errorMessage
            }.ToJson());
        }
    }
}
