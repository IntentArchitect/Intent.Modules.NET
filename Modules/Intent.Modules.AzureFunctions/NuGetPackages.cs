using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions
{
    public class NugetPackages : INugetPackages
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
        public const string MicrosoftEntityFrameworkCorePackageName = "Microsoft.EntityFrameworkCore";
        public const string MicrosoftEntityFrameworkCoreCosmosPackageName = "Microsoft.EntityFrameworkCore.Cosmos";
        public const string MicrosoftEntityFrameworkCoreDesignPackageName = "Microsoft.EntityFrameworkCore.Design";
        public const string MicrosoftEntityFrameworkCoreInMemoryPackageName = "Microsoft.EntityFrameworkCore.InMemory";
        public const string MicrosoftEntityFrameworkCoreProxiesPackageName = "Microsoft.EntityFrameworkCore.Proxies";
        public const string MicrosoftEntityFrameworkCoreSqlServerPackageName = "Microsoft.EntityFrameworkCore.SqlServer";
        public const string MicrosoftEntityFrameworkCoreToolsPackageName = "Microsoft.EntityFrameworkCore.Tools";
        public const string MicrosoftExtensionsConfigurationAbstractionsPackageName = "Microsoft.Extensions.Configuration.Abstractions";
        public const string MicrosoftExtensionsConfigurationBinderPackageName = "Microsoft.Extensions.Configuration.Binder";
        public const string MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName = "Microsoft.Extensions.Configuration.EnvironmentVariables";
        public const string MicrosoftExtensionsConfigurationFileExtensionsPackageName = "Microsoft.Extensions.Configuration.FileExtensions";
        public const string MicrosoftExtensionsConfigurationJsonPackageName = "Microsoft.Extensions.Configuration.Json";
        public const string MicrosoftExtensionsConfigurationUserSecretsPackageName = "Microsoft.Extensions.Configuration.UserSecrets";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";
        public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
        public const string MicrosoftExtensionsLoggingPackageName = "Microsoft.Extensions.Logging";
        public const string MicrosoftNETSdkFunctionsPackageName = "Microsoft.NET.Sdk.Functions";

        public void RegisterPackages()
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
            NugetRegistry.Register(MicrosoftEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreCosmosPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreCosmosPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreDesignPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreDesignPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreProxiesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreProxiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreToolsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreToolsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationAbstractionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationBinderPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("8.0.2"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationFileExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.1"),
                        ( >= 7, 0) => new PackageVersion("8.0.1"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationFileExtensionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationJsonPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationJsonPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationUserSecretsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationUserSecretsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
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
            NugetRegistry.Register(MicrosoftExtensionsLoggingPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsLoggingPackageName}'"),
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

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsLoggingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsStorageQueues(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsStorageQueuesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreCosmos(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreCosmosPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreDesign(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreDesignPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreInMemoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreProxies(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreProxiesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreSqlServerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreTools(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreToolsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationEnvironmentVariables(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationFileExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationJsonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationUserSecrets(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationUserSecretsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsTimer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureMessagingEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMessagingEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsCosmosDBPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
