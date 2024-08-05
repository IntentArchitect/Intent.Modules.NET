using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HashiCorp.Vault
{
    public static class NugetPackages
    {

        public static NugetPackageInfo VaultSharp(IOutputTarget outputTarget) => new(
            name: "VaultSharp",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.13.0.1",
                _ => "1.13.0.1",
            });
    }
}
