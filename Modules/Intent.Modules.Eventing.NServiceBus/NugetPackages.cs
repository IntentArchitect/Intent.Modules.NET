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
        public const string NServiceBusNHibernatePackageName = "NServiceBus.NHibernate";
        public const string NServiceBusNHibernateTransactionalSessionPackageName = "NServiceBus.NHibernate.TransactionalSession";
        public const string NServiceBusRabbitMQPackageName = "NServiceBus.RabbitMQ";
        public const string NServiceBusTransportAzureServiceBusPackageName = "NServiceBus.Transport.AzureServiceBus";
        public const string NServiceBusTransportSqlServerPackageName = "NServiceBus.Transport.SqlServer";
        public const string NServiceBusPersistenceSqlPackageName = "NServiceBus.Persistence.Sql";
        public const string NServiceBusPersistenceSqlTransactionalSessionPackageName = "NServiceBus.Persistence.Sql.TransactionalSession";
        public const string MicrosoftDataSqlClientPackageName = "Microsoft.Data.SqlClient";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("7.0.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "9.0.13")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.13")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.13")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "9.0.13"),
                        ( >= 8, >= 0) => new PackageVersion("7.0.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1"),
                        ( >= 2, >= 0) => new PackageVersion("7.0.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "1.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1")
                            .WithNugetDependency("System.Text.Json", "10.0.3")
                            .WithNugetDependency("System.Threading.Channels", "10.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftDataSqlClientPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.2.6")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.9")
                            .WithNugetDependency("NServiceBus.MessageInterfaces", "1.0.0")
                            .WithNugetDependency("System.IO.Hashing", "10.0.9"),
                        ( >= 8, >= 0) => new PackageVersion("9.2.12")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "8.0.0")
                            .WithNugetDependency("NServiceBus.MessageInterfaces", "1.0.0"),
                        ( >= 6, >= 0) => new PackageVersion("8.2.7")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "7.0.0")
                            .WithNugetDependency("NServiceBus.MessageInterfaces", "1.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "7.0.2")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        ( >= 2, >= 0) => new PackageVersion("7.8.6")
                            .WithNugetDependency("System.Reflection.Emit", "4.7.0")
                            .WithNugetDependency("System.Reflection.Emit.Lightweight", "4.7.0")
                            .WithNugetDependency("System.Reflection.Metadata", "1.8.1")
                            .WithNugetDependency("System.Security.Cryptography.Xml", "4.7.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusAmazonSQSPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.1.1")
                            .WithNugetDependency("AWSSDK.S3", "4.0.24.3")
                            .WithNugetDependency("AWSSDK.SecurityToken", "4.0.7.3")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "4.0.3.3")
                            .WithNugetDependency("AWSSDK.SQS", "4.0.3.3")
                            .WithNugetDependency("BitFaster.Caching", "2.6.0")
                            .WithNugetDependency("NServiceBus", "10.2.5"),
                        ( >= 8, >= 0) => new PackageVersion("8.1.2")
                            .WithNugetDependency("AWSSDK.S3", "4.0.17.1")
                            .WithNugetDependency("AWSSDK.SecurityToken", "4.0.5.6")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "4.0.2.13")
                            .WithNugetDependency("AWSSDK.SQS", "4.0.2.11")
                            .WithNugetDependency("BitFaster.Caching", "2.5.3")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("6.2.2")
                            .WithNugetDependency("AWSSDK.S3", "3.7.103.21")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.101.20")
                            .WithNugetDependency("AWSSDK.SQS", "3.7.100.83")
                            .WithNugetDependency("BitFaster.Caching", "2.1.1")
                            .WithNugetDependency("NServiceBus", "8.2.7"),
                        ( >= 2, >= 0) => new PackageVersion("5.7.3")
                            .WithNugetDependency("AWSSDK.S3", "3.7.103.2")
                            .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.101.3")
                            .WithNugetDependency("AWSSDK.SQS", "3.7.100.67")
                            .WithNugetDependency("NServiceBus", "7.2.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusAmazonSQSPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("4.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.8")
                            .WithNugetDependency("NServiceBus", "10.2.0"),
                        ( >= 8, >= 0) => new PackageVersion("3.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "8.0.1")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("2.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "6.0.1")
                            .WithNugetDependency("NServiceBus", "8.2.7"),
                        ( >= 2, >= 0) => new PackageVersion("1.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "3.1.6")
                            .WithNugetDependency("NServiceBus", "7.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusNHibernatePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.1.0")
                            .WithNugetDependency("NServiceBus", "10.1.4"),
                        ( >= 8, >= 0) => new PackageVersion("10.1.2")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("NServiceBus", "8.2.7"),
                        ( >= 0, >= 0) => new PackageVersion("8.6.4")
                            .WithNugetDependency("NServiceBus", "7.8.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusNHibernatePackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusNHibernateTransactionalSessionPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.1.0")
                            .WithNugetDependency("NServiceBus.NHibernate", "11.1.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.1.2")
                            .WithNugetDependency("NServiceBus.NHibernate", "10.1.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusNHibernateTransactionalSessionPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPersistenceSqlPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.4")
                            .WithNugetDependency("NServiceBus", "10.1.4"),
                        ( >= 8, >= 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("7.0.8")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("NServiceBus", "8.2.7"),
                        ( >= 2, >= 0) => new PackageVersion("6.6.5")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.1")
                            .WithNugetDependency("NServiceBus", "7.8.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPersistenceSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusPersistenceSqlTransactionalSessionPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("NServiceBus.Persistence.Sql", "9.0.1")
                            .WithNugetDependency("NServiceBus.TransactionalSession", "4.0.2"),
                        ( >= 8, >= 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("NServiceBus.Persistence.Sql", "8.3.1")
                            .WithNugetDependency("NServiceBus.TransactionalSession", "3.4.1"),
                        ( >= 6, >= 0) => new PackageVersion("7.0.8")
                            .WithNugetDependency("NServiceBus.Persistence.Sql", "7.0.8")
                            .WithNugetDependency("NServiceBus.TransactionalSession", "2.0.4"),
                        ( >= 2, >= 0) => new PackageVersion("6.6.5")
                            .WithNugetDependency("NServiceBus.Persistence.Sql", "6.6.5")
                            .WithNugetDependency("NServiceBus.TransactionalSession", "1.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPersistenceSqlTransactionalSessionPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusRabbitMQPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.2.1")
                            .WithNugetDependency("BitFaster.Caching", "2.5.4")
                            .WithNugetDependency("NServiceBus", "10.1.4")
                            .WithNugetDependency("RabbitMQ.Client", "7.2.1"),
                        ( >= 8, >= 0) => new PackageVersion("10.1.8")
                            .WithNugetDependency("BitFaster.Caching", "2.5.3")
                            .WithNugetDependency("NServiceBus", "9.2.11")
                            .WithNugetDependency("RabbitMQ.Client", "7.1.2"),
                        ( >= 6, >= 0) => new PackageVersion("8.0.10")
                            .WithNugetDependency("BitFaster.Caching", "2.0.0")
                            .WithNugetDependency("NServiceBus", "8.2.7")
                            .WithNugetDependency("RabbitMQ.Client", "6.4.0"),
                        ( >= 2, >= 0) => new PackageVersion("7.0.7")
                            .WithNugetDependency("BitFaster.Caching", "2.0.0")
                            .WithNugetDependency("NServiceBus", "7.7.3")
                            .WithNugetDependency("RabbitMQ.Client", "6.4.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusTransportAzureServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("6.4.1")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.20.1")
                            .WithNugetDependency("BitFaster.Caching", "2.6.0")
                            .WithNugetDependency("NServiceBus", "10.2.6"),
                        ( >= 8, >= 0) => new PackageVersion("5.1.3")
                            .WithNugetDependency("Azure.Core", "1.45.0")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.3")
                            .WithNugetDependency("BitFaster.Caching", "2.5.3")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("3.2.9")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.18.2")
                            .WithNugetDependency("BitFaster.Caching", "2.5.2")
                            .WithNugetDependency("NServiceBus", "8.2.7"),
                        ( >= 2, >= 0) => new PackageVersion("2.0.8")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.11.1")
                            .WithNugetDependency("BitFaster.Caching", "2.5.2")
                            .WithNugetDependency("NServiceBus", "7.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusTransportAzureServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusTransportSqlServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("9.1.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "6.1.5")
                            .WithNugetDependency("NServiceBus", "10.2.5"),
                        ( >= 8, >= 0) => new PackageVersion("8.1.13")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "5.2.2")
                            .WithNugetDependency("NServiceBus", "9.2.11"),
                        ( >= 6, >= 0) => new PackageVersion("7.0.13")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "3.1.7")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "6.36.0")
                            .WithNugetDependency("NServiceBus", "8.2.7")
                            .WithNugetDependency("System.Drawing.Common", "4.7.3")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "6.36.0"),
                        ( >= 2, >= 0) => new PackageVersion("6.3.8")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "3.1.5")
                            .WithNugetDependency("NServiceBus", "7.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusTransportSqlServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo NServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusAmazonSQS(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusAmazonSQSPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusNHibernate(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusNHibernatePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusNHibernateTransactionalSession(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusNHibernateTransactionalSessionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusPersistenceSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPersistenceSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusPersistenceSqlTransactionalSession(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPersistenceSqlTransactionalSessionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusTransportAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusTransportAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusTransportSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusTransportSqlServerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftDataSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftDataSqlClientPackageName, outputTarget.GetMaxNetAppVersion());
    }
}