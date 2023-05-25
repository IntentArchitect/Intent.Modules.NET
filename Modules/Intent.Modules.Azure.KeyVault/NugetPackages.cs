using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.KeyVault;

public static class NugetPackages
{
    public static readonly INugetPackageInfo AzureSecurityKeyVaultSecrets = new NugetPackageInfo("Azure.Security.KeyVault.Secrets", "4.5.0");
    public static readonly INugetPackageInfo AzureExtensionsAspNetCoreConfigurationSecrets = new NugetPackageInfo("Azure.Extensions.AspNetCore.Configuration.Secrets", "1.2.2");
    public static readonly INugetPackageInfo AzureIdentity = new NugetPackageInfo("Azure.Identity", "1.9.0");
}