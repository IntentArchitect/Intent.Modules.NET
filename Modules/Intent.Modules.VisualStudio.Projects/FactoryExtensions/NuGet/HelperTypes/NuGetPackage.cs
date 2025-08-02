using System;
using System.Collections.Generic;
using System.Diagnostics;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
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


    public VersionInfo VersionInfo { get; set; }
    public List<string> PrivateAssets { get; private init; }

    public List<string> IncludeAssets { get; private init; }

    public INugetPackageInfo RequestedPackage { get; private set; }


    public static NuGetPackage Create(string projectPath, [NotNull] INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options, string version)
    {
        ValidatePackageInformation(projectPath, nugetPackageInfo.Name, nugetPackageInfo.Version);

        return new NuGetPackage
        {
            RequestedPackage = nugetPackageInfo,
            Options = options,
            Name = nugetPackageInfo.Name,
            VersionInfo = version != null ? new VersionInfo(version)  : new VersionInfo(nugetPackageInfo.Version),
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
            VersionInfo = new VersionInfo(version),
            IncludeAssets = [..includeAssets ?? []],
            PrivateAssets = [..privateAssets ?? []]
        };
    }

    public void Update(VersionInfo highestVersion, INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options)
    {
        if (options != null)
        {
            Options.Consolidate(options);
        }

        if (VersionInfo < highestVersion)
        {
            VersionInfo = highestVersion;
        }

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

    public NuGetPackage Clone(VersionInfo version = null)
    {
        return new NuGetPackage
        {
            Options = Options,
            VersionInfo = version ?? VersionInfo,
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

internal sealed class VersionInfo : IComparable<VersionInfo>
{
    private readonly string _rawVersion;
    private readonly VersionRange _versionRange;

    public static bool TryParse(string version, out VersionInfo result)
    {
        if (!VersionRange.TryParse(version, out var semanticVersion) && !IsVariableSyntax(version))
        {
            result = null;
            return false;
        }

        result = new VersionInfo(version, semanticVersion);
        return true;
    }

    private VersionInfo(string raw, VersionRange parsedRange)
    {
        _rawVersion = raw;
        _versionRange = parsedRange;
    }

    public VersionInfo(string version)
    {
        _rawVersion = version;
        if (!VersionRange.TryParse(version, out var semanticVersion))
        {
            if (!IsVariableSyntax(version))
            {
                throw new FormatException($"Could not parse version: '{version}'.");
            }

            _versionRange = null; // indicates a variable version
        }
        else
        {
            _versionRange = semanticVersion;
        }
    }

    private static bool IsVariableSyntax(string version)
    {
        return version.TrimStart().StartsWith("$");
    }

    public string OriginalVersion => _rawVersion;

    public bool IsVariable => _versionRange == null;

    public int CompareTo(VersionInfo other)
    {
        if (other == null) return 1;

        if (this.IsVariable && !other.IsVariable) return 1;
        if (!this.IsVariable && other.IsVariable) return -1;
        if (this.IsVariable && other.IsVariable) return 0;

        return _versionRange.MinVersion.CompareTo(other._versionRange.MinVersion);
    }

    public string Version => IsVariable ? _rawVersion : _versionRange.MinVersion.ToString();

    public override string ToString() => Version;

    public override bool Equals(object obj)
    {
        return obj is VersionInfo other && CompareTo(other) == 0;
    }

    public override int GetHashCode()
    {
        return _rawVersion.GetHashCode();
    }

    public static bool operator <(VersionInfo left, VersionInfo right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator >(VersionInfo left, VersionInfo right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator <=(VersionInfo left, VersionInfo right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >=(VersionInfo left, VersionInfo right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    public static bool operator ==(VersionInfo left, VersionInfo right)
    {
        return ReferenceEquals(left, right) || (left is not null && left.Equals(right));
    }

    public static bool operator !=(VersionInfo left, VersionInfo right)
    {
        return !(left == right);
    }

    public static bool operator <(VersionInfo left, VersionRange right)
    {
        if (left is null) return right is not null;
        if (left.IsVariable) return false;
        return left._versionRange.MinVersion < right.MinVersion;
    }

    public static bool operator >(VersionInfo left, VersionRange right)
    {
        if (left is null) return right is not null;
        if (left.IsVariable) return true;
        return left._versionRange.MinVersion > right.MinVersion;
    }

    public static bool operator <=(VersionInfo left, VersionRange right)
    {
        if (left is null) return true;
        if (left.IsVariable) return false;
        return left._versionRange.MinVersion <= right.MinVersion;
    }

    public static bool operator >=(VersionInfo left, VersionRange right)
    {
        if (left is null) return false;
        if (left.IsVariable) return true;
        return left._versionRange.MinVersion >= right.MinVersion;
    }
}