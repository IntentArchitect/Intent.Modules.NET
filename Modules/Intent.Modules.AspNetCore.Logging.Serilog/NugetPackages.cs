using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Logging.Serilog
{
    public static class NugetPackages
    {
        public static NugetPackageInfo SerilogAspNetCore(IOutputTarget outputTarget) => new(
            name: "Serilog.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (5, 0) => "5.0.0",
                (6, 0) => "6.0.0",
                (7, 0) => "7.0.0",
                _ => "8.0.0"
            });

        public static readonly NugetPackageInfo SerilogSinksGraylog = new NugetPackageInfo("serilog.sinks.graylog", "3.1.1");
        public static readonly NugetPackageInfo SerilogEnrichersSpan = new NugetPackageInfo("Serilog.Enrichers.Span", "3.1.0");
        
    }
}
