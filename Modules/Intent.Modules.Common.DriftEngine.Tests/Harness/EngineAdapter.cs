using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;

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

    /// <summary>Why the engine can't run a case yet, or null when it can.</summary>
    string? CannotRun(AcceptanceCase @case);
}

/// <summary>The engine the suite runs against.</summary>
internal static class EngineUnderTest
{
    public const string NotYet = "The engine arrives in phase 1.";

    public static IDriftEngineAdapter? Current { get; } = new DriftEngineAdapter();
}

/// <summary>
/// Intent.Modules.Common.DriftEngine.Core behind the suite. It runs the cases whose kinds and packs it has; the rest stay
/// skipped, each saying what it waits for.
/// </summary>
internal sealed class DriftEngineAdapter : IDriftEngineAdapter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly IReadOnlySet<string> Kinds = Pack.Stock("intent.core")!.Kinds.Select(x => x.Kind).ToHashSet(StringComparer.Ordinal);

    public JsonNode Run(EngineInput input)
    {
        // A stock pack is the engine's data or a stock extension's.
        var packs = input.StockPacks.Select(x => Pack.Stock(x)).OfType<Pack>().Concat(input.InlinePacks.Select(Pack.Parse)).ToList();
        // The fixtures are C#: its language extension always reads them.
        var extensions = new[] { StockExtensions.Find("csharp")! }.Concat(input.StockPacks.Select(StockExtensions.Find).OfType<IDriftExtension>()).Distinct().ToList();
        var result = DriftEngine.Run(new DriftInput(FixtureModel.Build(input.Model), input.OutputFolder, packs, extensions));
        return JsonSerializer.SerializeToNode(result, Options)!;
    }

    public JsonNode RunCodebase(Codebase codebase)
    {
        Assert.Skip("A real codebase's model is read through the Software Factory's metadata API, which the engine isn't hosted in yet.");
        return null!;
    }

    public string? CannotRun(AcceptanceCase @case)
    {
        var meta = @case.Meta!;
        if (meta["phase"]?.ToString() != "1")
        {
            return $"Phase {meta["phase"]} case; the engine is at phase 1.";
        }

        if (meta.Texts("kinds").Concat(@case.Expect.Items("claims").Select(x => x.Text("kind")!)).FirstOrDefault(x => !Kinds.Contains(x)) is { } kind)
        {
            return $"The engine has no rule for {kind} yet.";
        }

        if (@case.Area == "optin" || meta.Texts("cases").Any(x => x.StartsWith("optin/", StringComparison.Ordinal) || x == "scope/gate"))
        {
            return "Opt-in \"must be modelled\" rules aren't built yet.";
        }

        if (meta.Texts("cases").Contains("engine/noVocabulary"))
        {
            return "Coverage of the model (noVocabulary findings) isn't built yet.";
        }

        if (meta.Texts("packs").FirstOrDefault(x => Pack.Stock(x) is null && StockExtensions.Find(x) is null) is { } pack)
        {
            return $"The engine doesn't ship {pack} yet.";
        }

        if (@case.Packs.Any())
        {
            return "Inline packs need the full expression language.";
        }

        if (meta.Text("language") is { } language && language != "csharp")
        {
            return $"The engine has no {language} reader yet.";
        }

        return null;
    }
}
