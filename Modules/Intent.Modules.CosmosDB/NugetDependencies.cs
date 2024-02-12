using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    internal class NugetDependencies
    {
        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => new(
            name: "IEvangelist.Azure.CosmosRepository",
            version: outputTarget.GetProject().GetMaxNetAppVersion() switch
            {
                (5, 0) => "3.7.0",
                (6, 0) => "3.7.0",
                (7, 0) => "7.1.0",
                _ => "8.1.3"
            });
        /// <summary>
        /// This dependency is introduced to override the version installed by CosmosDB due to a possible security
        /// vulnerability. https://github.com/advisories/GHSA-5crp-9r3c-p9vr
        /// </summary>
        public static readonly INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "13.0.3");
        public static readonly INugetPackageInfo FinbuckleMultiTenant = new NugetPackageInfo("Finbuckle.MultiTenant", "6.12.0");
    }
}
