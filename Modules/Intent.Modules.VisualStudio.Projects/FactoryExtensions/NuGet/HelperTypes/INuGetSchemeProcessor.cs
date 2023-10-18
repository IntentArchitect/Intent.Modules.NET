using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Settings;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;

internal interface INuGetSchemeProcessor
{
    Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode);

    string InstallPackages(
        string projectPath,
        string projectContent,
        Dictionary<string, NuGetPackage> requestedPackages,
        Dictionary<string, NuGetPackage> installedPackages,
        List<string> packagesToRemove,
        string projectName,
        ITracing tracing,
        DependencyVersionOverwriteBehaviorOption dependencyVersionOverwriteBehavior);
}