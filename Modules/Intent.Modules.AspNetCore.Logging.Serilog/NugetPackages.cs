using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog
{
    public class NugetPackages : INugetPackages
    {
        public const string SerilogAspNetCorePackageName = "Serilog.AspNetCore";
        public const string SerilogEnrichersSpanPackageName = "Serilog.Enrichers.Span";
        public const string SerilogSinksApplicationInsightsPackageName = "Serilog.Sinks.ApplicationInsights";
        public const string SerilogSinksGraylogPackageName = "serilog.sinks.graylog";

        public void RegisterPackages()
        {
            NugetRegistry.Register(SerilogAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Serilog", "4.3.0")
                            .WithNugetDependency("Serilog.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Serilog.Formatting.Compact", "3.0.0")
                            .WithNugetDependency("Serilog.Settings.Configuration", "10.0.0")
                            .WithNugetDependency("Serilog.Sinks.Console", "6.1.1")
                            .WithNugetDependency("Serilog.Sinks.Debug", "3.0.0")
                            .WithNugetDependency("Serilog.Sinks.File", "7.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Serilog", "4.3.0")
                            .WithNugetDependency("Serilog.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Serilog.Formatting.Compact", "3.0.0")
                            .WithNugetDependency("Serilog.Settings.Configuration", "10.0.0")
                            .WithNugetDependency("Serilog.Sinks.Console", "6.1.1")
                            .WithNugetDependency("Serilog.Sinks.Debug", "3.0.0")
                            .WithNugetDependency("Serilog.Sinks.File", "7.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Serilog", "4.3.0")
                            .WithNugetDependency("Serilog.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Serilog.Formatting.Compact", "3.0.0")
                            .WithNugetDependency("Serilog.Settings.Configuration", "10.0.0")
                            .WithNugetDependency("Serilog.Sinks.Console", "6.1.1")
                            .WithNugetDependency("Serilog.Sinks.Debug", "3.0.0")
                            .WithNugetDependency("Serilog.Sinks.File", "7.0.0"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Hosting.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.3.0")
                            .WithNugetDependency("Serilog", "4.3.0")
                            .WithNugetDependency("Serilog.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Serilog.Formatting.Compact", "3.0.0")
                            .WithNugetDependency("Serilog.Settings.Configuration", "10.0.0")
                            .WithNugetDependency("Serilog.Sinks.Console", "6.1.1")
                            .WithNugetDependency("Serilog.Sinks.Debug", "3.0.0")
                            .WithNugetDependency("Serilog.Sinks.File", "7.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Hosting.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.3.0")
                            .WithNugetDependency("Serilog", "4.3.0")
                            .WithNugetDependency("Serilog.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Serilog.Formatting.Compact", "3.0.0")
                            .WithNugetDependency("Serilog.Settings.Configuration", "10.0.0")
                            .WithNugetDependency("Serilog.Sinks.Console", "6.1.1")
                            .WithNugetDependency("Serilog.Sinks.Debug", "3.0.0")
                            .WithNugetDependency("Serilog.Sinks.File", "7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogEnrichersSpanPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 7, >= 0) => new PackageVersion("3.1.0"),
                        ( >= 6, >= 0) => new PackageVersion("3.1.0"),
                        ( >= 2, >= 0) => new PackageVersion("3.1.0")
                            .WithNugetDependency("Serilog", "2.10.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogEnrichersSpanPackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogSinksApplicationInsightsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("4.1.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights", "2.23.0")
                            .WithNugetDependency("Serilog", "4.2.0"),
                        ( >= 2, >= 0) => new PackageVersion("4.1.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights", "2.23.0")
                            .WithNugetDependency("Serilog", "4.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogSinksApplicationInsightsPackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogSinksGraylogPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 7, >= 0) => new PackageVersion("3.1.1"),
                        ( >= 6, >= 0) => new PackageVersion("3.1.1"),
                        ( >= 2, >= 0) => new PackageVersion("3.1.1")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Text.Encoding", "4.3.0")
                            .WithNugetDependency("System.Text.Json", "7.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogSinksGraylogPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SerilogAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SerilogAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SerilogSinksGraylog(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SerilogSinksGraylogPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SerilogEnrichersSpan(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SerilogEnrichersSpanPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SerilogSinksApplicationInsights(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SerilogSinksApplicationInsightsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
