using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Security.JWT
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Authentication.JwtBearer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Authentication.JwtBearer'")
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
