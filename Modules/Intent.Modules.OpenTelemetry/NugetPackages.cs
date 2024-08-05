using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.OpenTelemetry
{
    public static class NugetPackages
    {

        public static NugetPackageInfo OpenTelemetry(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry'")
            });

        public static NugetPackageInfo OpenTelemetryExtensionsHosting(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Extensions.Hosting'")
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Instrumentation.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 7, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Instrumentation.AspNetCore'")
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationHttp(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Instrumentation.Http",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Instrumentation.Http'")
            });

        public static NugetPackageInfo OpenTelemetryInstrumentationSqlClient(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Instrumentation.SqlClient",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Instrumentation.SqlClient'")
            });

        public static NugetPackageInfo OpenTelemetryExporterConsole(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Exporter.Console",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Exporter.Console'")
            });

        public static NugetPackageInfo OpenTelemetryExporterOpenTelemetryProtocol(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "OpenTelemetry.Exporter.OpenTelemetryProtocol",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.9.0",
                (>= 6, 0) => "1.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'OpenTelemetry.Exporter.OpenTelemetryProtocol'")
            });

        public static NugetPackageInfo AzureMonitorOpenTelemetryExporter(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Monitor.OpenTelemetry.Exporter",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "1.3.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Monitor.OpenTelemetry.Exporter'")
            });
    }
}
