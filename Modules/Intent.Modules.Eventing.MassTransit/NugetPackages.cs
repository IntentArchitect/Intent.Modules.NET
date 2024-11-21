using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit
{
    public class NugetPackages : INugetPackages
    {
        public const string MassTransitPackageName = "MassTransit";
        public const string MassTransitAbstractionsPackageName = "MassTransit.Abstractions";
        public const string MassTransitAmazonSQSPackageName = "MassTransit.AmazonSQS";
        public const string MassTransitAzureServiceBusCorePackageName = "MassTransit.Azure.ServiceBus.Core";
        public const string MassTransitEntityFrameworkCorePackageName = "MassTransit.EntityFrameworkCore";
        public const string MassTransitRabbitMQPackageName = "MassTransit.RabbitMQ";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MassTransitPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.3.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.3.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "6.0.0"),
                        ( >= 2, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.3.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "6.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "6.0.1")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Reflection.Emit", "4.7.0")
                            .WithNugetDependency("System.Reflection.Emit.Lightweight", "4.7.0")
                            .WithNugetDependency("System.Text.Json", "6.0.10")
                            .WithNugetDependency("System.Threading.Channels", "6.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAbstractionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1"),
                        ( >= 6, 0) => new PackageVersion("8.3.1"),
                        ( >= 2, 0) => new PackageVersion("8.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAmazonSQSPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.400.30")
                            .WithNugetDependency("AWSSDK.SQS", "3.7.400.30")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        ( >= 6, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.400.30")
                            .WithNugetDependency("AWSSDK.SQS", "3.7.400.30")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        ( >= 2, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.400.30")
                            .WithNugetDependency("AWSSDK.SQS", "3.7.400.30")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAmazonSQSPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAzureServiceBusCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.2")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        ( >= 6, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.2")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        ( >= 2, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.2")
                            .WithNugetDependency("MassTransit", "8.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAzureServiceBusCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.1"),
                        ( >= 2, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.18"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitRabbitMQPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("RabbitMQ.Client", "6.8.1"),
                        ( >= 6, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("RabbitMQ.Client", "6.8.1"),
                        ( >= 2, 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("MassTransit", "8.3.1")
                            .WithNugetDependency("RabbitMQ.Client", "6.8.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitRabbitMQPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MassTransit(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransitAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransitRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransitAzureServiceBusCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitAzureServiceBusCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransitAmazonSQS(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitAmazonSQSPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransitEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
