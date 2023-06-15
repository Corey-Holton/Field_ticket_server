using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Common.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteContent(this IUrlHelper url, string contentPath)
        {
            if (url?.ActionContext?.HttpContext?.Request == null)
            {
                return string.Empty;
            }

            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        public static string AbsolutePath(this IUrlHelper url, string action)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), action).ToString();
        }
    }
}
