using Intent.Modules.Common.DriftEngine.OptIn;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>
/// The engine's own opt-in rules. A module's rule with a stock rule's id replaces it; one that only adjusts a stock rule (drops
/// what its house can't model, rewords a finding) runs the stock rule from here and returns what it makes of the result,
/// rather than rebuilding it. What a framework's reader knows a designer can't state needs no rule at all: it marks the code
/// (<see cref="Code.CodeGraph.Unmodellable"/>, <see cref="Code.CodeGraph.Unstatable"/>, <see cref="Code.CodeGraph.StatedByLink"/>).
/// </summary>
public static class StockOptInRules
{
    /// <summary>Every stock opt-in rule, in the order the engine runs them.</summary>
    public static IReadOnlyList<IOptInRule> All { get; } =
        [new UnmodelledFile(), new UnmodelledType(), new UnmodelledMember(), new UnmodelledInteraction(), new UnmodelledExposed(), new UnmodelledValue()];

    /// <summary>A stock opt-in rule by its id, without <c>optin/</c> (<c>unmodelled.member</c>), or null.</summary>
    public static IOptInRule? Find(string id) => All.FirstOrDefault(x => x.Id == id);
}
