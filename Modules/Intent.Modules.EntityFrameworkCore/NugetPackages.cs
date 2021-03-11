using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore
{
    public static class NugetPackages
    {
        public static NugetPackageInfo EntityFrameworkCore = new NugetPackageInfo("Microsoft.EntityFrameworkCore", "3.1.13");
        public static NugetPackageInfo EntityFrameworkCoreDesign = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Design", "3.1.13");
        public static NugetPackageInfo EntityFrameworkCoreTools = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Tools", "3.1.13");
        public static NugetPackageInfo EntityFrameworkCoreSqlServer = new NugetPackageInfo("Microsoft.EntityFrameworkCore.SqlServer", "3.1.13");
        public static NugetPackageInfo EntityFrameworkCoreInMemory = new NugetPackageInfo("Microsoft.EntityFrameworkCore.InMemory", "3.1.13");
        public static NugetPackageInfo EntityFrameworkCoreProxies = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Proxies", "3.1.13");
    }
}
