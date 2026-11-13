namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>A real codebase from codebases.json, located on this machine.</summary>
internal sealed record Codebase(string Name, string Root, string? RecipesFolder, bool Client, IReadOnlyList<string> UsedFor)
{
    public bool IsPresent => Directory.Exists(Root);

    /// <summary>The Common.Drift recipe files, <c>*.drift.json</c>; none when the codebase or its recipe folder is absent.</summary>
    public IReadOnlyList<string> RecipeFiles => RecipesFolder is not null && Directory.Exists(Path.Combine(Root, RecipesFolder))
        ? Directory.GetFiles(Path.Combine(Root, RecipesFolder), "*.drift.json").Order(StringComparer.Ordinal).ToList()
        : [];

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
