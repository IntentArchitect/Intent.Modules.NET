using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>
/// Integrity check 4: every case file parses, every block validates, and everything a case refers to exists.
/// </summary>
public class CaseTests
{
    public static TheoryData<string> CaseFiles => new(AcceptanceData.Files.Select(x => x.Path));

    [Fact]
    public void There_are_case_files()
    {
        AcceptanceData.Files.ShouldNotBeEmpty($"The suite's cases live in {Paths.Cases}.");
    }

    [Theory]
    [MemberData(nameof(CaseFiles))]
    public void Case_file_is_well_formed(string path)
    {
        var file = AcceptanceData.Files.Single(x => x.Path == path);

        file.Cases.ShouldNotBeEmpty();
        CaseValidator.Validate(file).ShouldBeEmpty();
    }
}
