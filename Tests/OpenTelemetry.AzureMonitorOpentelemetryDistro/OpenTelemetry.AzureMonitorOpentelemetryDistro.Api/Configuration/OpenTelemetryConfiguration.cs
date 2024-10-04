using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration", Version = "1.0")]

namespace OpenTelemetry.AzureMonitorOpentelemetryDistro.Api.Configuration
{
    public static class OpenTelemetryConfiguration
    {
        public static IServiceCollection AddTelemetryConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOpenTelemetry().UseAzureMonitor(opt =>
            {
                opt.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
            })
                .ConfigureResource(res => res
                    .AddService(serviceName: configuration["OpenTelemetry:ServiceName"]!, serviceInstanceId: configuration.GetValue<string?>("OpenTelemetry:ServiceInstanceId"))
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector());
            return services;
        }
    }
}