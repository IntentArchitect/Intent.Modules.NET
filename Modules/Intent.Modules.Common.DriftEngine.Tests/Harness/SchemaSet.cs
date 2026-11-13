using System.Text.Json.Nodes;
using Json.Schema;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// The spec's JSON Schemas, registered by their <c>$id</c> so their relative <c>$ref</c>s resolve to each other.
/// </summary>
internal sealed class SchemaSet
{
    public const string BaseUri = "https://schemas.intentarchitect.com/driftengine/";

    private static readonly Lazy<SchemaSet> Shared = new(() => new SchemaSet(Paths.Schemas));
    private readonly EvaluationOptions _options = new() { OutputFormat = OutputFormat.List };

    public SchemaSet(string folder)
    {
        foreach (var file in Directory.GetFiles(folder, "*.schema.json").Order(StringComparer.Ordinal))
        {
            var name = Path.GetFileName(file);
            var text = File.ReadAllText(file);
            Documents[name] = JsonText.Parse(text);
            var schema = JsonSchema.FromText(text);
            Schemas[name] = schema;
            _options.SchemaRegistry.Register(schema);
        }
    }

    public static SchemaSet Instance => Shared.Value;

    /// <summary>Each schema file's JSON, by file name.</summary>
    public Dictionary<string, JsonNode> Documents { get; } = new(StringComparer.Ordinal);

    public Dictionary<string, JsonSchema> Schemas { get; } = new(StringComparer.Ordinal);

    /// <summary>Every error validating <paramref name="instance"/> against the named schema; empty when it is valid.</summary>
    public IReadOnlyList<string> Validate(string schemaFile, JsonNode? instance)
    {
        if (!Schemas.TryGetValue(schemaFile, out var schema))
        {
            throw new InvalidOperationException($"There is no schema '{schemaFile}' in {Paths.Schemas}.");
        }

        return Errors(schema.Evaluate(instance, _options));
    }

    /// <summary>Every error validating the named schema against the draft 2020-12 meta-schema.</summary>
    public IReadOnlyList<string> MetaValidate(string schemaFile)
    {
        return Errors(MetaSchemas.Draft202012.Evaluate(Documents[schemaFile], new EvaluationOptions { OutputFormat = OutputFormat.List }));
    }

    /// <summary>Every <c>$ref</c> in the named schema that names no schema in the set, or no location within it.</summary>
    public IReadOnlyList<string> UnresolvedReferences(string schemaFile)
    {
        var unresolved = new List<string>();
        var self = new Uri(BaseUri + schemaFile);
        foreach (var (pointer, reference) in References(Documents[schemaFile], ""))
        {
            var target = new Uri(self, reference);
            var targetFile = target.GetLeftPart(UriPartial.Path);
            if (!targetFile.StartsWith(BaseUri, StringComparison.Ordinal)
                || !Documents.TryGetValue(targetFile[BaseUri.Length..], out var document))
            {
                unresolved.Add($"{pointer}: '{reference}' names no schema in the set");
                continue;
            }

            var fragment = Uri.UnescapeDataString(target.Fragment.TrimStart('#'));
            if (Resolve(document, fragment) is null)
            {
                unresolved.Add($"{pointer}: '{reference}' names no location in {targetFile[BaseUri.Length..]}");
            }
        }

        return unresolved;
    }

    private static IEnumerable<(string Pointer, string Reference)> References(JsonNode? node, string pointer)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var (name, child) in obj)
                {
                    if (name == "$ref" && child is JsonValue value && value.TryGetValue<string>(out var reference))
                    {
                        yield return (pointer, reference);
                        continue;
                    }

                    foreach (var found in References(child, $"{pointer}/{name}"))
                    {
                        yield return found;
                    }
                }

                break;
            case JsonArray array:
                for (var i = 0; i < array.Count; i++)
                {
                    foreach (var found in References(array[i], $"{pointer}/{i}"))
                    {
                        yield return found;
                    }
                }

                break;
        }
    }

    private static JsonNode? Resolve(JsonNode document, string fragment)
    {
        if (fragment.Length == 0)
        {
            return document;
        }

        if (!fragment.StartsWith('/'))
        {
            // An anchor: the suite's schemas don't use them, so one is unresolved.
            return null;
        }

        JsonNode? current = document;
        foreach (var token in fragment[1..].Split('/').Select(x => x.Replace("~1", "/").Replace("~0", "~")))
        {
            current = current switch
            {
                JsonObject obj => obj.TryGetPropertyValue(token, out var child) ? child : null,
                JsonArray array when int.TryParse(token, out var index) && index >= 0 && index < array.Count => array[index],
                _ => null
            };

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    private static IReadOnlyList<string> Errors(EvaluationResults results)
    {
        if (results.IsValid)
        {
            return [];
        }

        var errors = results.Details
            .Prepend(results)
            .Where(x => x.HasErrors)
            .SelectMany(x => x.Errors!.Select(e => $"{(x.InstanceLocation.ToString() is { Length: > 0 } at ? at : "/")}: {e.Key}: {e.Value}"))
            .Distinct()
            .ToList();
        return errors.Count > 0 ? errors : ["the document is invalid, with no error detail"];
    }
}
