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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.4.0")
                            .WithNugetDependency("Azure.Core", "1.50.0")
                            .WithNugetDependency("Azure.Monitor.OpenTelemetry.Exporter", "1.5.0")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Instrumentation.AspNetCore", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Instrumentation.Http", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.4.0")
                            .WithNugetDependency("Azure.Core", "1.50.0")
                            .WithNugetDependency("Azure.Monitor.OpenTelemetry.Exporter", "1.5.0")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Instrumentation.AspNetCore", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Instrumentation.Http", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMonitorOpenTelemetryAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(AzureMonitorOpenTelemetryExporterPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.5.0")
                            .WithNugetDependency("Azure.Core", "1.50.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.PersistentStorage.FileSystem", "1.0.2"),
                        ( >= 2, >= 0) => new PackageVersion("1.5.0")
                            .WithNugetDependency("Azure.Core", "1.50.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.14.0")
                            .WithNugetDependency("OpenTelemetry.PersistentStorage.FileSystem", "1.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMonitorOpenTelemetryExporterPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "9.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "8.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 2, >= 1) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExporterConsolePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 9, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "4.7.2")
                            .WithNugetDependency("System.Text.Json", "4.7.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExporterConsolePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExporterOpenTelemetryProtocolPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 9, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "8.0.2")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 2, >= 1) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExporterOpenTelemetryProtocolPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("OpenTelemetry", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.1.1")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Features", "2.1.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "4.7.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationHttpPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationProcessPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0-beta.2")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0-beta.2")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0-beta.2")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationProcessPackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationRuntimePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0")
                            .WithNugetDependency("OpenTelemetry.Api", "1.14.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OpenTelemetryInstrumentationRuntimePackageName}'"),
                    }
                );
            NugetRegistry.Register(OpenTelemetryInstrumentationSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("1.14.0-beta.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 8, >= 0) => new PackageVersion("1.14.0-beta.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.14.0-beta.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("OpenTelemetry.Api.ProviderBuilderExtensions", "1.14.0"),
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
