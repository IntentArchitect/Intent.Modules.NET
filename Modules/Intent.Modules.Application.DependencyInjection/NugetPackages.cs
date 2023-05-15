using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.DependencyInjection
{
    public static class NugetPackages
    {
        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new("Microsoft.Extensions.DependencyInjection", GetVersion(outputTarget.GetProject()));

        private static string GetVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(5) => "5.0.2",
                _ when project.IsNetApp(6) => "6.0.1",
                _ when project.IsNetApp(7) => "7.0.0",
                _ => "6.0.1"
            };
        }
    }
}
