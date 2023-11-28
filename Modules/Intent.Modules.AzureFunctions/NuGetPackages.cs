using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions;

static class NuGetPackages
{
    public static readonly INugetPackageInfo MicrosoftNETSdkFunctions = new NugetPackageInfo("Microsoft.NET.Sdk.Functions", "4.2.0");
    public static readonly INugetPackageInfo MicrosoftExtensionsDependencyInjection = new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", "6.0.1");
    public static readonly INugetPackageInfo MicrosoftExtensionsHttp = new NugetPackageInfo("Microsoft.Extensions.Http", "6.0.0");
    public static readonly INugetPackageInfo MicrosoftAzureFunctionsExtensions = new NugetPackageInfo("Microsoft.Azure.Functions.Extensions", "1.1.0");
    public static readonly INugetPackageInfo MicrosoftAzureServiceBus = new NugetPackageInfo("Microsoft.Azure.ServiceBus", "5.2.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.ServiceBus", "5.5.1");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsStorageQueues = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.Storage.Queues","5.0.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsEventHubs = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.EventHubs", "5.3.0");
    public static readonly INugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsTimer = new NugetPackageInfo("Microsoft.Azure.Functions.Worker.Extensions.Timer", "4.2.0");
    public static readonly INugetPackageInfo AzureMessagingEventHubs = new NugetPackageInfo("Azure.Messaging.EventHubs", "5.9.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.CosmosDB", "4.3.0");
    public static readonly INugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ = new NugetPackageInfo("Microsoft.Azure.WebJobs.Extensions.RabbitMQ", "2.0.3");
}