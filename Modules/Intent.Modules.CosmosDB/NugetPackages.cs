using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    public static class NugetPackages
    {

        public static NugetPackageInfo NewtonsoftJson(IOutputTarget outputTarget) => new(
            name: "Newtonsoft.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "13.0.3",
            });

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => new(
            name: "Finbuckle.MultiTenant",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.13.1",
            });

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => new(
            name: "IEvangelist.Azure.CosmosRepository",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (7, 0) => "8.1.7",
                _ => "8.1.7",
            });
    }
}
