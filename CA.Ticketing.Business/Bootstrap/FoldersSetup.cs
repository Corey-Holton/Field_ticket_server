using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class FoldersSetup
    {
        public static void SetDistributionFolders(this IApplicationBuilder builder, IWebHostEnvironment env)
        {
            var documentsRootPath = Path.Combine(env.ContentRootPath, FilePaths.Tickets);
            if (!Directory.Exists(documentsRootPath))
            {
                Directory.CreateDirectory(documentsRootPath);
            }

            builder.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(documentsRootPath),
                RequestPath = new PathString($"/{FilePaths.Tickets}")
            });
        }
    }
}
