using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.KeyVault
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureSecurityKeyVaultSecrets(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Security.KeyVault.Secrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "4.6.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Security.KeyVault.Secrets'")
            });

        public static NugetPackageInfo AzureExtensionsAspNetCoreConfigurationSecrets(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Extensions.AspNetCore.Configuration.Secrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "1.3.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Extensions.AspNetCore.Configuration.Secrets'")
            });

        public static NugetPackageInfo AzureIdentity(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Identity",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "1.12.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Identity'")
            });
    }
}
