using System.Diagnostics;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Real;

/// <summary>
/// Runs the engine over each real codebase's own model and code, in place (acceptance-suite.md, Real codebases).
/// Each phase adds its gate's assertions here; every run asserts the codebase is unchanged (R17).
/// </summary>
public class RealCodebaseTests
{
    public static TheoryData<string> Names => new(Codebases.List.Select(x => x.Name));

    [Theory]
    [MemberData(nameof(Names))]
    public void Checking_a_codebase_changes_no_file(string name)
    {
        var codebase = Codebases.Get(name);
        var engine = EngineUnderTest.Current;
        if (engine is null)
        {
            Assert.Skip(EngineUnderTest.NotYet);
        }

        if (!codebase.IsPresent)
        {
            Assert.Skip($"{name} isn't on this machine (looked in {codebase.Root}; set DRIFTENGINE_CODEBASE_{name.ToUpperInvariant()} to point elsewhere).");
        }

        var before = GitStatus(codebase.Root);
        var result = engine.RunCodebase(codebase);
        var after = GitStatus(codebase.Root);

        SchemaSet.Instance.Validate("run-result.schema.json", result).ShouldBeEmpty();
        after.ShouldBe(before, "Checking never writes code (P4, R17).");
    }

    private static string GitStatus(string root)
    {
        using var git = Process.Start(new ProcessStartInfo("git", "status --porcelain")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            UseShellExecute = false
        })!;
        var output = git.StandardOutput.ReadToEnd();
        git.WaitForExit();
        return output;
    }
}
