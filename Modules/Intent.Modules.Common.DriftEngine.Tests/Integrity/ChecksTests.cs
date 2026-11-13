using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity check 2: checks.json has exactly the checks check-classification.md classifies.</summary>
public class ChecksTests
{
    private static readonly IReadOnlyList<ClassifiedCheck> Classified = SpecDocuments.Checks(Paths.CheckClassification);

    [Fact]
    public void Every_check_table_row_has_a_unique_id()
    {
        Classified.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ShouldBeEmpty();
    }

    [Fact]
    public void The_check_tables_agree_with_the_totals_table()
    {
        var (perSource, total) = SpecDocuments.CheckTotals(Paths.CheckClassification);
        var counted = SpecDocuments.Sources.Select(source => Classified.Count(x => x.Source == source)).ToList();

        counted.ShouldBe(perSource);
        Classified.Count.ShouldBe(total);
    }

    [Fact]
    public void Checks_json_has_exactly_the_classified_ids()
    {
        var listed = AcceptanceData.Dispositions.Keys.ToHashSet(StringComparer.Ordinal);
        var classified = Classified.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);

        classified.Except(listed).Order(StringComparer.Ordinal).ShouldBeEmpty("These checks have no disposition in checks.json.");
        listed.Except(classified).Order(StringComparer.Ordinal).ShouldBeEmpty("checks.json lists these, but check-classification.md has no such check.");
    }

    [Fact]
    public void Checks_json_count_is_the_number_of_checks()
    {
        AcceptanceData.ChecksJson["count"]!.GetValue<int>().ShouldBe(Classified.Count);
    }
}
