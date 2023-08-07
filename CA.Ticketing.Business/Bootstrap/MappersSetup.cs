using AutoMapper;
using CA.Ticketing.Business.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Ticketing.Business.Bootstrap
{
    public static class MappersSetup
    {
        public static void RegisterMappers(this IServiceCollection services)
        {
            services.AddScoped(provider => new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new EmployeesMapper());
                mapperConfiguration.AddProfile(new AuthenticationMapper());
                mapperConfiguration.AddProfile(new CustomersMapper());
                mapperConfiguration.AddProfile(new ChargesMapper());
                mapperConfiguration.AddProfile(new EquipmentMapper());
                mapperConfiguration.AddProfile(new SchedulingMapper());
                mapperConfiguration.AddProfile(new TicketsMapper());
                mapperConfiguration.AddProfile(new InvoiceMapper());
                mapperConfiguration.AddProfile(new SettingsMapper());
            })
            .CreateMapper());
        }
    }
}
