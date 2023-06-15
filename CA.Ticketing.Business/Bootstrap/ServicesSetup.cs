using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Business.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class ServicesSetup
    {
        public static void RegisterDomainServices(this IServiceCollection services)
        {

        }

        public static void RegisterBaseServices(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
        }
    }
}
