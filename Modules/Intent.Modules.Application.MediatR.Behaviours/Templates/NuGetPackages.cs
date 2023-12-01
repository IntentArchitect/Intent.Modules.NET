using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Behaviours.Templates;

public class NuGetPackages
{
    public static readonly INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "12.1.1");
    public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.Logging",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
}