using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Settings;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;

internal class UnsupportedSchemeProcessor : INuGetSchemeProcessor
{
    public Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode)
    {
        return new Dictionary<string, NuGetPackage>();
    }

    public string InstallPackages(
        string projectPath,
        string projectContent,
        Dictionary<string, NuGetPackage> requestedPackages,
        Dictionary<string, NuGetPackage> installedPackages,
        List<string> toRemovePackages,
        string projectName,
        ITracing tracing,
        DependencyVersionOverwriteBehaviorOption dependencyVersionOverwriteBehavior)
    {
        tracing.Debug($"Skipped processing project '{projectName}' as its type is unsupported.");

        return projectContent;
    }
}