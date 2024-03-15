using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.OpenTelemetry;

public static class NugetPackages
{
    public static readonly INugetPackageInfo OpenTelemetry = new NugetPackageInfo("OpenTelemetry", "1.6.0");
    public static readonly INugetPackageInfo OpenTelemetryExtensionsHosting = new NugetPackageInfo("OpenTelemetry.Extensions.Hosting", "1.6.0");
    public static readonly INugetPackageInfo OpenTelemetryInstrumentationAspNetCore = new NugetPackageInfo("OpenTelemetry.Instrumentation.AspNetCore", "1.6.0");
    public static readonly INugetPackageInfo OpenTelemetryInstrumentationHttp = new NugetPackageInfo("OpenTelemetry.Instrumentation.Http", "1.6.0");
    public static readonly INugetPackageInfo OpenTelemetryInstrumentationSqlClient = new NugetPackageInfo("OpenTelemetry.Instrumentation.SqlClient", "1.6.0-beta.3");
    
    public static readonly INugetPackageInfo OpenTelemetryExporterConsole = new NugetPackageInfo("OpenTelemetry.Exporter.Console", "1.6.0");
    public static readonly INugetPackageInfo OpenTelemetryExporterOpenTelemetryProtocol = new NugetPackageInfo("OpenTelemetry.Exporter.OpenTelemetryProtocol", "1.6.0");
    public static readonly INugetPackageInfo AzureMonitorOpenTelemetryExporter = new NugetPackageInfo("Azure.Monitor.OpenTelemetry.Exporter", "1.0.0");
}