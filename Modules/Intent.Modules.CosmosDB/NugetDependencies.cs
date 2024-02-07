using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    internal class NugetDependencies
    {
        public static readonly INugetPackageInfo IEvangelistAzureCosmosRepository = new NugetPackageInfo("IEvangelist.Azure.CosmosRepository", "8.1.2");
        /// <summary>
        /// This dependency is introduced to override the version installed by CosmosDB due to a possible security
        /// vulnerability. https://github.com/advisories/GHSA-5crp-9r3c-p9vr
        /// </summary>
        public static readonly INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "13.0.3");
        public static readonly INugetPackageInfo FinbuckleMultiTenant = new NugetPackageInfo("Finbuckle.MultiTenant", "6.12.0");
    }
}
