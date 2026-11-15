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

    // The core's and stock designer packs' kinds, and every stock extension's, in code or in its packs.
    private static readonly IReadOnlySet<string> Kinds = new[] { "intent.core", "intent.domain", "intent.services" }.SelectMany(x => Pack.Stock(x)!.Kinds)
        .Concat(StockExtensions.All.SelectMany(x => x.Kinds.Concat(x.Packs.SelectMany(p => p.Kinds))))
        .Select(x => x.Kind)
        .ToHashSet(StringComparer.Ordinal);

    private static readonly IReadOnlySet<string> Frameworks = new HashSet<string>(StringComparer.Ordinal)
    {
        "intent.efcore", "intent.aspnetcore", "intent.jpa", "intent.springmvc", "intent.mediatr", "intent.nservicebus"
    };

    public JsonNode Run(EngineInput input)
    {
        // A stock pack is the engine's data or a stock extension's.
        var packs = input.StockPacks.Select(x => Pack.Stock(x)).OfType<Pack>().Concat(input.InlinePacks.Select(Pack.Parse)).ToList();
        // The languages and the vocabularies that read no code (intent.rdbms, intent.http: a case's intent.domain and
        // intent.services) always; a framework's extension only when the case names it, so a case can leave a kind unread.
        // What a handler handles, sends, publishes, invokes and acts on is read from any framework's code, so it comes with each.
        var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any())
            .Concat(input.StockPacks.Select(StockExtensions.Find).OfType<IDriftExtension>())
            .Concat(input.StockPacks.Any(Frameworks.Contains) ? [StockExtensions.Find("intent.messaging")!, StockExtensions.Find("intent.interactions")!] : [])
            .Distinct()
            .ToList();
        var designers = FixtureModel.Build(input.Model, out var references);
        var result = DriftEngine.Run(new DriftInput(designers, input.OutputFolder, packs, extensions) { References = references });
        return JsonSerializer.SerializeToNode(result, Options)!;
    }

    public JsonNode RunCodebase(Codebase codebase)
    {
        Assert.Skip("A real codebase's model is read through the Software Factory's metadata API, which the engine isn't hosted in yet.");
        return null!;
    }

    /// <summary>Cases whose engine work isn't one of the checks below, each with what it waits for.</summary>
    private static readonly IReadOnlyDictionary<string, string> WaitingOn = new Dictionary<string, string>
    {
        ["optin/unmodelled.type#services-router"] = MinimalApiService,
        ["optin/unmodelled.member#endpoint-in-router"] = MinimalApiService,
        ["optin/unmodelled.member#request-parameter"] = MinimalApiService
    };

    private const string MinimalApiService = "Locating a Service as the Minimal API router that maps its routes isn't built yet (phase 4).";

    public string? CannotRun(AcceptanceCase @case)
    {
        // A case runs whatever phase it names once the engine has its kinds, packs, rules and language: a phase says when a
        // case was planned for, not whether what it checks is built.
        var meta = @case.Meta!;
        if (meta.Texts("kinds").Concat(@case.Expect.Items("claims").Select(x => x.Text("kind")!)).FirstOrDefault(x => !Kinds.Contains(x)) is { } kind)
        {
            return $"The engine has no rule for {kind} yet.";
        }

        if (meta.Texts("cases").FirstOrDefault(x => x is "optin/unmodelled.interaction" or "optin/unmodelled.exposed" or "optin/unmodelled.value") is { } rule)
        {
            return $"The opt-in rule {rule} isn't built yet.";
        }

        if (WaitingOn.GetValueOrDefault(@case.FullId) is { } waiting)
        {
            return waiting;
        }

        // Find path steps beyond types, members, parameters, calls, annotations and markup.
        var unescaped = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        if (@case.Packs.SelectMany(x => x.ToJsonString(unescaped).Split(" > ")).FirstOrDefault(x => x.StartsWith("chain", StringComparison.Ordinal)) is { } step)
        {
            return $"The find path step '{step.Split(' ')[0]}' isn't built yet.";
        }


        var inline = @case.Packs.Select(x => x.Text("id")).ToHashSet(StringComparer.Ordinal);
        // MediatR's and NServiceBus's handlers, sends and publishes are read by the framework-neutral messaging and interactions
        // readers, so a case naming either has what it needs without a pack of its own.
        if (meta.Texts("packs").FirstOrDefault(x => Pack.Stock(x) is null && StockExtensions.Find(x) is null && !inline.Contains(x)
                                                    && x is not ("intent.mediatr" or "intent.nservicebus")) is { } pack)
        {
            return $"The engine doesn't ship {pack} yet.";
        }

        // A pack of conventions alone needs only find paths, and vocabulary the expression language; recipes and queries
        // need a rule language the engine doesn't have.
        if (@case.Packs.Any(x => x["rules"] is not null || x["queries"] is not null))
        {
            return "Inline packs need the full expression language.";
        }

        if (meta.Text("language") is { } language && language is not ("csharp" or "java" or "typescript" or "protobuf"))
        {
            return $"The engine has no {language} reader yet.";
        }

        return null;
    }
}
