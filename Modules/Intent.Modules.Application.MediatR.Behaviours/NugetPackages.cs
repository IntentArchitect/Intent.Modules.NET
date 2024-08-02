using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Behaviours
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "12.1.1",
                _ => "12.4.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Logging",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
