
using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Bootstrap;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Seed;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging Configuration
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

// Clear Logging providers and add Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add settings and other bootstrap registrations
builder.Services.RegisterSettings(builder.Configuration);

// Add CORS * for now
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.RegisterAuthentication(builder.Configuration);

builder.Services.RegisterContextHelpers();

builder.Services.RegisterIdentityCore();

builder.Services.AddControllers();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    }
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterSwaggerGenerator();

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddDbContext<CATicketingContext>(options =>
{
    var connectionString = builder.Configuration["ApplicationSettings:ConnectionString"];
    options.UseSqlServer(connectionString);
});

builder.Services.RegisterDomainServices();

builder.Services.RegisterMappers();

var app = builder.Build();

// Execute Seed methods
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DatabaseInitializer>();
    await seeder.InitializeAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CA Ticketing API");
});

app.UseErrorLogging();

app.UseCors("EnableCORS");

app.UseHttpsRedirection();

app.SetDistributionFolders(builder.Environment);

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
