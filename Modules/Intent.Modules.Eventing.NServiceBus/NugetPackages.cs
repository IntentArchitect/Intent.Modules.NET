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
        public const string NServiceBusPersistenceSqlPackageName = "NServiceBus.Persistence.Sql";
        public const string NServiceBusPersistenceSqlTransactionalSessionPackageName = "NServiceBus.Persistence.Sql.TransactionalSession";
        public const string MicrosoftDataSqlClientPackageName = "Microsoft.Data.SqlClient";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("6.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftDataSqlClientPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.2.5"),
                        ( >= 8,  >= 0) => new PackageVersion("9.2.11"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusAmazonSQSPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.0.1"),
                        ( >= 8,  >= 0) => new PackageVersion("8.1.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusAmazonSQSPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("4.1.0"),
                        ( >= 8,  >= 0) => new PackageVersion("3.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPersistenceSqlPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.0.1"),
                        ( >= 8,  >= 0) => new PackageVersion("8.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPersistenceSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPersistenceSqlTransactionalSessionPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.0.1"),
                        ( >= 8,  >= 0) => new PackageVersion("8.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPersistenceSqlTransactionalSessionPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusRabbitMQPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.2.1"),
                        ( >= 8,  >= 0) => new PackageVersion("9.2.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusTransportAzureServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("6.3.0"),
                        ( >= 8,  >= 0) => new PackageVersion("5.1.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusTransportAzureServiceBusPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo NServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusAmazonSQS(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusAmazonSQSPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusPersistenceSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPersistenceSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusPersistenceSqlTransactionalSession(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPersistenceSqlTransactionalSessionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusTransportAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusTransportAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftDataSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftDataSqlClientPackageName, outputTarget.GetMaxNetAppVersion());
    }
}