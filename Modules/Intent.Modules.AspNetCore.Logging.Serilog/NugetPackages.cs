using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Logging.Serilog
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SerilogAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Serilog.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.2",
                (>= 7, 0) => "8.0.2",
                (>= 6, 0) => "8.0.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Serilog.AspNetCore'")
            });

        public static NugetPackageInfo SerilogSinksGraylog(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "serilog.sinks.graylog",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "3.1.1",
                (>= 6, 0) => "3.1.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'serilog.sinks.graylog'")
            });

        public static NugetPackageInfo SerilogEnrichersSpan(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Serilog.Enrichers.Span",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "3.1.0",
                (>= 6, 0) => "3.1.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Serilog.Enrichers.Span'")
            });

        public static NugetPackageInfo SerilogSinksApplicationInsights(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Serilog.Sinks.ApplicationInsights",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "4.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Serilog.Sinks.ApplicationInsights'")
            });
    }
}
