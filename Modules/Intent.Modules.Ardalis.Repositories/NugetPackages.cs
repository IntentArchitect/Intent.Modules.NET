using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Ardalis.Repositories
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ArdalisSpecification(IOutputTarget outputTarget) => new(
            name: "Ardalis.Specification",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });

        public static NugetPackageInfo ArdalisSpecificationEntityFrameworkCore(IOutputTarget outputTarget) => new(
            name: "Ardalis.Specification.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
