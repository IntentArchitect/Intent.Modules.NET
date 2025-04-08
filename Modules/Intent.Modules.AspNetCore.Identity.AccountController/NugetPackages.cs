using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController
{
    public class NugetPackages : INugetPackages
    {
        public const string IdentityModelPackageName = "IdentityModel";
        public const string MicrosoftAspNetCoreAuthenticationJwtBearerPackageName = "Microsoft.AspNetCore.Authentication.JwtBearer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(IdentityModelPackageName,
                (framework) => framework switch
                    {
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.0.1"),
                        ( >= 8, 0) => new PackageVersion("8.0.14")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "7.1.2"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0"),
                        ( >= 2, 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication", "2.3.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "5.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreAuthenticationJwtBearerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
