using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.KeyVault
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureSecurityKeyVaultSecrets(IOutputTarget outputTarget) => new(
            name: "Azure.Security.KeyVault.Secrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.6.0",
            });

        public static NugetPackageInfo AzureExtensionsAspNetCoreConfigurationSecrets(IOutputTarget outputTarget) => new(
            name: "Azure.Extensions.AspNetCore.Configuration.Secrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.3.1",
            });

        public static NugetPackageInfo AzureIdentity(IOutputTarget outputTarget) => new(
            name: "Azure.Identity",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.12.0",
            });
    }
}
