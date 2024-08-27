using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Security.MSAL
{
    public class NugetPackages : INugetPackages
    {
        public const string IdentityModelPackageName = "IdentityModel";
        public const string MicrosoftAspNetCoreAuthenticationJwtBearerPackageName = "Microsoft.AspNetCore.Authentication.JwtBearer";
        public const string MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName = "Microsoft.AspNetCore.Authentication.OpenIdConnect";
        public const string MicrosoftIdentityWebPackageName = "Microsoft.Identity.Web";
        public const string MicrosoftIdentityWebMicrosoftGraphPackageName = "Microsoft.Identity.Web.MicrosoftGraph";
        public const string MicrosoftIdentityWebUIPackageName = "Microsoft.Identity.Web.UI";

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
            NugetRegistry.Register(MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.0.1"),
                        ( >= 7, 0) => new PackageVersion("3.0.1"),
                        ( >= 6, 0) => new PackageVersion("3.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftIdentityWebPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebMicrosoftGraphPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.0.1"),
                        ( >= 7, 0) => new PackageVersion("3.0.1"),
                        ( >= 6, 0) => new PackageVersion("3.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftIdentityWebMicrosoftGraphPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebUIPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.0.1"),
                        ( >= 7, 0) => new PackageVersion("3.0.1"),
                        ( >= 6, 0) => new PackageVersion("3.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftIdentityWebUIPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreAuthenticationJwtBearerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftIdentityWeb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftIdentityWebPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftIdentityWebMicrosoftGraphPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftIdentityWebUI(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftIdentityWebUIPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
