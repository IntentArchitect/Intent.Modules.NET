using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions;

static class NuGetPackages
{
    public static readonly INugetPackageInfo MicrosoftNETSdkFunctions = new NugetPackageInfo("Microsoft.NET.Sdk.Functions", "4.1.1");
    public static readonly INugetPackageInfo MicrosoftExtensionsDependencyInjection = new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", "6.0.0");
    public static readonly INugetPackageInfo MicrosoftExtensionsHttp = new NugetPackageInfo("Microsoft.Extensions.Http", "6.0.0");
    public static readonly INugetPackageInfo MicrosoftAzureFunctionsExtensions = new NugetPackageInfo("Microsoft.Azure.Functions.Extensions", "1.1.0");
    public static readonly INugetPackageInfo MicrosoftAzureServiceBus = new NugetPackageInfo("Microsoft.Azure.ServiceBus", "5.2.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.ServiceBus", "5.5.1");
    public static readonly INugetPackageInfo AzureStorageQueues = new NugetPackageInfo("Azure.Storage.Queues", "12.2.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsStorage = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.Storage", "4.0.5");
}