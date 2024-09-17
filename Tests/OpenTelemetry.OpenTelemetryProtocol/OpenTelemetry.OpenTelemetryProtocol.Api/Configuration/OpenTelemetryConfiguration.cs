using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration", Version = "1.0")]

namespace OpenTelemetry.OpenTelemetryProtocol.Api.Configuration
{
    public static class OpenTelemetryConfiguration
    {
        public static IServiceCollection AddTelemetryConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(res => res
                    .AddService(serviceName: configuration["OpenTelemetry:ServiceName"]!, serviceInstanceId: configuration.GetValue<string?>("OpenTelemetry:ServiceInstanceId"))
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .WithTracing(trace => trace
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = configuration.GetValue<Uri>("open-telemetry-protocol:endpoint")!;
                        opt.Protocol = configuration.GetValue<OtlpExportProtocol>("open-telemetry-protocol:protocol");
                    }))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = configuration.GetValue<Uri>("open-telemetry-protocol:endpoint")!;
                        opt.Protocol = configuration.GetValue<OtlpExportProtocol>("open-telemetry-protocol:protocol");
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
                options.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = context.Configuration.GetValue<Uri>("open-telemetry-protocol:endpoint")!;
                    opt.Protocol = context.Configuration.GetValue<OtlpExportProtocol>("open-telemetry-protocol:protocol");
                });
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
            });
        }
    }
}