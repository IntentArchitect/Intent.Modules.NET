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
            NugetRegistry.Register(MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName,
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
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OAuth", "2.3.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "5.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreAuthenticationOpenIdConnectPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificate", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificateless", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenCache", "3.8.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.8.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Validators", "8.8.0")
                            .WithNugetDependency("System.Formats.Asn1", "9.0.0")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.8.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificate", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificateless", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenCache", "3.8.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.8.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Validators", "8.8.0")
                            .WithNugetDependency("System.Formats.Asn1", "8.0.1")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.8.0")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        ( >= 7, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificate", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificateless", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenCache", "3.8.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.8.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Validators", "8.8.0")
                            .WithNugetDependency("System.Formats.Asn1", "8.0.1")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.8.0")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        ( >= 6, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificate", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificateless", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenCache", "3.8.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.8.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Validators", "8.8.0")
                            .WithNugetDependency("System.Formats.Asn1", "6.0.1")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.8.0")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        ( >= 2, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.AspNetCore.DataProtection", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "2.1.1")
                            .WithNugetDependency("Microsoft.Extensions.Http", "3.1.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificate", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.Certificateless", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenCache", "3.8.3")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.8.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Validators", "8.8.0")
                            .WithNugetDependency("System.Formats.Asn1", "8.0.1")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.8.0")
                            .WithNugetDependency("System.Security.Cryptography.Xml", "4.7.1")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftIdentityWebPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebMicrosoftGraphPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Graph", "4.36.0")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3"),
                        ( >= 8, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Graph", "4.36.0")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3"),
                        ( >= 7, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Graph", "4.36.0")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3"),
                        ( >= 6, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Graph", "4.36.0")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3"),
                        ( >= 2, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Graph", "4.36.0")
                            .WithNugetDependency("Microsoft.Identity.Web.TokenAcquisition", "3.8.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftIdentityWebMicrosoftGraphPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftIdentityWebUIPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web", "3.8.3"),
                        ( >= 8, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web", "3.8.3"),
                        ( >= 7, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web", "3.8.3"),
                        ( >= 6, 0) => new PackageVersion("3.8.3")
                            .WithNugetDependency("Microsoft.Identity.Web", "3.8.3"),
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
