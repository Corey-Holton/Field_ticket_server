using CA.Ticketing.Business.Authentication;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Customers;
using CA.Ticketing.Business.Services.Employees;
using CA.Ticketing.Business.Services.Equipments;
using CA.Ticketing.Business.Services.Notifications;
using CA.Ticketing.Business.Services.Notifications.Renderers;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Persistance.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class ServicesSetup
    {
        public static void RegisterDomainServices(this IServiceCollection services)
        {
            services.RegisterBaseServices();

            services.AddSingleton<MessagesComposer>();

            services.AddTransient<DatabaseInitializer>();

            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAccountsService, AccountsService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
        }

        private static void RegisterBaseServices(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
        }
    }
}
