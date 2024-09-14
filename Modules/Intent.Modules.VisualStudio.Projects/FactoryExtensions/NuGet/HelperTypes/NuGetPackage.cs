using System;
using System.Collections.Generic;
using Intent.Modules.Common.VisualStudio;
using JetBrains.Annotations;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;

internal class NuGetPackage
{
    private NuGetPackage()
    {
    }

    public VersionRange Version { get; set; }

    public List<string> PrivateAssets { get; private set; }

    public List<string> IncludeAssets { get; private set; }

    public INugetPackageInfo RequestedPackage { get; private set; }

    public static NuGetPackage Create(string projectPath, [NotNull] INugetPackageInfo nugetPackageInfo, VersionRange version = null)
    {
        ValidatePackageInformation(projectPath, nugetPackageInfo.Name, nugetPackageInfo.Version);

        return new NuGetPackage
        {
            RequestedPackage = nugetPackageInfo,
            Version = version ?? VersionRange.Parse(nugetPackageInfo.Version),
            IncludeAssets = new List<string>(nugetPackageInfo.IncludeAssets ?? Array.Empty<string>()),
            PrivateAssets = new List<string>(nugetPackageInfo.PrivateAssets ?? Array.Empty<string>())
        };
    }

    public static NuGetPackage Create(string projectPath, string packageName, string version, IEnumerable<string> includeAssets, IEnumerable<string> privateAssets)
    {
        ValidatePackageInformation(projectPath, packageName, version);

        return new NuGetPackage
        {
            Version = VersionRange.TryParse(version, out var parsed)
                ? parsed
                : null,
            IncludeAssets = new List<string>(includeAssets ?? Array.Empty<string>()),
            PrivateAssets = new List<string>(privateAssets ?? Array.Empty<string>())
        };
    }

    public void Update(VersionRange highestVersion, INugetPackageInfo nugetPackageInfo)
    {
        if (Version.MinVersion < highestVersion.MinVersion) Version = highestVersion;

        if (nugetPackageInfo == null)
        {
            return;
        }
        if (nugetPackageInfo.IncludeAssets != null)
        {
            foreach (var item in nugetPackageInfo.IncludeAssets)
            {
                if (!IncludeAssets.Contains(item)) IncludeAssets.Add(item);
            }
        }

        if (nugetPackageInfo.PrivateAssets != null)
        {
            foreach (var item in nugetPackageInfo.PrivateAssets)
            {
                if (!PrivateAssets.Contains(item)) PrivateAssets.Add(item);
            }
        }
    }

    public NuGetPackage Clone(VersionRange version = null)
    {
        return new NuGetPackage
        {
            Version = version ?? Version,
            PrivateAssets = new List<string>(PrivateAssets),
            IncludeAssets = new List<string>(IncludeAssets)
        };
    }

    private static void ValidatePackageInformation(string projectPath, string packageName, string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new InvalidOperationException($"Version is undefined for Package '{packageName}' in '{projectPath}'");
        }
    }
}