using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HashiCorp.Vault
{
    public static class NugetPackages
    {

        public static NugetPackageInfo VaultSharp(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "VaultSharp",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "1.13.0.1",
                (>= 6, 0) => "1.13.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'VaultSharp'")
            });
    }
}
