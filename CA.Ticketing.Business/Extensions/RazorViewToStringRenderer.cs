using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace CA.Ticketing.Business.Extensions
{
    public class RazorViewToStringRenderer : IRazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpContext? _context;

        public RazorViewToStringRenderer(
          IRazorViewEngine viewEngine,
          ITempDataProvider tempDataProvider,
          IServiceProvider serviceProvider,
          IHttpContextAccessor accessor)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _context = accessor.HttpContext;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var view = FindView(actionContext, viewName);

            using var output = new StringWriter();
            var viewContext = new ViewContext(
              actionContext,
              view,
              new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
              {
                  Model = model
              },
              new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
              output,
              new HtmlHelperOptions())
            {
                RouteData = _context?.GetRouteData() ?? new RouteData()
            };
            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
              Environment.NewLine,
              new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }
    }
}
