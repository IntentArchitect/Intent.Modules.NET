using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Ardalis.GuardClauses
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ArdalisGuardClauses(IOutputTarget outputTarget) => new(
            name: "Ardalis.GuardClauses",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "4.0.1",
                (7, 0) => "4.5.0",
                _ => "4.6.0",
            });
    }
}
