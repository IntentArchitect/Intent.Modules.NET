using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";
        public const string SolaceSystemsSolclientMessagingPackageName = "SolaceSystems.Solclient.Messaging";

        public void RegisterPackages()
        {
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
            NugetRegistry.Register(SolaceSystemsSolclientMessagingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("10.28.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SolaceSystemsSolclientMessagingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SolaceSystemsSolclientMessaging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SolaceSystemsSolclientMessagingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
