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
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAbstractionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAmazonSQSPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAmazonSQSPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitAzureServiceBusCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitAzureServiceBusCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitRabbitMQPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.2.3"),
                        ( >= 7, 0) => new PackageVersion("8.2.1"),
                        ( >= 6, 0) => new PackageVersion("8.2.3"),
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
