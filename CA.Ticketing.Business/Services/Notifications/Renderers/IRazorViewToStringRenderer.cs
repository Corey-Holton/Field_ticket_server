namespace CA.Ticketing.Business.Services.Notifications.Renderers
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
