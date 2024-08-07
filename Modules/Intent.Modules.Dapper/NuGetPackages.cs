using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapper
{
    public static class NugetPackages
    {

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Dapper",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "2.1.35",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Dapper'")
            });

        public static NugetPackageInfo SystemDataSqlClient(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "System.Data.SqlClient",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "4.8.6",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'System.Data.SqlClient'")
            });
    }
}
