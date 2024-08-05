using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapper
{
    public static class NugetPackages
    {

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => new(
            name: "Dapper",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.1.35",
            });

        public static NugetPackageInfo SystemDataSqlClient(IOutputTarget outputTarget) => new(
            name: "System.Data.SqlClient",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.8.6",
            });
    }
}
