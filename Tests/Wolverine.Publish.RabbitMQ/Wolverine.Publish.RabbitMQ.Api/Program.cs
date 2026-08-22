using Intent.RoslynWeaver.Attributes;
using Serilog;
using Serilog.Events;
using Wolverine;
using Wolverine.Publish.RabbitMQ.Api.Configuration;
using Wolverine.Publish.RabbitMQ.Api.Filters;
using Wolverine.Publish.RabbitMQ.Api.Logging;
using Wolverine.Publish.RabbitMQ.Application;
using Wolverine.Publish.RabbitMQ.Infrastructure;
using Wolverine.Publish.RabbitMQ.Infrastructure.Configuration;
using Wolverine.Publish.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Api
{
    public class Program
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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
                // Golden phase, hand-written: stands in for the chain statement
                // Intent.Wolverine.Common will later emit via ConfigureHostBuilderChainStatement.
                // Survives regeneration only because of the [IntentManaged(Body = Mode.Merge)]
                // directive on Main - without it the Intent.AspNetCore.Program template rebuilds
                // this body wholesale and strips the line. Comments above the attribute are NOT
                // protected by Body = Mode.Merge, which is why this note lives inside the body.
                builder.Host.UseWolverine(opts =>
                {
                    WolverineConfiguration.Configure(opts);
                    WolverineEventingConfiguration.ConfigureRabbitMq(opts, builder.Configuration);
                });

                // Add services to the container.
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
