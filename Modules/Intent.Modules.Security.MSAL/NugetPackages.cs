using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Security.MSAL
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Authentication.OpenIdConnect",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Authentication.OpenIdConnect'")
            });

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Authentication.JwtBearer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Authentication.JwtBearer'")
            });

        public static NugetPackageInfo MicrosoftIdentityWeb(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Identity.Web",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.0.1",
                (>= 7, 0) => "3.0.1",
                (>= 6, 0) => "3.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Identity.Web'")
            });

        public static NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Identity.Web.MicrosoftGraph",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.0.1",
                (>= 7, 0) => "3.0.1",
                (>= 6, 0) => "3.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Identity.Web.MicrosoftGraph'")
            });

        public static NugetPackageInfo MicrosoftIdentityWebUI(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Identity.Web.UI",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.0.1",
                (>= 7, 0) => "3.0.1",
                (>= 6, 0) => "3.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Identity.Web.UI'")
            });

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "IdentityModel",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "7.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'IdentityModel'")
            });
    }
}
