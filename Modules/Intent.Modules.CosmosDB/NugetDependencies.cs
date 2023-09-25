using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    internal class NugetDependencies
    {
        public static readonly INugetPackageInfo IEvangelistAzureCosmosRepository = new NugetPackageInfo("IEvangelist.Azure.CosmosRepository", "3.7.0");
        public static readonly INugetPackageInfo FinbuckleMultiTenant = new NugetPackageInfo("Finbuckle.MultiTenant", "6.12.0");
    }
}
