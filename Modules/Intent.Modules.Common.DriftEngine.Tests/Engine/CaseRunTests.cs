using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>Runs every case through the engine and checks the run result against its expect block.</summary>
public class CaseRunTests
{
    public static TheoryData<string> Cases => new(AcceptanceData.Cases.Select(x => x.FullId));

    [Theory]
    [MemberData(nameof(Cases))]
    public void Case_produces_its_expected_result(string fullId)
    {
        var engine = EngineUnderTest.Current;
        if (engine is null)
        {
            Assert.Skip(EngineUnderTest.NotYet);
        }

        var @case = AcceptanceData.Cases.Single(x => x.FullId == fullId);
        if (engine.CannotRun(@case) is { } why)
        {
            Assert.Skip(why);
        }

        var outputFolder = Directory.CreateTempSubdirectory("driftengine-case-").FullName;
        try
        {
            foreach (var (path, block) in @case.Files)
            {
                var target = Path.Combine(outputFolder, path);
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.WriteAllText(target, block.Content + "\n");
            }

            var input = new EngineInput(
                @case.Model!,
                outputFolder,
                @case.Meta.Texts("packs").Where(x => AcceptanceData.StockPacks.Contains(x) || HouseExtensions.Find(x) is not null).ToList(),
                @case.Packs.ToList());
            var result = engine.Run(input);

            SchemaSet.Instance.Validate("run-result.schema.json", result).ShouldBeEmpty("The engine's run result must match run-result.schema.json.");
            ExpectComparer.Compare(@case.Expect!, result).ShouldBeEmpty();
            FilesReadAreInScope(result);
        }
        finally
        {
            Directory.Delete(outputFolder, recursive: true);
        }
    }

    /// <summary>R22: no out-of-scope file is read. The coverage listing says which rows excluded which files.</summary>
    private static void FilesReadAreInScope(JsonNode result)
    {
        var claimsOutOfScope = result.Items("claims")
            .Where(x => x.Text("reason") == "outOfScope")
            .Select(x => x["reading"].Text("shadowFile"))
            .OfType<string>()
            .ToHashSet(StringComparer.Ordinal);
        var read = result.Items("files").Select(x => x.Text("path")!).ToList();

        read.Where(claimsOutOfScope.Contains).ShouldBeEmpty("A file a claim was shadowed by is out of scope, and must never be read.");
    }
}
