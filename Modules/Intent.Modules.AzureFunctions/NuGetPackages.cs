using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions
{
    public class NugetPackages
    {
        public const string AzureMessagingEventHubsPackageName = "Azure.Messaging.EventHubs";
        public const string MediatRPackageName = "MediatR";
        public const string MicrosoftAzureFunctionsExtensionsPackageName = "Microsoft.Azure.Functions.Extensions";
        public const string MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName = "Microsoft.Azure.Functions.Worker.Extensions.Timer";
        public const string MicrosoftAzureServiceBusPackageName = "Microsoft.Azure.ServiceBus";
        public const string MicrosoftAzureWebJobsExtensionsCosmosDBPackageName = "Microsoft.Azure.WebJobs.Extensions.CosmosDB";
        public const string MicrosoftAzureWebJobsExtensionsEventHubsPackageName = "Microsoft.Azure.WebJobs.Extensions.EventHubs";
        public const string MicrosoftAzureWebJobsExtensionsRabbitMQPackageName = "Microsoft.Azure.WebJobs.Extensions.RabbitMQ";
        public const string MicrosoftAzureWebJobsExtensionsServiceBusPackageName = "Microsoft.Azure.WebJobs.Extensions.ServiceBus";
        public const string MicrosoftAzureWebJobsExtensionsStorageQueuesPackageName = "Microsoft.Azure.WebJobs.Extensions.Storage.Queues";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";
        public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
        public const string MicrosoftNETSdkFunctionsPackageName = "Microsoft.NET.Sdk.Functions";

        static NugetPackages()
        {
            NugetRegistry.Register(AzureMessagingEventHubsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.9.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMessagingEventHubsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MediatRPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("12.4.0"),
                        ( >= 6, 0) => new PackageVersion("12.1.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.1.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsExtensionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("4.2.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureServiceBusPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.2.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsCosmosDBPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("4.3.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsCosmosDBPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsEventHubsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.3.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsEventHubsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsRabbitMQPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("2.0.3", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsServiceBusPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.5.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsStorageQueuesPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsStorageQueuesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("6.0.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHttpPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftNETSdkFunctionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("4.2.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftNETSdkFunctionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftNETSdkFunctions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftNETSdkFunctionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsStorageQueues(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsStorageQueuesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsTimer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureMessagingEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMessagingEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsCosmosDBPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
