using System.Text.Json;
using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>Reads JSON as the suite's files are written: comments and trailing commas allowed.</summary>
internal static class JsonText
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static JsonNode Parse(string text)
    {
        return JsonNode.Parse(text, documentOptions: DocumentOptions)
               ?? throw new JsonException("The document is the literal null.");
    }

    public static JsonNode Load(string path) => Parse(File.ReadAllText(path));

    /// <summary>The string value of <paramref name="property"/>, or <see langword="null"/>.</summary>
    public static string? Text(this JsonNode? node, string property)
    {
        return node is JsonObject obj && obj[property] is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
    }

    /// <summary>The items of the array <paramref name="property"/>; none when it is missing.</summary>
    public static IEnumerable<JsonNode> Items(this JsonNode? node, string property)
    {
        return node is JsonObject obj && obj[property] is JsonArray array ? array.OfType<JsonNode>() : [];
    }

    /// <summary>The string items of the array <paramref name="property"/>.</summary>
    public static IEnumerable<string> Texts(this JsonNode? node, string property)
    {
        return node.Items(property).OfType<JsonValue>().Select(x => x.TryGetValue<string>(out var text) ? text : null).OfType<string>();
    }
}
