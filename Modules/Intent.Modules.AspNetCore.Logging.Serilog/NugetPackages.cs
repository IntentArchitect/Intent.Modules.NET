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
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("8.0.2"),
                        ( >= 6, 0) => new PackageVersion("8.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogEnrichersSpanPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("3.1.0"),
                        ( >= 6, 0) => new PackageVersion("3.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogEnrichersSpanPackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogSinksApplicationInsightsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("4.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SerilogSinksApplicationInsightsPackageName}'"),
                    }
                );
            NugetRegistry.Register(SerilogSinksGraylogPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("3.1.1"),
                        ( >= 6, 0) => new PackageVersion("3.1.1"),
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
