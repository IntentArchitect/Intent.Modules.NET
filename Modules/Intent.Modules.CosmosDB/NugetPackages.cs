using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.CosmosDB
{
    public class NugetPackages : INugetPackages
    {
        public const string FinbuckleMultiTenantPackageName = "Finbuckle.MultiTenant";
        public const string IEvangelistAzureCosmosRepositoryPackageName = "IEvangelist.Azure.CosmosRepository";
        public const string NewtonsoftJsonPackageName = "Newtonsoft.Json";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FinbuckleMultiTenantPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("6.13.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantPackageName}'"),
                    }
                );
            NugetRegistry.Register(IEvangelistAzureCosmosRepositoryPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.46.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "9.0.0")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.10.0"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.46.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "9.0.0")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.10.0"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Azure.Identity", "1.13.1")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.46.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "9.0.0")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("OpenTelemetry.Extensions.Hosting", "1.10.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IEvangelistAzureCosmosRepositoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(NewtonsoftJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("13.0.3"),
                        ( >= 2, >= 0) => new PackageVersion("13.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NewtonsoftJsonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo NewtonsoftJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NewtonsoftJsonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IEvangelistAzureCosmosRepositoryPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
