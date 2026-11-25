namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>A real codebase from codebases.json, located on this machine.</summary>
internal sealed record Codebase(string Name, string Root, string? RecipesFolder, bool Client, IReadOnlyList<string> UsedFor)
{
    public bool IsPresent => Directory.Exists(Root);

    /// <summary>The Common.Drift recipe files, <c>*.drift.json</c>; none when the codebase or its recipe folder is absent.</summary>
    public IReadOnlyList<string> RecipeFiles => RecipeFolders().SelectMany(x => Directory.GetFiles(x, "*.drift.json")).Order(StringComparer.Ordinal).ToList();

    /// <summary>
    /// The folders <see cref="RecipesFolder"/> names under the root, a <c>*</c> segment standing for any one folder
    /// (<c>intent/*/guardrails</c>), so the committed codebases.json needn't name a client's application.
    /// </summary>
    private IEnumerable<string> RecipeFolders()
    {
        if (RecipesFolder is null || !Directory.Exists(Root))
        {
            return [];
        }

        IEnumerable<string> folders = [Root];
        foreach (var segment in RecipesFolder.Split('/', '\\'))
        {
            folders = segment == "*"
                ? folders.SelectMany(Directory.GetDirectories)
                : folders.Select(x => Path.Combine(x, segment)).Where(Directory.Exists);
        }

        return folders;
    }

    public override string ToString() => Name;
}

internal static class Codebases
{
    private static readonly Lazy<IReadOnlyList<Codebase>> All = new(Load);

    public static IReadOnlyList<Codebase> List => All.Value;

    public static Codebase Get(string name) => List.Single(x => x.Name == name);

    private static IReadOnlyList<Codebase> Load()
    {
        var json = JsonText.Load(Path.Combine(Paths.Acceptance, "codebases.json"));
        return json.Items("codebases")
            .Select(x =>
            {
                var name = x.Text("name")!;
                var root = Environment.GetEnvironmentVariable($"DRIFTENGINE_CODEBASE_{name.ToUpperInvariant()}") is { Length: > 0 } overridden
                    ? overridden
                    : x.Text("root")!;
                return new Codebase(name, root, x.Text("recipes"), x["client"]?.GetValue<bool>() == true, x.Texts("usedFor").ToList());
            })
            .ToList();
    }
}
