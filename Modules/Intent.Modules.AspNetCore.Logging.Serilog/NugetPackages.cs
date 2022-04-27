using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Logging.Serilog
{
    public static class NugetPackages
    {
        public static readonly INugetPackageInfo SerilogAspNetCore = new NugetPackageInfo("Serilog.AspNetCore", "5.0.0");
    }
}
