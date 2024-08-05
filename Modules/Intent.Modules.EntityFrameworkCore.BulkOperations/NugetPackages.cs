using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.BulkOperations
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ZEntityFrameworkExtensionsEFCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Z.EntityFramework.Extensions.EFCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.103.1",
                (>= 6, 0) => "7.103.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Z.EntityFramework.Extensions.EFCore'")
            });
    }
}
