using Intent.RoslynWeaver.Attributes;
using Serilog;
using Serilog.Events;
using Wolverine;
using WolverineEventing.Transport.AmazonSqs.Api.Configuration;
using WolverineEventing.Transport.AmazonSqs.Api.Filters;
using WolverineEventing.Transport.AmazonSqs.Api.Logging;
using WolverineEventing.Transport.AmazonSqs.Application;
using WolverineEventing.Transport.AmazonSqs.Infrastructure;
using WolverineEventing.Transport.AmazonSqs.Infrastructure.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Api
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

                builder.Host.UseWolverine(opts =>
                {
                    WolverineConfiguration.Configure(opts, builder.Configuration);
                });

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()));

                // Add services to the container.
                builder.Services.AddControllers(
                    opt =>
                    {
                        opt.Filters.Add<ExceptionFilter>();
                    });
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureHealthChecks(builder.Configuration);
                builder.Services.ConfigureProblemDetails();
                builder.Services.ConfigureApiVersioning();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.ConfigureSwagger(builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging();
                app.UseExceptionHandler();
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapDefaultHealthChecks();
                app.MapControllers();
                app.UseSwashbuckle(builder.Configuration);

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