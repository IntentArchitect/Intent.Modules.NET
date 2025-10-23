using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Security.JWT
{
    public class NugetPackages : INugetPackages
    {
        public const string DuendeIdentityModelPackageName = "Duende.IdentityModel";
        public const string IdentityModelPackageName = "IdentityModel";
        public const string MicrosoftAspNetCoreAuthenticationJwtBearerPackageName = "Microsoft.AspNetCore.Authentication.JwtBearer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(DuendeIdentityModelPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("7.0.0", locked: true),
                        ( >= 2, >= 0) => new PackageVersion("7.0.0", locked: true)
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DuendeIdentityModelPackageName}'"),
                    }
                );
            NugetRegistry.Register(IdentityModelPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 7, >= 0) => new PackageVersion("7.0.0", locked: true),
                        ( >= 2, >= 0) => new PackageVersion("7.0.0", locked: true)
                            .WithNugetDependency("System.Text.Json", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.0.1"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.21")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "7.1.2"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication", "2.3.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "5.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreAuthenticationJwtBearerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo DuendeIdentityModel(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DuendeIdentityModelPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
