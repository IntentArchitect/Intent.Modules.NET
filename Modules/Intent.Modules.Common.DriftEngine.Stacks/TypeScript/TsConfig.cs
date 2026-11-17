using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// What a file's <c>tsconfig.json</c> says that the reader needs: how non-relative imports resolve (<c>baseUrl</c>,
/// <c>paths</c>) and whether types state nullability (<c>strict</c>, <c>strictNullChecks</c>). Paths are relative to the
/// output folder, with <c>/</c> separators.
/// </summary>
/// <param name="BaseUrl">The folder non-relative imports resolve from; null when none is set.</param>
/// <param name="Paths">Each <c>paths</c> pattern (<c>@/*</c>) and the folders it maps to, resolved.</param>
/// <param name="Strict">Null when neither is set: nullability is then unknown.</param>
internal sealed record TsConfig(string? BaseUrl, IReadOnlyList<(string Pattern, IReadOnlyList<string> Targets)> Paths, bool? Strict)
{
    private static readonly JsonDocumentOptions Json = new() { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    /// <summary>
    /// The nearest <c>tsconfig.json</c> at or above a file's folder, inside the output folder, or a JavaScript project's
    /// <c>jsconfig.json</c>, which says the same; null when there is none.
    /// </summary>
    /// <param name="loaded">The configs read so far, by path, for the run's other files.</param>
    public static TsConfig? For(string root, string file, Dictionary<string, object?> loaded)
    {
        for (var folder = Folder(file); ; folder = Folder(folder))
        {
            if ((Load(root, Join(folder, "tsconfig.json"), loaded, 0) ?? Load(root, Join(folder, "jsconfig.json"), loaded, 0)) is { } config)
            {
                // A solution-style tsconfig.json (create-vue's) only references the ones that hold the options: the first
                // that includes the file.
                var referenced = (config.Json["references"] as JsonArray ?? [])
                    .Select(x => x?["path"]?.GetValue<string>())
                    .OfType<string>()
                    .Select(x => Normalize(Join(folder, x)))
                    .Select(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? x : Join(x, "tsconfig.json"))
                    .Select(x => Load(root, x, loaded, 0))
                    .OfType<Loaded>()
                    .FirstOrDefault(x => Includes(x, file));
                return (referenced ?? config).Config;
            }

            if (folder.Length == 0)
            {
                return null;
            }
        }
    }

    private sealed record Loaded(TsConfig Config, JsonNode Json, string Folder);

    /// <summary>A config with what it extends: its own options override those.</summary>
    private static Loaded? Load(string root, string path, Dictionary<string, object?> loaded, int depth)
    {
        if (loaded.TryGetValue(path, out var known))
        {
            return known as Loaded;
        }

        return (Loaded?)(loaded[path] = Read(root, path, loaded, depth));
    }

    private static Loaded? Read(string root, string path, Dictionary<string, object?> loaded, int depth)
    {
        if (depth > 8 || !File.Exists(Path.Combine(root, path)))
        {
            return null;
        }

        JsonNode? json;
        try
        {
            json = JsonNode.Parse(File.ReadAllText(Path.Combine(root, path)), documentOptions: Json);
        }
        catch (JsonException)
        {
            return null;
        }

        if (json is null)
        {
            return null;
        }

        var folder = Folder(path);
        var bases = json["extends"] switch
        {
            JsonArray list => list.Select(x => x?.GetValue<string>()).OfType<string>().ToList(),
            JsonValue value => [value.GetValue<string>()],
            _ => []
        };
        var config = new TsConfig(null, [], null);
        foreach (var extended in bases.Where(x => x.StartsWith('.')))
        {
            var target = Normalize(Join(folder, extended));
            if (Load(root, target.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? target : target + ".json", loaded, depth + 1) is { } extendedConfig)
            {
                config = extendedConfig.Config;
            }
        }

        var options = json["compilerOptions"];
        var baseUrl = options?["baseUrl"]?.GetValue<string>() is { } url ? Normalize(Join(folder, url)) : config.BaseUrl;
        var paths = options?["paths"] is JsonObject declared
            ? declared.Select(x => (x.Key, (IReadOnlyList<string>)(x.Value as JsonArray ?? [])
                    .Select(t => t?.GetValue<string>())
                    .OfType<string>()
                    .Select(t => Normalize(Join(options["baseUrl"] is null ? folder : baseUrl!, t)))
                    .ToList()))
                .ToList()
            : config.Paths;
        var strict = Flag(options, "strictNullChecks") ?? Flag(options, "strict") ?? config.Strict;
        return new Loaded(new TsConfig(baseUrl, paths, strict), json, folder);
    }

    private static bool? Flag(JsonNode? options, string name) => options?[name] is JsonValue value && value.TryGetValue<bool>(out var flag) ? flag : null;

    /// <summary>The config's <c>include</c> globs match the file; a config without any includes everything.</summary>
    private static bool Includes(Loaded config, string file)
    {
        var include = (config.Json["include"] as JsonArray ?? []).Select(x => x?.GetValue<string>()).OfType<string>().ToList();
        return include.Count == 0 || include.Any(x => Glob(Normalize(Join(config.Folder, x))).IsMatch(file));
    }

    private static Regex Glob(string pattern) => new("^" + Regex.Escape(pattern)
        .Replace(@"\*\*/", "(?:.*/)?")
        .Replace(@"\*\*", ".*")
        .Replace(@"\*", "[^/]*")
        .Replace(@"\?", "[^/]") + (pattern.Contains('.') ? "" : "(?:/.*)?") + "$", RegexOptions.CultureInvariant);

    internal static string Folder(string path) => path.LastIndexOf('/') is var slash and >= 0 ? path[..slash] : "";

    internal static string Join(string folder, string path) => folder.Length == 0 ? path : $"{folder}/{path}";

    /// <summary>A path with its <c>.</c> and <c>..</c> segments resolved.</summary>
    internal static string Normalize(string path)
    {
        var parts = new List<string>();
        foreach (var part in path.Replace('\\', '/').Split('/'))
        {
            if (part == "..")
            {
                if (parts.Count > 0)
                {
                    parts.RemoveAt(parts.Count - 1);
                }
            }
            else if (part is not ("." or ""))
            {
                parts.Add(part);
            }
        }

        return string.Join("/", parts);
    }
}
