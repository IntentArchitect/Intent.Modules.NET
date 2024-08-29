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
        public const string IdentityModelPackageName = "IdentityModel";
        public const string MicrosoftAspNetCoreAuthenticationJwtBearerPackageName = "Microsoft.AspNetCore.Authentication.JwtBearer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(IdentityModelPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreAuthenticationJwtBearerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
