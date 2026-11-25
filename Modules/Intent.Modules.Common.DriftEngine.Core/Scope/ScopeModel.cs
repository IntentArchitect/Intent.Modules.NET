using System.Text;
using System.Text.Json;
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

/// <summary>Where a file stands: in or out, and the row that decided it, null for none.</summary>
/// <param name="In">The file is read: an In or a Context row decided it.</param>
internal sealed record ScopeDecision(bool In, ScopeRow? Row)
{
    /// <summary>Read, and held to the model: an In row decided it, not a Context one.</summary>
    public bool Checked => In && Row is not { Context: true };
}

/// <summary>A file under the output folder: its path relative to it, with <c>/</c> separators, its size and when it was last written.</summary>
internal sealed record WalkedFile(string Path, long Length, DateTime LastWrite);

/// <summary>
/// The output folder's files, less the built-in exclusions (scope.md): each folder's files, the folders breadth first, as
/// .NET's own recursive enumeration gives them; and how many folders each built-in exclusion took.
/// </summary>
internal sealed record WalkedFolder(IReadOnlyList<WalkedFile> Files, IReadOnlyDictionary<string, int> Builtin);

/// <summary>
/// Scope on the Codebase Structure designer (decision 1, scope.md): the deepest folder with a matching row decides a
/// file, the last matching row within it wins, and a file no row matches is out.
/// </summary>
internal sealed class ScopeModel
{
    private static readonly string[] BuiltinFolders = [".git", "bin", "obj", "node_modules", "Intent.Metadata"];

    private ScopeModel(IReadOnlyList<ScopeRow> rows) => Rows = rows;

    public IReadOnlyList<ScopeRow> Rows { get; }

    /// <param name="codebaseStructure">The Codebase Structure designer's packages.</param>
    public static ScopeModel From(IReadOnlyList<IPackage>? codebaseStructure)
    {
        var rows = new List<ScopeRow>();
        foreach (var package in codebaseStructure ?? [])
        {
            AddRows(rows, package.Id, "", package.Stereotypes);
            foreach (var element in package.ChildElements)
            {
                AddFolder(rows, element, "");
            }
        }

        return new ScopeModel(rows);
    }

    /// <summary>
    /// Every file under the output folder that no built-in exclusion takes (scope.md, Built-in exclusions). The walk never
    /// enters an excluded folder, so a build's output or a package cache costs nothing however large it is: <c>.git</c>,
    /// <c>bin</c>, <c>obj</c>, <c>node_modules</c> and <c>Intent.Metadata</c>, hidden folders, Intent's own (a folder holding an
    /// <c>.isln</c>, an <c>.application.config</c> or <c>Intent.Metadata</c>), NuGet's packages folder, known by what it
    /// holds: a folder of its own per package, with the package's <c>.nupkg</c> in it, and the folders LibMan installs
    /// libraries into, which its <c>libman.json</c> names. A folder named <c>packages</c> is also where a JavaScript or
    /// TypeScript monorepo keeps its source, so the name alone excludes nothing.
    /// </summary>
    public static WalkedFolder Walk(string root)
    {
        var files = new List<WalkedFile>();
        var builtin = new Dictionary<string, int>(StringComparer.Ordinal);
        var queue = new Queue<Folder>([Visit(new DirectoryInfo(root), "", null, new HashSet<string>(StringComparer.OrdinalIgnoreCase)).Folder!]);
        while (queue.TryDequeue(out var folder))
        {
            files.AddRange(folder.Files);
            foreach (var excluded in folder.Excluded)
            {
                builtin[excluded] = builtin.GetValueOrDefault(excluded) + 1;
            }

            foreach (var child in folder.Children)
            {
                queue.Enqueue(child);
            }
        }

        return new WalkedFolder(files, builtin);
    }

    /// <summary>A folder's own files, the folders under it that are walked, and the built-in exclusion of each that isn't.</summary>
    private sealed record Folder(List<WalkedFile> Files, List<Folder> Children, List<string> Excluded);

    /// <summary>
    /// The folders a <c>libman.json</c> installs libraries into, relative to its own folder with <c>/</c> separators: each
    /// library's <c>destination</c>, else its <c>defaultDestination</c>. They hold third-party copies (eShop10's Identity.API
    /// has jQuery, jQuery Validation and Bootstrap in <c>wwwroot/lib</c>) that no model describes, as <c>node_modules</c> does.
    /// One that leaves its folder, or a file that won't parse, gives nothing.
    /// </summary>
    private static List<string> LibManDestinations(FileInfo libman)
    {
        JsonNode? json;
        try
        {
            json = JsonNode.Parse(File.ReadAllText(libman.FullName), documentOptions: new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        }
        catch (Exception e) when (e is JsonException or IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            return [];
        }

        string? Text(JsonNode? node) => node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
        var fallback = Text(json?["defaultDestination"]);
        return (json?["libraries"] as JsonArray ?? [])
            .Select(x => (Text(x?["destination"]) ?? fallback)?.Replace('\\', '/').Trim('/'))
            .OfType<string>()
            .Where(x => x.Length > 0 && !x.Split('/').Any(s => s is "" or "." or ".."))
            .ToList();
    }

    /// <summary>A folder walked, or the built-in exclusion that takes it, or a package's own folder in NuGet's packages folder.</summary>
    private sealed record Visited(Folder? Folder, string? Excluded = null, bool Package = false);

    /// <param name="parent">The folder above, by its path; null for the output folder, which no built-in exclusion takes.</param>
    /// <param name="libraries">The folders, by path, that a <c>libman.json</c> above or in this folder installs libraries into.</param>
    private static Visited Visit(DirectoryInfo directory, string path, string? parent, HashSet<string> libraries)
    {
        var files = directory.EnumerateFiles().ToList();
        if (files.FirstOrDefault(x => x.Name.Equals("libman.json", StringComparison.OrdinalIgnoreCase)) is { } libman)
        {
            libraries.UnionWith(LibManDestinations(libman).Select(x => Join(path, x)));
        }

        // A package's own folder in NuGet's packages folder, which takes the folder above it; never the output folder's.
        if (parent is { Length: > 0 } && files.Any(x => x.Name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase)))
        {
            return new Visited(null, Package: true);
        }

        var directories = directory.EnumerateDirectories().ToList();

        // A solution at the root of the output folder can't make the whole codebase Intent's own.
        if (parent is not null && (files.Any(x => x.Name.EndsWith(".isln", StringComparison.OrdinalIgnoreCase) || x.Name.EndsWith(".application.config", StringComparison.OrdinalIgnoreCase))
                                   || directories.Any(x => x.Name.Equals("Intent.Metadata", StringComparison.OrdinalIgnoreCase))))
        {
            return new Visited(null, "Intent solution");
        }

        var folder = new Folder(files.Select(x => new WalkedFile(Join(path, x.Name), x.Length, x.LastWriteTimeUtc)).ToList(), [], []);
        foreach (var child in directories)
        {
            // .NET's own recursive enumeration doesn't follow a link to a folder (a junction, a symbolic link), so nor does this.
            if (child.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                continue;
            }

            var excluded = BuiltinFolders.Contains(child.Name, StringComparer.OrdinalIgnoreCase) ? child.Name
                : child.Name.StartsWith('.') ? "hidden folders"
                : libraries.Contains(Join(path, child.Name)) ? "LibMan libraries"
                : null;
            if (excluded is null)
            {
                var visited = Visit(child, Join(path, child.Name), path, libraries);
                if (visited.Package)
                {
                    return new Visited(null, "NuGet packages");
                }

                if (visited.Folder is { } walked)
                {
                    folder.Children.Add(walked);
                    continue;
                }

                excluded = visited.Excluded;
            }

            folder.Excluded.Add(excluded!);
        }

        return new Visited(folder);
    }

    private static string Join(string path, string name) => path.Length == 0 ? name : $"{path}/{name}";

    public ScopeDecision Decide(string path)
    {
        // Deepest host first; within a host, the last matching row wins.
        var decider = Rows
            .Where(x => x.Types.Count == 0 && x.Matches(path))
            .GroupBy(x => x.Host)
            .OrderByDescending(x => x.First().BasePath.Length)
            .Select(x => x.Last())
            .FirstOrDefault();
        return decider is null ? new ScopeDecision(false, null) : new ScopeDecision(decider.In, decider);
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
