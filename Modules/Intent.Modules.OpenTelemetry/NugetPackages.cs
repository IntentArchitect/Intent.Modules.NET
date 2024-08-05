using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.OpenTelemetry
{
    public static class NugetPackages
    {

        public static NugetPackageInfo OpenTelemetry(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo OpenTelemetryExtensionsHosting(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationAspNetCore(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Instrumentation.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                (7, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationHttp(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Instrumentation.Http",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationSqlClient(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Instrumentation.SqlClient",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
            });

        public static NugetPackageInfo OpenTelemetryExporterConsole(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Exporter.Console",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo OpenTelemetryExporterOpenTelemetryProtocol(IOutputTarget outputTarget) => new(
            name: "OpenTelemetry.Exporter.OpenTelemetryProtocol",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.9.0",
                _ => "1.9.0",
            });

        public static NugetPackageInfo AzureMonitorOpenTelemetryExporter(IOutputTarget outputTarget) => new(
            name: "Azure.Monitor.OpenTelemetry.Exporter",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.3.0",
            });
    }
}
