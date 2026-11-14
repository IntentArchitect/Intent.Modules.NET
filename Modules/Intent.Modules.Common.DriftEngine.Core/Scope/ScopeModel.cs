using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Scope;

/// <summary>A glob as the File Classification matcher reads it: <c>**</c> crosses folders, <c>*</c> and <c>?</c> don't.</summary>
internal sealed class GlobPattern
{
    private readonly Regex _regex;

    public GlobPattern(string pattern)
    {
        Pattern = pattern;
        var regex = new StringBuilder("^");
        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            if (c == '*' && i + 1 < pattern.Length && pattern[i + 1] == '*')
            {
                i++;
                if (i + 1 < pattern.Length && pattern[i + 1] == '/')
                {
                    i++;
                    regex.Append("(?:.*/)?");
                }
                else
                {
                    regex.Append(".*");
                }
            }
            else
            {
                regex.Append(c switch { '*' => "[^/]*", '?' => "[^/]", _ => Regex.Escape(c.ToString()) });
            }
        }

        _regex = new Regex(regex.Append('$').ToString(), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    }

    public string Pattern { get; }

    public bool IsMatch(string path) => _regex.IsMatch(path);
}

/// <summary>One row of a <c>Drift Scope</c> stereotype's Entries (scope.md).</summary>
/// <param name="In">The files it matches are read: In or Context.</param>
/// <param name="Context">Read for context only: rules read the code, but it is never reported as code the model doesn't describe.</param>
internal sealed record ScopeRow(string Host, int Number, string BasePath, IReadOnlyList<GlobPattern> Globs, IReadOnlyList<string> Types, bool In, string? Reason, bool Context = false)
{
    public string Id => $"{Host}#{Number}";

    public string Scope => Context ? "Context" : In ? "In" : "Out";

    public bool Matches(string path)
    {
        if (BasePath.Length > 0 && !path.StartsWith(BasePath + "/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var relative = BasePath.Length == 0 ? path : path[(BasePath.Length + 1)..];
        return Globs.Count == 0 || Globs.Any(x => x.IsMatch(relative));
    }
}

/// <summary>Where a file stands: in or out, and what decided it.</summary>
/// <param name="In">The file is read: an In or a Context row decided it.</param>
internal sealed record ScopeDecision(bool In, ScopeRow? Row, string? Builtin)
{
    /// <summary>Read, and held to the model: an In row decided it, not a Context one.</summary>
    public bool Checked => In && Row is not { Context: true };
}

/// <summary>
/// Scope on the Codebase Structure designer (decision 1, scope.md): the deepest folder with a matching row decides a
/// file, the last matching row within it wins, and a file no row matches is out.
/// </summary>
internal sealed class ScopeModel
{
    private static readonly string[] BuiltinFolders = [".git", "bin", "obj", "node_modules", "Intent.Metadata"];

    private readonly IReadOnlyList<string> _solutionFolders;

    private ScopeModel(IReadOnlyList<ScopeRow> rows, IReadOnlyList<string> solutionFolders)
    {
        Rows = rows;
        _solutionFolders = solutionFolders;
    }

    public IReadOnlyList<ScopeRow> Rows { get; }

    /// <param name="codebaseStructure">The Codebase Structure designer's packages.</param>
    /// <param name="files">Every file under the output folder, so the folders holding an Intent solution are known.</param>
    public static ScopeModel From(IReadOnlyList<IPackage>? codebaseStructure, IEnumerable<string> files)
    {
        // A folder holding an .isln is Intent's own, with everything under it (scope.md, Built-in exclusions).
        var solutionFolders = files.Where(x => x.EndsWith(".isln", StringComparison.OrdinalIgnoreCase))
            .Select(x => Path.GetDirectoryName(x)!.Replace('\\', '/'))
            .ToList();
        var rows = new List<ScopeRow>();
        foreach (var package in codebaseStructure ?? [])
        {
            AddRows(rows, package.Id, "", package.Stereotypes);
            foreach (var element in package.ChildElements)
            {
                AddFolder(rows, element, "");
            }
        }

        return new ScopeModel(rows, solutionFolders);
    }

    public ScopeDecision Decide(string path)
    {
        // A solution at the root of the output folder can't make the whole codebase Intent's own.
        if (_solutionFolders.Any(x => x.Length > 0 && path.StartsWith(x + "/", StringComparison.OrdinalIgnoreCase)))
        {
            return new ScopeDecision(false, null, "Intent solution");
        }

        var segments = path.Split('/');
        foreach (var segment in segments[..^1])
        {
            if (BuiltinFolders.Contains(segment, StringComparer.OrdinalIgnoreCase))
            {
                return new ScopeDecision(false, null, segment);
            }

            if (segment.StartsWith('.'))
            {
                return new ScopeDecision(false, null, "hidden folders");
            }
        }

        // Deepest host first; within a host, the last matching row wins.
        var decider = Rows
            .Where(x => x.Types.Count == 0 && x.Matches(path))
            .GroupBy(x => x.Host)
            .OrderByDescending(x => x.First().BasePath.Length)
            .Select(x => x.Last())
            .FirstOrDefault();
        return decider is null ? new ScopeDecision(false, null, null) : new ScopeDecision(decider.In, decider, null);
    }

    /// <summary>A row that excludes a type by name in an otherwise in-scope file (R26).</summary>
    public ScopeRow? ExcludesType(string path, string typeName) =>
        Rows.LastOrDefault(x => !x.In && x.Types.Contains(typeName, StringComparer.Ordinal) && x.Matches(path));

    private static void AddFolder(List<ScopeRow> rows, IElement element, string parentPath)
    {
        var path = parentPath.Length == 0 ? element.Name : $"{parentPath}/{element.Name}";
        AddRows(rows, element.Id, path, element.Stereotypes);
        foreach (var child in element.ChildElements)
        {
            AddFolder(rows, child, path);
        }
    }

    private static void AddRows(List<ScopeRow> rows, string host, string basePath, IEnumerable<IStereotype>? stereotypes)
    {
        // Entries is an item list: Intent hands each row over as a stereotype of its own.
        var entries = stereotypes?.FirstOrDefault(x => x.Name == "Drift Scope")?.Properties.FirstOrDefault(x => x.Key == "Entries");
        if (entries is not IStereotypeProperty<IStereotype[]> { Value: { } items })
        {
            return;
        }

        var number = 0;
        foreach (var item in items)
        {
            number++;
            var scope = Text(item, "Scope");
            rows.Add(new ScopeRow(
                host,
                number,
                basePath,
                Lines(item, "Glob").Select(x => new GlobPattern(x)).ToList(),
                Lines(item, "Types").ToList(),
                !string.Equals(scope, "Out", StringComparison.OrdinalIgnoreCase),
                Text(item, "Reason") is { Length: > 0 } reason ? reason : null,
                string.Equals(scope, "Context", StringComparison.OrdinalIgnoreCase)));
        }
    }

    private static string? Text(IStereotype item, string key) => item.Properties.FirstOrDefault(x => x.Key == key)?.Value;

    /// <summary>A text property holding one value per line.</summary>
    private static IEnumerable<string> Lines(IStereotype item, string key) =>
        Text(item, key)?.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
}
