using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>What one engine run is given: a case's model, its code files written to a folder, and its packs.</summary>
/// <param name="Model">The case's <c>model</c> block. The adapter turns it into Intent's model objects (decision 2).</param>
/// <param name="OutputFolder">The folder standing for the application's output folder, holding the case's files.</param>
/// <param name="Packs">The packs the case names: stock pack ids, and the case's own single-file packs.</param>
internal sealed record EngineInput(JsonNode Model, string OutputFolder, IReadOnlyList<string> StockPacks, IReadOnlyList<JsonNode> InlinePacks);

/// <summary>
/// The one interface a phase plugs its engine into (acceptance-suite.md, The harness): given a run's input, the run
/// result as data (run-result.schema.json).
/// </summary>
internal interface IDriftEngineAdapter
{
    JsonNode Run(EngineInput input);

    /// <summary>Runs over a real codebase's own model and code, read in place.</summary>
    JsonNode RunCodebase(Codebase codebase);
}

/// <summary>The engine the suite runs against. Phase 1 sets it; until then every engine test is skipped.</summary>
internal static class EngineUnderTest
{
    public const string NotYet = "The engine arrives in phase 1.";

    public static IDriftEngineAdapter? Current => null;
}
