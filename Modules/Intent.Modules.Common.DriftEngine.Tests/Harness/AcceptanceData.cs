using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>A claim kind as catalog.json lists it.</summary>
internal sealed record CatalogKind(string Kind, string Area, string ValueType, string Comparer, string Stated, bool Existence, string Phase)
{
    /// <summary>The phase as a number; <c>later</c> is beyond every phase.</summary>
    public int PhaseNumber => int.TryParse(Phase, out var number) ? number : int.MaxValue;
}

/// <summary>The suite's own data files, loaded once.</summary>
internal static class AcceptanceData
{
    private static readonly Lazy<JsonNode> ChecksNode = new(() => JsonText.Load(Path.Combine(Paths.Acceptance, "checks.json")));
    private static readonly Lazy<JsonNode> CatalogNode = new(() => JsonText.Load(Path.Combine(Paths.Acceptance, "catalog.json")));
    private static readonly Lazy<JsonNode> ConversionMapNode = new(() => JsonText.Load(Path.Combine(Paths.Acceptance, "conversion-map.json")));
    private static readonly Lazy<IReadOnlyList<CaseFile>> CaseFiles = new(() => CaseFileParser.ParseFolder(Paths.Cases));

    public static JsonNode ChecksJson => ChecksNode.Value;

    public static JsonNode CatalogJson => CatalogNode.Value;

    public static JsonNode ConversionMapJson => ConversionMapNode.Value;

    /// <summary>Each check id's dispositions, in the order checks.json lists them.</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> Dispositions => DispositionsLazy.Value;

    public static IReadOnlyDictionary<string, CatalogKind> Kinds => KindsLazy.Value;

    public static IReadOnlySet<string> ScopeCases => CatalogJson.Texts("scope").ToHashSet(StringComparer.Ordinal);

    public static IReadOnlySet<string> EngineCases => CatalogJson.Texts("engine").ToHashSet(StringComparer.Ordinal);

    public static IReadOnlySet<string> OptInRules => CatalogJson.Texts("optIn").ToHashSet(StringComparer.Ordinal);

    public static IReadOnlySet<string> StockPacks => CatalogJson.Texts("packs").ToHashSet(StringComparer.Ordinal);

    /// <summary>Every construct key conversion-map.json says what becomes of.</summary>
    public static IReadOnlySet<string> ConversionConstructs => ((ConversionMapJson["constructs"] as JsonObject)?.Select(x => x.Key) ?? []).ToHashSet(StringComparer.Ordinal);

    public static IReadOnlyList<CaseFile> Files => CaseFiles.Value;

    public static IEnumerable<AcceptanceCase> Cases => Files.SelectMany(x => x.Cases);

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyList<string>>> DispositionsLazy = new(() =>
        ((JsonObject)ChecksJson["dispositions"]!).ToDictionary(
            x => x.Key,
            x => (IReadOnlyList<string>)(x.Value is JsonArray array ? array.Select(v => v!.GetValue<string>()).ToList() : [x.Value!.GetValue<string>()]),
            StringComparer.Ordinal));

    private static readonly Lazy<IReadOnlyDictionary<string, CatalogKind>> KindsLazy = new(() =>
        CatalogJson.Items("kinds")
            .Select(x => new CatalogKind(
                x.Text("kind")!,
                x.Text("area")!,
                x.Text("valueType")!,
                x.Text("comparer")!,
                x.Text("stated")!,
                x["existence"]!.GetValue<bool>(),
                x["phase"]!.ToString()))
            .GroupBy(x => x.Kind, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal));
}
