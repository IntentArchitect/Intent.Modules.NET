using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity checks 5 and 6: every check the engine owns is traced, and every early kind has every outcome.</summary>
public class TraceabilityTests
{
    private static readonly string[] Outcomes = ["verified", "contradicted", "cantVerify"];

    [Fact]
    public void Every_claim_scope_engine_and_opt_in_check_is_traced_by_a_case()
    {
        var traced = AcceptanceData.Cases.SelectMany(x => x.Meta.Texts("traces")).ToHashSet(StringComparer.Ordinal);
        var untraced = AcceptanceData.Dispositions
            .Where(x => x.Value.Any(d => d.StartsWith("claim:", StringComparison.Ordinal)
                                         || d.StartsWith("scope:", StringComparison.Ordinal)
                                         || d.StartsWith("engine:", StringComparison.Ordinal)
                                         || d.StartsWith("optin:", StringComparison.Ordinal)))
            .Where(x => !traced.Contains(x.Key))
            .Select(x => $"{x.Key} ({string.Join(", ", x.Value)})")
            .ToList();

        untraced.ShouldBeEmpty($"{untraced.Count} checks have no case tracing them.");
    }

    [Fact]
    public void Every_kind_of_phase_1_to_4_has_a_case_for_each_outcome()
    {
        var covered = AcceptanceData.Cases
            .SelectMany(x => x.Expect.Items("claims"))
            .Select(x => (Kind: x.Text("kind"), Outcome: x.Text("outcome")))
            .ToHashSet();
        var missing = AcceptanceData.Kinds.Values
            .Where(x => x.PhaseNumber <= 4)
            .SelectMany(kind => Outcomes.Where(outcome => !covered.Contains((kind.Kind, outcome))).Select(outcome => $"{kind.Kind} {outcome}"))
            .ToList();

        missing.ShouldBeEmpty($"{missing.Count} kind and outcome pairs have no case.");
    }

    [Fact]
    public void Every_scope_engine_and_opt_in_case_is_shown_by_a_case()
    {
        var shown = AcceptanceData.Cases.SelectMany(x => x.Meta.Texts("cases")).ToHashSet(StringComparer.Ordinal);
        var all = AcceptanceData.ScopeCases.Select(x => $"scope/{x}")
            .Concat(AcceptanceData.EngineCases.Select(x => $"engine/{x}"))
            .Concat(AcceptanceData.OptInRules.Select(x => $"optin/{x}"));

        all.Where(x => !shown.Contains(x)).ShouldBeEmpty();
    }
}
