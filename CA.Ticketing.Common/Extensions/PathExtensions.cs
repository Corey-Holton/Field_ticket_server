namespace CA.Ticketing.Common.Extensions
{
    public static class PathExtensions
    {
        public static string GetXmlCommentPath(string assemblyName) => Path.Combine(AppContext.BaseDirectory, assemblyName + ".xml");
    }
}
