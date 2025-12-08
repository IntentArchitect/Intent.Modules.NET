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
        public const string MicrosoftExtensionsIdentityStoresPackageName = "Microsoft.Extensions.Identity.Stores";
        public const string SystemIdentityModelTokensJwtPackageName = "System.IdentityModel.Tokens.Jwt";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("2.3.1")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.Cookies", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Cryptography.KeyDerivation", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Hosting.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("9.0.11")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.11")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "9.0.11"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.22")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.22")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.22"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "7.0.20"),
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
                        ( >= 9, >= 0) => new PackageVersion("9.0.10", locked: true)
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.10", locked: true)
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Core", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsIdentityStoresPackageName}'"),
                    }
                );
            NugetRegistry.Register(SystemIdentityModelTokensJwtPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Tokens", "8.15.0"),
                        ( >= 9, >= 0) => new PackageVersion("8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Tokens", "8.15.0"),
                        ( >= 8, >= 0) => new PackageVersion("8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Tokens", "8.15.0"),
                        ( >= 6, >= 0) => new PackageVersion("8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Tokens", "8.15.0"),
                        ( >= 2, >= 0) => new PackageVersion("8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.15.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Tokens", "8.15.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SystemIdentityModelTokensJwtPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreIdentity(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsIdentityStores(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsIdentityStoresPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SystemIdentityModelTokensJwt(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemIdentityModelTokensJwtPackageName, outputTarget.GetMaxNetAppVersion());
    }
}