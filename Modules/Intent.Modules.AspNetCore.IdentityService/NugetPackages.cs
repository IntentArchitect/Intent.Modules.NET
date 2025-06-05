using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreIdentityPackageName = "Microsoft.AspNetCore.Identity";
        public const string MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName = "Microsoft.AspNetCore.Identity.EntityFrameworkCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("2.3.1")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.Cookies", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Cryptography.KeyDerivation", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Hosting.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "9.0.5"),
                        ( >= 8, 0) => new PackageVersion("8.0.16")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.16")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.16"),
                        ( >= 7, 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "6.0.36"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity", "2.3.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "2.1.14")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreIdentity(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}