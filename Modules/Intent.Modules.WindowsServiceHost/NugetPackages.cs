using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsConfigurationAbstractionsPackageName = "Microsoft.Extensions.Configuration.Abstractions";
        public const string MicrosoftExtensionsConfigurationBinderPackageName = "Microsoft.Extensions.Configuration.Binder";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";
        public const string MicrosoftExtensionsHostingWindowsServicesPackageName = "Microsoft.Extensions.Hosting.WindowsServices";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsConfigurationAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationBinderPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
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
            NugetRegistry.Register(MicrosoftExtensionsHostingWindowsServicesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("System.ServiceProcess.ServiceController", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("System.ServiceProcess.ServiceController", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("System.ServiceProcess.ServiceController", "10.0.0"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("System.ServiceProcess.ServiceController", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.0")
                            .WithNugetDependency("System.ServiceProcess.ServiceController", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingWindowsServicesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHostingWindowsServices(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingWindowsServicesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
