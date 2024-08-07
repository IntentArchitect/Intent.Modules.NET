using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DapperHybrid
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
    }
}
