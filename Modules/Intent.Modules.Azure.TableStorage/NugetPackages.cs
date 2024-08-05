using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.TableStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureDataTables(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Data.Tables",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "12.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Data.Tables'")
            });
    }
}
