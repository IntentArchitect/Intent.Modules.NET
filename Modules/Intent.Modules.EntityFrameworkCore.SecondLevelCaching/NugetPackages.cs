using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching
{
    public static class NugetPackages
    {

        public static NugetPackageInfo EFCoreSecondLevelCacheInterceptor(IOutputTarget outputTarget) => new(
            name: "EFCoreSecondLevelCacheInterceptor",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "4.6.0",
                (7, 0) => "4.6.0",
                _ => "4.6.0",
            });

        public static NugetPackageInfo MessagePack(IOutputTarget outputTarget) => new(
            name: "MessagePack",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.5.172",
            });
    }
}
