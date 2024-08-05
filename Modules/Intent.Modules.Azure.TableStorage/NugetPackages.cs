using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.TableStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureDataTables(IOutputTarget outputTarget) => new(
            name: "Azure.Data.Tables",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "12.9.0",
            });
    }
}
