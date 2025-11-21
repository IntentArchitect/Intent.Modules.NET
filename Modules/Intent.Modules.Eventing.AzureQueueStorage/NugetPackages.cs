using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureStorageQueuesPackageName = "Azure.Storage.Queues";
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureStorageQueuesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("12.24.0")
                            .WithNugetDependency("Azure.Core", "1.47.3")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0"),
                        ( >= 2, >= 1) => new PackageVersion("12.24.0")
                            .WithNugetDependency("Azure.Core", "1.47.3")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0"),
                        ( >= 2, >= 0) => new PackageVersion("12.24.0")
                            .WithNugetDependency("Azure.Core", "1.47.3")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureStorageQueuesPackageName}'"),
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

        public static NugetPackageInfo AzureStorageQueues(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureStorageQueuesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}