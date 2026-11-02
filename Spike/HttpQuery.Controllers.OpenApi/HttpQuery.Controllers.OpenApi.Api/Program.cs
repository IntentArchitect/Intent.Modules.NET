using HttpQuery.Controllers.OpenApi.Api.Configuration;
using HttpQuery.Controllers.OpenApi.Api.Filters;
using HttpQuery.Controllers.OpenApi.Api.Logging;
using HttpQuery.Controllers.OpenApi.Application;
using HttpQuery.Controllers.OpenApi.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace HttpQuery.Controllers.OpenApi.Api
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
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()));

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
                builder.Services.ConfigureOpenApi();
                builder.Services.ConfigureHttpQueryOpenApi();

                var app = builder.Build();

                // Spike only: create the SQLite schema so the running API can be exercised by hand.
                // Off unless HTTPQUERY_ENSURE_DB=true, so the integration tests' WebApplicationFactory never runs it.
                if (Environment.GetEnvironmentVariable("HTTPQUERY_ENSURE_DB") == "true")
                {
                    using var scope = app.Services.CreateScope();
                    scope.ServiceProvider.GetRequiredService<HttpQuery.Controllers.OpenApi.Infrastructure.Persistence.ApplicationDbContext>().Database.EnsureCreated();
                }

                // Configure the HTTP request pipeline.
                app.UseHttpQueryOpenApi();
                app.UseSerilogRequestLogging();
                app.UseExceptionHandler();
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapScalarApiReference();
                app.MapOpenApi();
                app.MapDefaultHealthChecks();
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
