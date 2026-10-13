using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.VisualStudio.Projects.NuGet;

namespace Intent.Modules.VisualStudio.Projects.Templates;

internal enum TargetFrameworksSyncResult
{
    Unchanged,
    Rewritten,

    /// <summary>
    /// The existing value targets platforms (e.g. <c>net10.0-android</c>) and its base frameworks
    /// differ from the model's. It was left as-is, since the model can't express platform targets.
    /// </summary>
    PlatformTargetsPreserved
}

/// <summary>
/// Brings an SDK-style project's <c>TargetFramework</c>/<c>TargetFrameworks</c> in line with the
/// model, without overwriting a value the model can't represent: conditional or multiple
/// declarations, MSBuild property references, or platform-specific monikers.
/// </summary>
internal static class TargetFrameworksSync
{
    public static TargetFrameworksSyncResult Sync(XDocument doc, IReadOnlyCollection<string> modelledFrameworks)
    {
        var (prefix, namespaceManager, _) = doc.GetNamespaceManager();
        var elements = doc
            .XPathSelectElements($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:*", namespaceManager)
            .Where(x => x.Name.LocalName is "TargetFramework" or "TargetFrameworks")
            .ToArray();

        if (elements.Length != 1)
        {
            return TargetFrameworksSyncResult.Unchanged;
        }

        var element = elements[0];
        if (element.Attribute("Condition") != null ||
            element.Parent?.Attribute("Condition") != null ||
            element.Value.Contains("$(", StringComparison.Ordinal))
        {
            return TargetFrameworksSyncResult.Unchanged;
        }

        var existingFrameworks = element.Value
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();

        if (SetEquals(existingFrameworks, modelledFrameworks) ||
            SetEquals(existingFrameworks.Select(GetBaseMoniker), modelledFrameworks))
        {
            return TargetFrameworksSyncResult.Unchanged;
        }

        if (existingFrameworks.Any(HasPlatform))
        {
            return TargetFrameworksSyncResult.PlatformTargetsPreserved;
        }

        var elementName = modelledFrameworks.Count == 1
            ? "TargetFramework"
            : "TargetFrameworks";
        var elementValue = string.Join(";", modelledFrameworks.OrderBy(x => x));

        element.ReplaceWith(XElement.Parse($"<{elementName}>{elementValue}</{elementName}>"));

        return TargetFrameworksSyncResult.Rewritten;
    }

    /// <summary>
    /// <c>net10.0-android35.0</c> becomes <c>net10.0</c>; a moniker without a platform is returned as-is.
    /// </summary>
    private static string GetBaseMoniker(string framework)
    {
        var separatorIndex = framework.IndexOf('-');
        return separatorIndex < 0 ? framework : framework[..separatorIndex];
    }

    private static bool HasPlatform(string framework) => framework.Contains('-');

    private static bool SetEquals(IEnumerable<string> a, IEnumerable<string> b) =>
        new HashSet<string>(a, StringComparer.OrdinalIgnoreCase).SetEquals(b);
}
