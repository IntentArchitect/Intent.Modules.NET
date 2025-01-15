using System;
using Hangfire.Tests.Api.Configuration;
using Hangfire.Tests.Api.Filters;
using Hangfire.Tests.Application;
using Hangfire.Tests.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Hangfire.Tests.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
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
                    .ReadFrom.Services(services));

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
                builder.Services.ConfigureHangfire(builder.Configuration);
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
                app.UseHangfire(builder.Configuration);
                app.UseSwashbuckle(builder.Configuration);

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
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}