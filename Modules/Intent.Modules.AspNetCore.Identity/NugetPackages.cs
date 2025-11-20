using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName = "Microsoft.AspNetCore.Identity.EntityFrameworkCore";
        public const string MicrosoftExtensionsIdentityStoresPackageName = "Microsoft.Extensions.Identity.Stores";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "10.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("9.0.11")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.11")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "9.0.11"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.22")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.22")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.22"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "6.0.36"),
                        ( >= 2, >= 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "5.0.17"),
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity", "2.3.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "2.1.14")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsIdentityStoresPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsIdentityStoresPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsIdentityStores(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsIdentityStoresPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
