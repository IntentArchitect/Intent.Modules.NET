using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.Solace
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SolaceSystemsSolclientMessaging(IOutputTarget outputTarget) => new(
            name: "SolaceSystems.Solclient.Messaging",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "10.25.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
