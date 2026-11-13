using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity check 7: every requirement a case names exists.</summary>
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
}
