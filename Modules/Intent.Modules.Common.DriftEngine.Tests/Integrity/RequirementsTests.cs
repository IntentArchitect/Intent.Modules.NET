using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity check 7: every requirement a case names exists, and every case a requirement names exists.</summary>
public class RequirementsTests
{
    private static readonly IReadOnlyList<string> Ids = SpecDocuments.RequirementIds(Paths.Requirements);

    [Fact]
    public void Requirements_are_numbered_from_R1_without_gaps_or_repeats()
    {
        Ids.ShouldBe(Enumerable.Range(1, Ids.Count).Select(x => $"R{x}").ToList());
    }

    [Fact]
    public void Every_requirement_a_case_names_exists()
    {
        var known = Ids.ToHashSet(StringComparer.Ordinal);
        var unknown = AcceptanceData.Cases
            .SelectMany(x => x.Meta.Texts("requirements").Where(r => !known.Contains(r)).Select(r => $"{x.FullId}: {r}"))
            .ToList();

        unknown.ShouldBeEmpty();
    }

    [Fact]
    public void Every_case_a_requirement_names_exists()
    {
        var shown = AcceptanceData.Cases.SelectMany(x => x.Meta.Texts("cases")).ToHashSet(StringComparer.Ordinal);
        var missing = SpecDocuments.RequirementCases(Paths.Requirements)
            .Where(x => !Exists(x.Case, shown))
            .Select(x => $"{x.Requirement}: {x.Case}")
            .ToList();

        missing.ShouldBeEmpty("Each is a case file, an area with at least one file, or a scope, engine or opt-in case some case shows.");
    }

    private static bool Exists(string name, IReadOnlySet<string> shown)
    {
        if (name.EndsWith(".cases.md", StringComparison.Ordinal))
        {
            return File.Exists(Path.Combine(Paths.Cases, name));
        }

        if (name.EndsWith("/*", StringComparison.Ordinal))
        {
            var area = Path.Combine(Paths.Cases, name[..^2]);
            return Directory.Exists(area) && Directory.EnumerateFiles(area, "*.cases.md").Any();
        }

        return shown.Contains(name);
    }
}
