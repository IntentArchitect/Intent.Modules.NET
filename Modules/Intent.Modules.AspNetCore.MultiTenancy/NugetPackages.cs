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
        public const string MicrosoftExtensionsHostingAbstractionsPackageName = "Microsoft.Extensions.Hosting.Abstractions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FinbuckleMultiTenantPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Http", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "9.0.23"),
                        ( >= 8, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Http", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0"),
                        ( >= 7, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "7.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "7.0.0"),
                        ( >= 6, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Options", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "6.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantPackageName}'"),
                    }
                );
            NugetRegistry.Register(FinbuckleMultiTenantAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Finbuckle.MultiTenant", "9.4.16")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "9.0.23"),
                        ( >= 8, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Finbuckle.MultiTenant", "9.4.16")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "8.0.34"),
                        ( >= 7, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.1.21")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.1.21")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "6.0.36"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(FinbuckleMultiTenantEntityFrameworkCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Finbuckle.MultiTenant", "9.4.16")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "9.0.23")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "9.0.23"),
                        ( >= 8, >= 0) => new PackageVersion("9.4.16")
                            .WithNugetDependency("Finbuckle.MultiTenant", "9.4.16")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "8.0.34")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.34")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.34"),
                        ( >= 7, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.1.21")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "7.0.20")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("8.1.21")
                            .WithNugetDependency("Finbuckle.MultiTenant", "8.1.21")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "6.0.36")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "6.0.36"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FinbuckleMultiTenantEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreInMemoryPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.15")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "10.0.15")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "10.0.15")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.15"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.23")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.23")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.23"),
                        ( >= 6, >= 0) => new PackageVersion("7.0.20"),
                        ( >= 2, >= 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.17"),
                        ( >= 2, >= 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHostingAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "11.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingAbstractionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreInMemoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenantAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FinbuckleMultiTenantEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FinbuckleMultiTenantEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
