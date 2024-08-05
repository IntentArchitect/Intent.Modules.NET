using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching
{
    public static class NugetPackages
    {

        public static NugetPackageInfo EFCoreSecondLevelCacheInterceptor(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "EFCoreSecondLevelCacheInterceptor",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "4.6.0",
                (>= 7, 0) => "4.6.0",
                (>= 6, 0) => "4.6.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'EFCoreSecondLevelCacheInterceptor'")
            });

        public static NugetPackageInfo MessagePack(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MessagePack",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "2.5.172",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MessagePack'")
            });
    }
}
