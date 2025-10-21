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
                        ( >= 9, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10"),
                        ( >= 2, >= 1) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.10"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.10"),
                        ( >= 2, >= 1) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.10"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.10")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
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
