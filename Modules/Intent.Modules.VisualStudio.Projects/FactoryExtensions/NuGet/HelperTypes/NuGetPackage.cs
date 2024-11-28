using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using JetBrains.Annotations;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;

internal class NuGetPackage
{
    private NuGetPackage()
    {
    }

    public NuGetInstallOptions Options { get; set; }

    public string Name { get; set; }

    public VersionRange Version { get; set; }

    public List<string> PrivateAssets { get; private init; }

    public List<string> IncludeAssets { get; private init; }

    public INugetPackageInfo RequestedPackage { get; private set; }

    public static NuGetPackage Create(string projectPath, [NotNull] INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options, VersionRange version = null)
    {
        ValidatePackageInformation(projectPath, nugetPackageInfo.Name, nugetPackageInfo.Version);

        return new NuGetPackage
        {
            RequestedPackage = nugetPackageInfo,
            Options = options,
            Name = nugetPackageInfo.Name,
            Version = version ?? VersionRange.Parse(nugetPackageInfo.Version),
            IncludeAssets = [..nugetPackageInfo.IncludeAssets ?? []],
            PrivateAssets = [..nugetPackageInfo.PrivateAssets ?? []]
        };
    }

    public static NuGetPackage Create(string projectPath, string packageName, string version, IEnumerable<string> includeAssets, IEnumerable<string> privateAssets)
    {
        ValidatePackageInformation(projectPath, packageName, version);

        return new NuGetPackage
        {
            Options = new NuGetInstallOptions(),
            Name = packageName,
            Version = VersionRange.TryParse(version, out var parsed)
                ? parsed
                : null,
            IncludeAssets = [..includeAssets ?? []],
            PrivateAssets = [..privateAssets ?? []]
        };
    }

    public void Update(VersionRange highestVersion, INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options)
    {
        if (options != null)
        {
            Options.Consolidate(options);
        }
        
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
            Options = Options,
            Version = version ?? Version,
            PrivateAssets = [..PrivateAssets],
            IncludeAssets = [..IncludeAssets]
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