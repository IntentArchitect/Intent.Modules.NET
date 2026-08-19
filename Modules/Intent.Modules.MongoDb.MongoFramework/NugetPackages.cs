using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.MongoDb.MongoFramework
{
    public class NugetPackages : INugetPackages
    {
        public const string FinbuckleMultiTenantPackageName = "Finbuckle.MultiTenant";
        public const string MongoFrameworkPackageName = "MongoFramework";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FinbuckleMultiTenantPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.4.10"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantPackageName}'"),
                    }
                );
            NugetRegistry.Register(MongoFrameworkPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                {
                    ( >= 6, >= 0) => new PackageVersion("0.29.0")
                        .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "7.0.0")
                        .WithNugetDependency("MongoDB.Driver", "2.19.2")
                        .WithNugetDependency("System.ComponentModel.Annotations", "5.0.0")
                        .WithNugetDependency("System.Configuration.ConfigurationManager", "7.0.0")
                        .WithNugetDependency("System.Linq.Async", "6.0.1"),
                    ( >= 2, >= 1) => new PackageVersion("0.29.0")
                        .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "7.0.0")
                        .WithNugetDependency("MongoDB.Driver", "2.19.2")
                        .WithNugetDependency("System.ComponentModel.Annotations", "5.0.0")
                        .WithNugetDependency("System.Configuration.ConfigurationManager", "7.0.0")
                        .WithNugetDependency("System.Linq.Async", "6.0.1"),
                    ( >= 2, >= 0) => new PackageVersion("0.29.0")
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
