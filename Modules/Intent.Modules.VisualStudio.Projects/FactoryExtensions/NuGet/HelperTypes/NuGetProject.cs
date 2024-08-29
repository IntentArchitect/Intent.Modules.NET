using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;

internal class NuGetProject
{
    public string SolutionModelId { get; set; }
    public string ProjectId { get; set; }
    public IEnumerable<IStereotype> ProjectStereotypes { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public Dictionary<string, NuGetPackage> RequestedPackages { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, NuGetPackage> InstalledPackages { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, VersionRange> HighestVersions { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string FilePath { get; set; }
    public INuGetSchemeProcessor Processor { get; set; }
    public IOutputTarget OutputTarget { get; set; }

    public Dictionary<string, NuGetPackage> GetConsolidatedPackages()
    {
        var allPackages = RequestedPackages.ToList();
        allPackages.AddRange(InstalledPackages);

        return allPackages
            .GroupBy(x => x.Key, x => x.Value)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Version?.MinVersion).First());
    }
}