using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftNETSdkFunctions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.NET.Sdk.Functions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "4.2.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.NET.Sdk.Functions'")
            });

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Http",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Http'")
            });

        public static NugetPackageInfo MicrosoftAzureFunctionsExtensions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.Functions.Extensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "1.1.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.Functions.Extensions'")
            });

        public static NugetPackageInfo MicrosoftAzureServiceBus(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.ServiceBus",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "5.2.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.ServiceBus'")
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.ServiceBus",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "5.5.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.ServiceBus'")
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsStorageQueues(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.Storage.Queues",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "5.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.Storage.Queues'")
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsEventHubs(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.EventHubs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "5.3.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.EventHubs'")
            });

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsTimer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.Functions.Worker.Extensions.Timer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "4.2.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.Functions.Worker.Extensions.Timer'")
            });

        public static NugetPackageInfo AzureMessagingEventHubs(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Messaging.EventHubs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "5.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Messaging.EventHubs'")
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.CosmosDB",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "4.3.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.CosmosDB'")
            });

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.RabbitMQ",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "2.0.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.RabbitMQ'")
            });

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.DependencyInjection",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "6.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.DependencyInjection'")
            });
    }
}
