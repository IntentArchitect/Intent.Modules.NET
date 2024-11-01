using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.MongoDb
{
    public class NugetPackages : INugetPackages
    {
        public const string FinbuckleMultiTenantPackageName = "Finbuckle.MultiTenant";
        public const string MongoFrameworkPackageName = "MongoFramework";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FinbuckleMultiTenantPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "7.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "6.0.0"),
                        ( >= 2, 0) => new PackageVersion("6.9.1")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "3.1.0")
                            .WithNugetDependency("System.Text.Json", "6.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantPackageName}'"),
                    }
                );
            NugetRegistry.Register(MongoFrameworkPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("0.29.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "7.0.0")
                            .WithNugetDependency("MongoDB.Driver", "2.19.2")
                            .WithNugetDependency("System.ComponentModel.Annotations", "5.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "7.0.0")
                            .WithNugetDependency("System.Linq.Async", "6.0.1"),
                        ( >= 2, 0) => new PackageVersion("0.29.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "7.0.0")
                            .WithNugetDependency("MongoDB.Driver", "2.19.2")
                            .WithNugetDependency("System.ComponentModel.Annotations", "5.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "7.0.0")
                            .WithNugetDependency("System.Linq.Async", "6.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MongoFrameworkPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MongoFramework(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MongoFrameworkPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
