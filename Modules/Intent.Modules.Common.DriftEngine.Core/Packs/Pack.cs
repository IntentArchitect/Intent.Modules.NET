using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Packs;

/// <summary>A claim kind a pack or an extension defines (claim-kinds.schema.json).</summary>
/// <param name="Label">What the finding calls the value: <c>sends</c>.</param>
/// <param name="ValueType"><c>text</c>, <c>bool</c>, <c>number</c>, <c>set</c> or <c>presence</c>.</param>
/// <param name="Comparer">The comparer's name: the engine's <c>exact</c>, <c>ignoreCase</c>, <c>number</c>, <c>set</c> or <c>subset</c>, or an extension's.</param>
/// <param name="Stated"><c>always</c>, or <c>whenSet</c> for a value that is no claim when the model leaves it empty.</param>
/// <param name="Existence">The claim is that the element is in the code at all, so its finding says where was searched.</param>
public sealed record KindDefinition(string Kind, string Label, string ValueType = "text", string Comparer = "exact", string Stated = "always", bool Existence = false);

/// <summary>A pack in its single-file form (packs.md): kinds, vocabulary, conventions and language profiles, as data.</summary>
public sealed class Pack
{
    private Pack(JsonNode data)
    {
        Data = data;
        Id = data["id"]!.GetValue<string>();
        Version = data["version"]!.GetValue<string>();
    }

    public string Id { get; }

    public string Version { get; }

    public JsonNode Data { get; }

    public IEnumerable<KindDefinition> Kinds => (Data["kinds"] as JsonArray ?? [])
        .OfType<JsonNode>()
        .Select(x => new KindDefinition(
            x["kind"]!.GetValue<string>(),
            x["label"]?.GetValue<string>() ?? x["kind"]!.GetValue<string>(),
            x["valueType"]?.GetValue<string>() ?? "text",
            x["comparer"]?.GetValue<string>() ?? "exact",
            x["stated"]?.GetValue<string>() ?? "always",
            x["existence"]?.GetValue<bool>() ?? false));

    /// <summary>The vocabulary entries for one designer.</summary>
    public IEnumerable<JsonNode> Vocabulary(string designer) => (Data["vocabulary"]?[designer]?["entries"] as JsonArray ?? []).OfType<JsonNode>();

    /// <summary>The type conventions for one designer's element type.</summary>
    public JsonNode? TypeConvention(string designer, string specialization) => Data["conventions"]?[designer]?["types"]?[specialization];

    public JsonNode? Profile(string language) => Data["profiles"]?[language];

    /// <summary>A recipe for a claim kind in one designer (rules.md, Recipes), or null when the pack has none.</summary>
    public JsonNode? Recipe(string designer, string kind) => Data["rules"]?[designer]?["facts"]?[kind];

    public static Pack Parse(JsonNode data) => new(data);

    /// <summary>A stock pack shipped inside the engine, or null when the engine has no pack of that id.</summary>
    public static Pack? Stock(string id)
    {
        using var stream = typeof(Pack).Assembly.GetManifestResourceStream($"{id}.pack.json");
        return stream is null ? null : new Pack(JsonNode.Parse(stream)!);
    }
}
