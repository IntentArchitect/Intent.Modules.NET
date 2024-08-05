using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Logging.Serilog
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SerilogAspNetCore(IOutputTarget outputTarget) => new(
            name: "Serilog.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.2",
                (7, 0) => "8.0.2",
                _ => "8.0.2",
            });

        public static NugetPackageInfo SerilogSinksGraylog(IOutputTarget outputTarget) => new(
            name: "serilog.sinks.graylog",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.1.1",
                _ => "3.1.1",
            });

        public static NugetPackageInfo SerilogEnrichersSpan(IOutputTarget outputTarget) => new(
            name: "Serilog.Enrichers.Span",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.1.0",
                _ => "3.1.0",
            });

        public static NugetPackageInfo SerilogSinksApplicationInsights(IOutputTarget outputTarget) => new(
            name: "Serilog.Sinks.ApplicationInsights",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.0.0",
            });
    }
}
