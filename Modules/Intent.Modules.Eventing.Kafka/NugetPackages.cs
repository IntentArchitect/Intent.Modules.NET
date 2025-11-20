using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka
{
    public class NugetPackages : INugetPackages
    {
        public const string ConfluentKafkaPackageName = "Confluent.Kafka";
        public const string ConfluentSchemaRegistrySerdesJsonPackageName = "Confluent.SchemaRegistry.Serdes.Json";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";

        public void RegisterPackages()
        {
            NugetRegistry.Register(ConfluentKafkaPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("librdkafka.redist", "2.12.0"),
                        ( >= 6, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("librdkafka.redist", "2.12.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("librdkafka.redist", "2.12.0")
                            .WithNugetDependency("System.Buffers", "4.6.1")
                            .WithNugetDependency("System.Memory", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ConfluentKafkaPackageName}'"),
                    }
                );
            NugetRegistry.Register(ConfluentSchemaRegistrySerdesJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("Confluent.Kafka", "2.12.0")
                            .WithNugetDependency("Confluent.SchemaRegistry", "2.12.0")
                            .WithNugetDependency("NJsonSchema.NewtonsoftJson", "11.0.2"),
                        ( >= 6, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("Confluent.Kafka", "2.12.0")
                            .WithNugetDependency("Confluent.SchemaRegistry", "2.12.0")
                            .WithNugetDependency("NJsonSchema", "10.9.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.12.0")
                            .WithNugetDependency("Confluent.Kafka", "2.12.0")
                            .WithNugetDependency("Confluent.SchemaRegistry", "2.12.0")
                            .WithNugetDependency("NJsonSchema", "10.9.0")
                            .WithNugetDependency("System.Net.NameResolution", "4.3.0")
                            .WithNugetDependency("System.Net.Sockets", "4.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ConfluentSchemaRegistrySerdesJsonPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ConfluentKafka(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ConfluentKafkaPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ConfluentSchemaRegistrySerdesJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ConfluentSchemaRegistrySerdesJsonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
