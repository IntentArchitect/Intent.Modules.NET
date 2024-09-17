using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.OpenTelemetry
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureMonitorOpenTelemetryAspNetCorePackageName = "Azure.Monitor.OpenTelemetry.AspNetCore";
        public const string AzureMonitorOpenTelemetryExporterPackageName = "Azure.Monitor.OpenTelemetry.Exporter";
        public const string OpenTelemetryPackageName = "OpenTelemetry";
        public const string OpenTelemetryExporterConsolePackageName = "OpenTelemetry.Exporter.Console";
        public const string OpenTelemetryExporterOpenTelemetryProtocolPackageName = "OpenTelemetry.Exporter.OpenTelemetryProtocol";
        public const string OpenTelemetryExtensionsHostingPackageName = "OpenTelemetry.Extensions.Hosting";
        public const string OpenTelemetryInstrumentationAspNetCorePackageName = "OpenTelemetry.Instrumentation.AspNetCore";
        public const string OpenTelemetryInstrumentationHttpPackageName = "OpenTelemetry.Instrumentation.Http";
        public const string OpenTelemetryInstrumentationProcessPackageName = "OpenTelemetry.Instrumentation.Process";
        public const string OpenTelemetryInstrumentationRuntimePackageName = "OpenTelemetry.Instrumentation.Runtime";
        public const string OpenTelemetryInstrumentationSqlClientPackageName = "OpenTelemetry.Instrumentation.SqlClient";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureMonitorOpenTelemetryAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMonitorOpenTelemetryAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(AzureMonitorOpenTelemetryExporterPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMonitorOpenTelemetryExporterPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExporterConsolePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExporterConsolePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExporterOpenTelemetryProtocolPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExporterOpenTelemetryProtocolPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExtensionsHostingPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationHttpPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationProcessPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("0.5.0-beta.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationProcessPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationRuntimePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationRuntimePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationSqlClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.9.0-beta.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationSqlClientPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureMonitorOpenTelemetryAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMonitorOpenTelemetryAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetry(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryInstrumentationAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryInstrumentationAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryInstrumentationHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryInstrumentationHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryInstrumentationProcess(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryInstrumentationProcessPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryInstrumentationRuntime(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryInstrumentationRuntimePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryInstrumentationSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryInstrumentationSqlClientPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryExporterConsole(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryExporterConsolePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OpenTelemetryExporterOpenTelemetryProtocol(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OpenTelemetryExporterOpenTelemetryProtocolPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureMonitorOpenTelemetryExporter(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMonitorOpenTelemetryExporterPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
