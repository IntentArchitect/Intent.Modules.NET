using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Behaviours.Templates
{
    public class NuGetPackages
    {
        public static INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "9.0.*");
        public static INugetPackageInfo MicrosoftExtensionsLogging = new NugetPackageInfo("Microsoft.Extensions.Logging", "3.0.*");
    }
}
