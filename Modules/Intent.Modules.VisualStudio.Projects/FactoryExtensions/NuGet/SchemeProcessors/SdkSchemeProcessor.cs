using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Templates.DirectoryPackagesProps;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;

internal class SdkSchemeProcessor : INuGetSchemeProcessor
{
    private readonly Dictionary<string, ICpmTemplate> _directoryPackagesPropsTemplatesBySolutionId;
    private readonly ISoftwareFactoryEventDispatcher _softwareFactoryEventDispatcher;

    public SdkSchemeProcessor(IApplication application, ISoftwareFactoryEventDispatcher softwareFactoryEventDispatcher)
    {
        _softwareFactoryEventDispatcher = softwareFactoryEventDispatcher;
        _directoryPackagesPropsTemplatesBySolutionId = application.GetApplicationTemplates()
            .OfType<ICpmTemplate>()
            .ToDictionary(x => x.SolutionModelId);
    }

    public Dictionary<string, NuGetPackage> GetInstalledPackages(
        string solutionModelId,
        string projectPath,
        XNode xNode)
    {
        var project = ProjectRootElement.Create(XmlReader.Create(new StringReader(xNode.ToString())));

        var packageReferenceElements = project.Items.Where(x => string.Equals(x.ItemType, "PackageReference", StringComparison.OrdinalIgnoreCase));

        return packageReferenceElements
            .GroupBy(pr => pr.Include, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var packageReference = group.First();
                    if (!_directoryPackagesPropsTemplatesBySolutionId.TryGetValue(solutionModelId, out var template) ||
                        !template.TryGetVersion(group.Key, out var version))
                    {
                        version = packageReference.Metadata.SingleOrDefault(x => string.Equals(x.Name, "Version", StringComparison.OrdinalIgnoreCase))?.Value;
                    }

                    var privateAssetsMetadata = packageReference.Metadata.SingleOrDefault(x => string.Equals(x.Name, "PrivateAssets", StringComparison.OrdinalIgnoreCase));
                    var privateAssets = privateAssetsMetadata != null
                        ? privateAssetsMetadata.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : Array.Empty<string>();

                    var includeAssetsElement = packageReference.Metadata.SingleOrDefault(x => string.Equals(x.Name, "IncludeAssets", StringComparison.OrdinalIgnoreCase));
                    var includeAssets = includeAssetsElement != null
                        ? includeAssetsElement.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : Array.Empty<string>();

                    return NuGetPackage.Create(projectPath, packageReference.Include, version, includeAssets, privateAssets);
                });
    }

    public string InstallPackages(
        string solutionModelId,
        IEnumerable<IStereotype> projectStereotypes,
        string projectPath,
        string projectContent,
        Dictionary<string, NuGetPackage> requestedPackages,
        Dictionary<string, NuGetPackage> installedPackages,
        List<string> toRemovePackages,
        string projectName,
        ITracing tracing,
        DependencyVersionOverwriteBehaviorOption dependencyVersionOverwriteBehavior)
    {
        var project = ProjectRootElement.Create(
            XmlReader.Create(new StringReader(projectContent)),
            ProjectCollection.GlobalProjectCollection,
            preserveFormatting: true);

        ApplyProjectCpmSettings(projectStereotypes, project, out var projectCpmSetting);

        var cpmTemplate = _directoryPackagesPropsTemplatesBySolutionId[solutionModelId];
        var isUsingCpm = projectCpmSetting != false &&
                         (projectCpmSetting == true || cpmTemplate.CanRunTemplate());

        foreach (var packageId in toRemovePackages)
        {
            var item = project.Items.FirstOrDefault(x => x.ItemType == "PackageReference" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                continue;
            }

            var parent = item.Parent;
            parent.RemoveChild(item);

            if (parent.Count != 0)
            {
                continue;
            }

            parent.Parent.RemoveChild(parent);
        }

        foreach (var (packageId, package) in requestedPackages)
        {
            var existingProjectReference = project.Items
                .FirstOrDefault(x => x.ItemType == "ProjectReference" &&
                                     x.Include.EndsWith($"\\{packageId}.csproj", StringComparison.OrdinalIgnoreCase));
            if (existingProjectReference != null)
            {
                continue;
            }

            var requestedVersion = package.Version;

            var packageReferenceItem = project.Items.FirstOrDefault(x => x.ItemType == "PackageReference" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase));
            if (packageReferenceItem == null)
            {
                tracing.Info($"Installing {packageId} {requestedVersion} into project {projectName}");
                packageReferenceItem = project.AddItem("PackageReference", packageId);
            }

            var currentVersions = new List<VersionRange>(2);
            var projectVersionMetadata = packageReferenceItem.Metadata.SingleOrDefault(x => string.Equals(x.Name, "Version", StringComparison.OrdinalIgnoreCase));
            if (projectVersionMetadata != null)
            {
                currentVersions.Add(VersionRange.Parse(projectVersionMetadata.Value));
            }

            if (cpmTemplate.TryGetVersion(packageId, out var cpmVersion))
            {
                currentVersions.Add(VersionRange.Parse(cpmVersion));
            }

            var currentVersion = currentVersions.Any()
                ? currentVersions.MaxBy(x => x.MinVersion)
                : null;

            if (!isUsingCpm && projectVersionMetadata == null)
            {
                var toVersion = currentVersion != null && currentVersion.MinVersion > requestedVersion.MinVersion
                    ? currentVersion
                    : requestedVersion;
                projectVersionMetadata = packageReferenceItem.AddMetadata("Version", toVersion.ToShortString(), expressAsAttribute: true);
            }

            switch (dependencyVersionOverwriteBehavior)
            {
                case DependencyVersionOverwriteBehaviorOption.Always:
                case DependencyVersionOverwriteBehaviorOption.IfNewer when currentVersion is not null && requestedVersion.MinVersion > currentVersion.MinVersion:
                case DependencyVersionOverwriteBehaviorOption.Never when currentVersion is null:
                    if (isUsingCpm && cpmTemplate.CanRunTemplate())
                    {
                        tracing.Info($"Updating {packageId} from \"{currentVersion?.ToShortString()}\" to \"{requestedVersion.ToShortString()}\" in \"Directory.Packages.props\" for project \"{projectName}\"");
                        cpmTemplate.SetPackageVersion(packageId, requestedVersion.ToShortString(), _softwareFactoryEventDispatcher);
                    }
                    else if (!isUsingCpm)
                    {
                        tracing.Info($"Updating {packageId} from \"{currentVersion?.ToShortString()}\" to \"{requestedVersion.ToShortString()}\" in project \"{projectName}\"");
                        projectVersionMetadata.Value = requestedVersion.ToShortString();
                    }

                    break;
                case DependencyVersionOverwriteBehaviorOption.IfNewer:
                case DependencyVersionOverwriteBehaviorOption.Never:
                    if (isUsingCpm && cpmTemplate.CanRunTemplate() && !cpmTemplate.TryGetVersion(packageId, out _))
                    {
                        var toVersion = currentVersion ?? requestedVersion;
                        tracing.Info($"Setting {packageId} version to \"{toVersion.ToShortString()}\" in \"Directory.Packages.props\" for project \"{projectName}\"");
                        cpmTemplate.SetPackageVersion(packageId, toVersion.ToShortString(), _softwareFactoryEventDispatcher);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dependencyVersionOverwriteBehavior), dependencyVersionOverwriteBehavior, null);
            }

            if (package.PrivateAssets.Any())
            {
                var privateAssets = packageReferenceItem.Metadata.SingleOrDefault(x => string.Equals(x.Name, "PrivateAssets", StringComparison.OrdinalIgnoreCase)) ??
                                    packageReferenceItem.AddMetadata("PrivateAssets", string.Empty, expressAsAttribute: false);

                privateAssets.Value = string.Join("; ", package.PrivateAssets);
            }

            if (package.IncludeAssets.Any())
            {
                var includeAssets = packageReferenceItem.Metadata.SingleOrDefault(x => string.Equals(x.Name, "IncludeAssets", StringComparison.OrdinalIgnoreCase)) ??
                                    packageReferenceItem.AddMetadata("IncludeAssets", string.Empty, expressAsAttribute: false);

                includeAssets.Value = string.Join("; ", package.IncludeAssets);
            }
        }

        if (isUsingCpm)
        {
            var items = project.Items.Where(x => string.Equals(x.ItemType, "PackageReference", StringComparison.OrdinalIgnoreCase));
            foreach (var item in items)
            {
                var version = item.Metadata.SingleOrDefault(x => string.Equals(x.Name, "Version", StringComparison.OrdinalIgnoreCase));
                if (version == null)
                {
                    continue;
                }

                if (!cpmTemplate.TryGetVersion(item.Include, out var cpmVersion) ||
                    VersionRange.Parse(cpmVersion).MinVersion < VersionRange.Parse(version.Value).MinVersion)
                {
                    cpmTemplate.SetPackageVersion(item.Include, version.Value, _softwareFactoryEventDispatcher);
                }

                version.Parent.RemoveChild(version);
            }
        }

        SortAlphabetically(project);
        return project.RawXml;
    }

    private static void ApplyProjectCpmSettings(
        IEnumerable<IStereotype> projectStereotypes,
        ProjectRootElement projectRootElement,
        out bool? projectCpmSetting)
    {
        var csProjProperty = projectRootElement.Properties.SingleOrDefault(x => string.Equals(x.Name, "ManagePackageVersionsCentrally"));
        var stereotype = projectStereotypes.SingleOrDefault(x => x.Name == ".NET Settings");
        var stereotypePropertyValue = stereotype?.Properties
            .SingleOrDefault(x => x.Key == "Manage Package Versions Centrally")?.Value?.ToLower();

        switch (stereotypePropertyValue)
        {
            case "(unspecified)":
                if (csProjProperty != null)
                {
                    var parent = csProjProperty.Parent;
                    parent.RemoveChild(csProjProperty);
                    if (!parent.Parent.Children.Any())
                    {
                        parent.Parent.RemoveChild(parent);
                    }

                }

                projectCpmSetting = null;
                break;
            case "false":
                csProjProperty ??= projectRootElement.AddProperty("ManagePackageVersionsCentrally", "false");
                csProjProperty.Value = "false";
                projectCpmSetting = false;
                break;
            case "true":
                csProjProperty ??= projectRootElement.AddProperty("ManagePackageVersionsCentrally", "true");
                csProjProperty.Value = "true";
                projectCpmSetting = true;
                break;
            default:
                projectCpmSetting = null;
                break;
        }
    }

    private static void SortAlphabetically(ProjectRootElement project)
    {
        var groups = project.ItemGroups.Where(x => x.Items.Any() &&
                                                   x.Items.All(y => string.Equals(y.ItemType, "PackageReference")));
        foreach (var group in groups)
        {
            // Remove and re-add the elements in alphabetical order
            var items = group.Items.OrderBy(x => x.Include).ToArray();
            group.RemoveAllChildren();

            foreach (var item in items)
            {
                group.AppendChild(item);
            }
        }
    }
}