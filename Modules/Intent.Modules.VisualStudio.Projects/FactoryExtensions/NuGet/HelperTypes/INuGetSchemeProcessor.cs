using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Settings;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;

internal interface INuGetSchemeProcessor
{
    Dictionary<string, NuGetPackage> GetInstalledPackages(
        string solutionModelId,
        string projectPath,
        XNode xNode);

    string InstallPackages(
        string solutionModelId,
        IEnumerable<IStereotype> projectStereotypes,
        string projectPath,
        string projectContent,
        Dictionary<string, NuGetPackage> requestedPackages,
        Dictionary<string, NuGetPackage> installedPackages,
        List<string> packagesToRemove,
        string projectName,
        ITracing tracing,
        DependencyVersionOverwriteBehaviorOption dependencyVersionOverwriteBehavior);
}