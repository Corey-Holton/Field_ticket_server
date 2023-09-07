using CA.Ticketing.Business.Authentication;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Charges;
using CA.Ticketing.Business.Services.Customers;
using CA.Ticketing.Business.Services.Employees;
using CA.Ticketing.Business.Services.Equipments;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Business.Services.Invoices;
using CA.Ticketing.Business.Services.Notifications;
using CA.Ticketing.Business.Services.Payroll;
using CA.Ticketing.Business.Services.Pdf;
using CA.Ticketing.Business.Services.Scheduling;
using CA.Ticketing.Business.Services.Settings;
using CA.Ticketing.Business.Services.Tickets;
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
            services.AddScoped<IChargesService, ChargesService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<ISchedulingService, SchedulingService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
            services.AddScoped<IPayrollService, PayrollService>();
            services.AddScoped<IFileManagerService, FileManagerService>();
        }

        private static void RegisterBaseServices(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
        }
    }
}
