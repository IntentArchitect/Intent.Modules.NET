using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy
{
    public class NugetPackages : INugetPackages
    {
        public const string FinbuckleMultiTenantPackageName = "Finbuckle.MultiTenant";
        public const string FinbuckleMultiTenantAspNetCorePackageName = "Finbuckle.MultiTenant.AspNetCore";
        public const string FinbuckleMultiTenantEntityFrameworkCorePackageName = "Finbuckle.MultiTenant.EntityFrameworkCore";
        public const string MicrosoftEntityFrameworkCoreInMemoryPackageName = "Microsoft.EntityFrameworkCore.InMemory";

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
            NugetRegistry.Register(FinbuckleMultiTenantAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "8.0.1"),
                        ( >= 7, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "7.0.15"),
                        ( >= 6, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "6.0.26"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(FinbuckleMultiTenantEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "8.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "7.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "7.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "6.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "6.0.0"),
                        ( >= 2, 0) => new PackageVersion("6.9.1")
                            .WithNugetDependency("Finbuckle.MultiTenant", "6.9.1")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "3.1.0")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "3.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreInMemoryPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreInMemoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenantAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenantEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
