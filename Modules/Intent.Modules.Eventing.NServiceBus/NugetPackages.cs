using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus
{
    public class NugetPackages : INugetPackages
    {
        public const string NServiceBusPackageName = "NServiceBus";
        public const string NServiceBusAmazonSQSPackageName = "NServiceBus.AmazonSQS";
        public const string NServiceBusExtensionsHostingPackageName = "NServiceBus.Extensions.Hosting";
        public const string NServiceBusRabbitMQPackageName = "NServiceBus.RabbitMQ";
        public const string NServiceBusTransportAzureServiceBusPackageName = "NServiceBus.Transport.AzureServiceBus";
        public const string NServiceBusTransportSqlServerPackageName = "NServiceBus.Transport.SqlServer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(NServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("8.2.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusAmazonSQSPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("8.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusAmazonSQSPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusRabbitMQPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusTransportAzureServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("6.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusTransportAzureServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusTransportSqlServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusTransportSqlServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo NServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusAmazonSQS(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusAmazonSQSPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusTransportAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusTransportAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusTransportSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusTransportSqlServerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}