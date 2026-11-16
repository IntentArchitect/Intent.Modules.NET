using EntityFrameworkCore.SqlServer.EF11.Api.Configuration;
using EntityFrameworkCore.SqlServer.EF11.Api.Filters;
using EntityFrameworkCore.SqlServer.EF11.Api.Logging;
using EntityFrameworkCore.SqlServer.EF11.Application;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.IO.Converters;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy())
                    .Destructure.With(new GeoDestructureSerilogPolicy()));

                builder.Services.AddControllers(
                    opt =>
                    {
                        opt.Filters.Add<ExceptionFilter>();
                    })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
                });
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureProblemDetails();
                builder.Services.AddInfrastructure(builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging();
                app.UseExceptionHandler();
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                logger.Write(LogEventLevel.Information, "Starting web host");

                app.Run();
            }
            catch (HostAbortedException)
            {
                // Excluding HostAbortedException from being logged, as this is an expected
                // exception when working with EF Core migrations (as per the .NET team on the below link)
                // https://github.com/dotnet/efcore/issues/29809#issuecomment-1344101370
            }
            catch (Exception ex)
            {
                logger.Write(LogEventLevel.Fatal, ex, "Unhandled exception");
            }
        }
    }
}