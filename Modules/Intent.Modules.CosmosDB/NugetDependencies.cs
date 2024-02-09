using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    internal class NugetDependencies
    {
        /// <summary>
        /// I have not configured different output targets because version 3.7.0 of the repository does not support etags at all, and version 7.x.x 
        /// supports .NET 7.0. However, version 8.1.2 supports .NET 7.0 and .NET 8.0, along with netstandard2.0. Version 3.7.0 only supports netstandard2.0.
        /// This means that all support is encapsulated by this version.
        /// </summary>
        public static readonly INugetPackageInfo IEvangelistAzureCosmosRepository = new NugetPackageInfo("IEvangelist.Azure.CosmosRepository", "8.1.2");
        /// <summary>
        /// This dependency is introduced to override the version installed by CosmosDB due to a possible security
        /// vulnerability. https://github.com/advisories/GHSA-5crp-9r3c-p9vr
        /// </summary>
        public static readonly INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "13.0.3");
        public static readonly INugetPackageInfo FinbuckleMultiTenant = new NugetPackageInfo("Finbuckle.MultiTenant", "6.12.0");
    }
}
