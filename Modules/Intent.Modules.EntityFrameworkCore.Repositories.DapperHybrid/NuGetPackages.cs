using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DapperHybrid
{
    public static class NugetPackages
    {

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => new(
            name: "Dapper",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.1.35",
            });
    }
}
