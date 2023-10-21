
using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Bootstrap;
using CA.Ticketing.Persistance.Context;
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
        .AllowAnyMethod()
        .WithExposedHeaders("Content-Disposition");
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
        options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    }
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterSwaggerGenerator();

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddDbContext<CATicketingContext>(options =>
{
    var connectionString = builder.Configuration["ApplicationSettings:ConnectionString"];
    options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services.RegisterDomainServices();

builder.Services.RegisterServer(builder.Configuration);

builder.Services.RegisterMappers();

var app = builder.Build();

await app.InitiateDatabase(builder.Configuration);

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
