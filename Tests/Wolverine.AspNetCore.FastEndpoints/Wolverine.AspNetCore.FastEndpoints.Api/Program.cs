using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Serilog;
using Serilog.Events;
using Wolverine;
using Wolverine.AspNetCore.FastEndpoints.Api.Configuration;
using Wolverine.AspNetCore.FastEndpoints.Api.FastEndpoints;
using Wolverine.AspNetCore.FastEndpoints.Api.Logging;
using Wolverine.AspNetCore.FastEndpoints.Application;
using Wolverine.AspNetCore.FastEndpoints.Infrastructure;
using Wolverine.AspNetCore.FastEndpoints.Infrastructure.Configuration;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api
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
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureHealthChecks(builder.Configuration);
                builder.Services.ConfigureProblemDetails();
                builder.Services.ConfigureApiVersioning();
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Services.AddFastEndpoints(opt =>
                {
                    opt.Assemblies = [typeof(Program).Assembly];
                });

                builder.Host.UseWolverine(opts =>
                {
                    WolverineConfiguration.Configure(opts);
                });

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()));

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging();
                app.UseExceptionHandler();
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapDefaultHealthChecks();
                app.MapFastEndpoints(c => c.Endpoints.Configurator = ep =>
                {
                    ep.PostProcessors(0, new ExceptionProcessor());
                });

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