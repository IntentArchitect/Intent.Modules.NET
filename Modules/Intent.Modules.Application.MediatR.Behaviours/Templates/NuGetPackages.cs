using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Behaviours.Templates
{
    public class NuGetPackages
    {
        public static INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "12.1.1");
        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => new("Microsoft.Extensions.Logging", GetVersion(outputTarget.GetProject()));

        private static string GetVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(6) => "6.0.0",
                _ when project.IsNetApp(7) => "7.0.0",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => "6.0.0"
            };
        }
    }
}
