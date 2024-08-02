using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    internal class NugetDependencies
    {
        /// <summary>
        /// This dependency is introduced to override the version installed by CosmosDB due to a possible security
        /// vulnerability. https://github.com/advisories/GHSA-5crp-9r3c-p9vr
        /// </summary>
        /*
        public static readonly INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "13.0.3");
        public static readonly INugetPackageInfo FinbuckleMultiTenant = new NugetPackageInfo("Finbuckle.MultiTenant", "6.12.0");

		public static readonly INugetPackageInfo IEvangelistAzureCosmosRepository = new NugetPackageInfo("IEvangelist.Azure.CosmosRepository", "8.1.5");
		//These are brought in by "IEvangelist.Azure.CosmosRepository"
		public static readonly INugetPackageInfo MicrosoftExtensionsConfigurationAbstractions = new NugetPackageInfo("Microsoft.Extensions.Configuration.Abstractions", "8.0.0");
		public static readonly INugetPackageInfo MicrosoftExtensionsConfigurationBinder = new NugetPackageInfo("Microsoft.Extensions.Configuration.Binder", "8.0.0");
		public static readonly INugetPackageInfo MicrosoftExtensionsDependencyInjection = new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", "8.0.0");
        */
    }
}
