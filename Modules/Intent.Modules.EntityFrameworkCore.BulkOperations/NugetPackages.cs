using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.BulkOperations
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ZEntityFrameworkExtensionsEFCore(IOutputTarget outputTarget) => new(
            name: "Z.EntityFramework.Extensions.EFCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "7.103.1",
                _ => "8.103.1",
            });
    }
}
