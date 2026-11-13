using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>
/// Integrity check 8: every construct the local Common.Drift recipe files use is in conversion-map.json
/// (conversion.md, The conversion test). Recipe files are read in place; a failure names construct keys only.
/// </summary>
public class ConversionMapTests
{
    public static TheoryData<string> RecipeCodebases => new(Codebases.List.Where(x => x.RecipesFolder is not null).Select(x => x.Name));

    [Theory]
    [MemberData(nameof(RecipeCodebases))]
    public void Every_recipe_construct_is_mapped(string name)
    {
        var codebase = Codebases.Get(name);
        if (codebase.RecipeFiles.Count == 0)
        {
            Assert.Skip($"{name}'s recipe files aren't on this machine (looked in {Path.Combine(codebase.Root, codebase.RecipesFolder!)}).");
        }

        var mapped = AcceptanceData.ConversionConstructs;
        var missing = codebase.RecipeFiles
            .SelectMany(file => RecipeConstructs.Of(JsonText.Load(file)).Keys.Select(key => (File: Path.GetFileName(file), Key: key)))
            .Where(x => !mapped.Contains(x.Key))
            .GroupBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => $"{x.Key} ({string.Join(", ", x.Select(f => f.File).Distinct())})")
            .Order(StringComparer.Ordinal)
            .ToList();

        missing.ShouldBeEmpty($"conversion-map.json doesn't say what becomes of {missing.Count} constructs {name}'s recipes use.");
    }

    [Fact]
    public void Every_semantic_change_a_construct_cites_exists()
    {
        var changes = File.ReadLines(Paths.Conversion)
            .SkipWhile(x => x.Trim() != "## Semantic changes")
            .Select(x => SpecDocuments.Cells(x))
            .Where(x => x.Count > 1 && int.TryParse(x[0], out _))
            .Select(x => int.Parse(x[0]))
            .ToHashSet();
        var cited = AcceptanceData.ConversionMapJson["constructs"]!.AsObject()
            .Where(x => x.Value?["change"] is not null)
            .Where(x => !changes.Contains(x.Value!["change"]!.GetValue<int>()))
            .Select(x => x.Key)
            .ToList();

        changes.ShouldNotBeEmpty("conversion.md has a '## Semantic changes' table.");
        cited.ShouldBeEmpty();
    }
}
