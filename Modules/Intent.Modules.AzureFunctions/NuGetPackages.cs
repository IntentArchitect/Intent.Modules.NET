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
        public const string MicrosoftApplicationInsightsWorkerServicePackageName = "Microsoft.ApplicationInsights.WorkerService";
        public const string MicrosoftAzureFunctionsExtensionsPackageName = "Microsoft.Azure.Functions.Extensions";
        public const string MicrosoftAzureFunctionsWorkerPackageName = "Microsoft.Azure.Functions.Worker";
        public const string MicrosoftAzureFunctionsWorkerApplicationInsightsPackageName = "Microsoft.Azure.Functions.Worker.ApplicationInsights";
        public const string MicrosoftAzureFunctionsWorkerExtensionsCosmosDBPackageName = "Microsoft.Azure.Functions.Worker.Extensions.CosmosDB";
        public const string MicrosoftAzureFunctionsWorkerExtensionsEventHubsPackageName = "Microsoft.Azure.Functions.Worker.Extensions.EventHubs";
        public const string MicrosoftAzureFunctionsWorkerExtensionsHttpPackageName = "Microsoft.Azure.Functions.Worker.Extensions.Http";
        public const string MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCorePackageName = "Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore";
        public const string MicrosoftAzureFunctionsWorkerExtensionsRabbitMQPackageName = "Microsoft.Azure.Functions.Worker.Extensions.RabbitMQ";
        public const string MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName = "Microsoft.Azure.Functions.Worker.Extensions.ServiceBus";
        public const string MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName = "Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues";
        public const string MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName = "Microsoft.Azure.Functions.Worker.Extensions.Timer";
        public const string MicrosoftAzureFunctionsWorkerSdkPackageName = "Microsoft.Azure.Functions.Worker.Sdk";
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
        public const string RabbitMQClientPackageName = "RabbitMQ.Client";

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
            NugetRegistry.Register(MicrosoftApplicationInsightsWorkerServicePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights", "2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.DependencyCollector", "2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.EventCounterCollector", "2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.PerfCounterCollector", "2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.WindowsServer", "2.22.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel", "2.22.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "2.1.1")
                            .WithNugetDependency("Microsoft.Extensions.Logging.ApplicationInsights", "2.22.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftApplicationInsightsWorkerServicePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.1.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsExtensionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.23.0")
                            .WithNugetDependency("Azure.Core", "1.41.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.19.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Grpc", "1.17.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "5.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "5.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerApplicationInsightsPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.4.0")
                            .WithNugetDependency("Azure.Identity", "1.12.0")
                            .WithNugetDependency("Microsoft.ApplicationInsights.PerfCounterCollector", "2.22.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.19.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "5.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerApplicationInsightsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsCosmosDBPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.39.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.19.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0")
                            .WithNugetDependency("Microsoft.Extensions.Azure", "1.7.5")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsCosmosDBPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsEventHubsPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("6.3.6")
                            .WithNugetDependency("Azure.Messaging.EventHubs", "5.11.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.19.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0")
                            .WithNugetDependency("Microsoft.Extensions.Azure", "1.7.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsEventHubsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsHttpPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("3.2.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.18.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.3.2")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker", "1.22.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.2.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore.Analyzers", "1.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsRabbitMQPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("2.0.3")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.2.0-preview1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("5.22.0")
                            .WithNugetDependency("Azure.Identity", "1.12.0")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.1")
                            .WithNugetDependency("Google.Protobuf", "3.27.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.19.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Rpc", "1.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Azure", "1.7.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("5.5.0")
                            .WithNugetDependency("Azure.Storage.Queues", "12.17.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.18.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("4.3.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.18.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsTimerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerSdkPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.18.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Sdk.Analyzers", "1.2.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Sdk.Generators", "1.3.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerSdkPackageName}'"),
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
                        ( >= 8, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.0"),
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
            NugetRegistry.Register(RabbitMQClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("6.8.1")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Threading.Channels", "7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{RabbitMQClientPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftNETSdkFunctions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftNETSdkFunctionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo RabbitMQClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(RabbitMQClientPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsLoggingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorker(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerApplicationInsights(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerApplicationInsightsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsCosmosDB(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsCosmosDBPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsStorageQueues(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName, outputTarget.GetMaxNetAppVersion());

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

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerSdk(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerSdkPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureMessagingEventHubs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMessagingEventHubsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftApplicationInsightsWorkerService(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftApplicationInsightsWorkerServicePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsCosmosDB(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsCosmosDBPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
