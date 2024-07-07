using Azure.Monitor.OpenTelemetry.Exporter;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration", Version = "1.0")]

namespace OpenTelemetry.AzureAppInsights.Api.Configuration
{
    public static class OpenTelemetryConfiguration
    {
        public static IServiceCollection AddTelemetryConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(res => res
                    .AddService(configuration["OpenTelemetry:ServiceName"]!)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .WithTracing(trace => trace
                    .AddAspNetCoreInstrumentation()
                    .AddAzureMonitorTraceExporter(opt =>
                    {
                        opt.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                    }));
            return services;
        }

        public static ILoggingBuilder AddTelemetryConfiguration(
            this ILoggingBuilder logBuilder,
            HostBuilderContext context)
        {
            return logBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddService(context.Configuration["OpenTelemetry:ServiceName"]!));
                options.AddAzureMonitorLogExporter(opt =>
                {
                    opt.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"];
                });
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
            });
        }
    }
}