using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftNETSdkFunctions(IOutputTarget outputTarget) => new(
            name: "Microsoft.NET.Sdk.Functions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.2.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Http",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.0.0",
            });

        public static NugetPackageInfo MicrosoftAzureFunctionsExtensions(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.Functions.Extensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.1.0",
            });

        public static NugetPackageInfo MicrosoftAzureServiceBus(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.ServiceBus",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "5.2.0",
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.ServiceBus",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "5.5.1",
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsStorageQueues(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.Storage.Queues",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "5.0.0",
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsEventHubs(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.EventHubs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "5.3.0",
            });

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsTimer(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.Functions.Worker.Extensions.Timer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.2.0",
            });

        public static NugetPackageInfo AzureMessagingEventHubs(IOutputTarget outputTarget) => new(
            name: "Azure.Messaging.EventHubs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "5.9.0",
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.CosmosDB",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.3.0",
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.RabbitMQ",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.0.3",
            });

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.DependencyInjection",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.0.1",
            });
    }
}
