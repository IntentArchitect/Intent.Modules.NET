using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.Solace
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SolaceSystemsSolclientMessaging(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "SolaceSystems.Solclient.Messaging",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "10.25.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'SolaceSystems.Solclient.Messaging'")
            });

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Hosting'")
            });
    }
}
