using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching
{
    internal class NugetPackages
    {
        public static INugetPackageInfo EFCoreSecondLevelCacheInterceptor = new NugetPackageInfo("EFCoreSecondLevelCacheInterceptor", "4.2.3");
        public static INugetPackageInfo MessagePack = new NugetPackageInfo("MessagePack", "2.5.140");
    }
}
