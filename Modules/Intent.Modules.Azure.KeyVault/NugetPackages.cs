using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Azure.KeyVault
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureExtensionsAspNetCoreConfigurationSecretsPackageName = "Azure.Extensions.AspNetCore.Configuration.Secrets";
        public const string AzureIdentityPackageName = "Azure.Identity";
        public const string AzureSecurityKeyVaultSecretsPackageName = "Azure.Security.KeyVault.Secrets";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureExtensionsAspNetCoreConfigurationSecretsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.4.0")
                            .WithNugetDependency("Azure.Core", "1.44.1")
                            .WithNugetDependency("Azure.Security.KeyVault.Secrets", "4.6.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "2.1.0"),
                        ( >= 2, >= 0) => new PackageVersion("1.4.0")
                            .WithNugetDependency("Azure.Core", "1.44.1")
                            .WithNugetDependency("Azure.Security.KeyVault.Secrets", "4.6.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "2.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureExtensionsAspNetCoreConfigurationSecretsPackageName}'"),
                    }
                );
            NugetRegistry.Register(AzureIdentityPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.15.0")
                            .WithNugetDependency("Azure.Core", "1.47.1")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.73.1")
                            .WithNugetDependency("Microsoft.Identity.Client.Extensions.Msal", "4.73.1"),
                        ( >= 2, >= 0) => new PackageVersion("1.15.0")
                            .WithNugetDependency("Azure.Core", "1.47.1")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.73.1")
                            .WithNugetDependency("Microsoft.Identity.Client.Extensions.Msal", "4.73.1")
                            .WithNugetDependency("System.Memory", "4.5.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureIdentityPackageName}'"),
                    }
                );
            NugetRegistry.Register(AzureSecurityKeyVaultSecretsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("4.8.0")
                            .WithNugetDependency("Azure.Core", "1.46.2"),
                        ( >= 2, >= 0) => new PackageVersion("4.8.0")
                            .WithNugetDependency("Azure.Core", "1.46.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureSecurityKeyVaultSecretsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureSecurityKeyVaultSecrets(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureSecurityKeyVaultSecretsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureExtensionsAspNetCoreConfigurationSecrets(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureExtensionsAspNetCoreConfigurationSecretsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AzureIdentity(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureIdentityPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
