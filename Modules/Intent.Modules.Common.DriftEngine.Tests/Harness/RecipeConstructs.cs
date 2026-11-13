using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// The constructs a Common.Drift recipe file uses, as conversion-map.json keys them: <c>file.&lt;key&gt;</c>,
/// <c>files.&lt;string|list&gt;</c>, <c>kind.&lt;kind&gt;</c>, <c>recipe.&lt;property&gt;[.&lt;form&gt;]</c>,
/// <c>case.&lt;property&gt;[.&lt;value&gt;]</c>, <c>step.&lt;name&gt;</c>, <c>filter.&lt;name&gt;</c>, <c>regex.&lt;feature&gt;</c>,
/// <c>own.&lt;property&gt;</c>, <c>own.step.&lt;name&gt;</c>, <c>own.name.group2plus</c> and <c>exception.&lt;form&gt;</c>.
/// </summary>
/// <remarks>
/// A key never carries text from the recipe beyond the format's own words (property names, step kinds, filter names,
/// fact kinds and keyword values), so a failure message can't quote a client's code or identifiers.
/// </remarks>
internal static class RecipeConstructs
{
    private static readonly HashSet<string> StepKinds = new(StringComparer.Ordinal) { "file", "class", "method", "property", "parameter", "statement", "chain" };
    private static readonly HashSet<string> RelativeInheritTargets = new(StringComparer.Ordinal) { "class", "base", "method", "property", "statement", "file" };
    private static readonly HashSet<string> Filters = new(StringComparer.Ordinal) { "lower", "upper", "camel", "pascal", "kebab", "trimStart", "trimEnd", "replace" };
    private static readonly Dictionary<string, HashSet<string>> Keywords = new(StringComparer.Ordinal)
    {
        ["part"] = new(StringComparer.Ordinal) { "declaration", "body" },
        ["in"] = new(StringComparer.Ordinal) { "class", "method", "property", "parameter", "statement", "file" },
        ["case"] = new(StringComparer.Ordinal) { "upper", "lower" }
    };

    private static readonly Regex Word = new("^[A-Za-z][A-Za-z0-9]*$", RegexOptions.CultureInvariant);
    private static readonly Regex KindName = new(@"^[a-z][A-Za-z]*(\.[a-z][A-Za-z]*)*$", RegexOptions.CultureInvariant);
    private static readonly Regex Placeholder = new(@"(?<!\\)\{(?<name>[A-Z][A-Za-z]*)(?<filters>(?:\|[A-Za-z]+(?::[^|}]*)?)*)\}", RegexOptions.CultureInvariant);
    private static readonly Regex BackReference = new(@"\\[1-9]|\\k<", RegexOptions.CultureInvariant);
    private static readonly Regex NamedGroup = new(@"\(\?<[A-Za-z]\w*>|\(\?'[A-Za-z]\w*'", RegexOptions.CultureInvariant);

    /// <summary>Each construct the recipe file uses, with how many times.</summary>
    public static IReadOnlyDictionary<string, int> Of(JsonNode recipeFile)
    {
        var keys = new SortedDictionary<string, int>(StringComparer.Ordinal);
        void Add(string key) => keys[key] = keys.GetValueOrDefault(key) + 1;

        if (recipeFile is not JsonObject file)
        {
            Add("file.notAnObject");
            return keys;
        }

        foreach (var (name, value) in file)
        {
            Add($"file.{Safe(name.TrimStart('$'))}");
            switch (name)
            {
                case "files":
                    Add(value is JsonArray ? "files.list" : "files.string");
                    break;
                case "facts" when value is JsonObject facts:
                    foreach (var (kind, recipe) in facts)
                    {
                        Add(KindName.IsMatch(kind) ? $"kind.{kind}" : "kind.other");
                        Recipe(recipe, Add);
                    }

                    break;
                case "own" when value is JsonArray own:
                    foreach (var entry in own.OfType<JsonObject>())
                    {
                        Own(entry, Add);
                    }

                    break;
                case "exceptions" when value is JsonArray exceptions:
                    foreach (var exception in exceptions)
                    {
                        Exception(exception, Add);
                    }

                    break;
            }
        }

        return keys;
    }

    private static void Recipe(JsonNode? recipe, Action<string> add)
    {
        if (recipe is not JsonObject obj)
        {
            add("recipe.notAnObject");
            return;
        }

        foreach (var (name, value) in obj)
        {
            switch (name)
            {
                case "find":
                    add(value is JsonArray ? "recipe.find.list" : "recipe.find.string");
                    foreach (var path in StringsOf(value))
                    {
                        Steps(path, "step", add);
                        Regexes(path, add);
                    }

                    break;
                case "inherit":
                    add(value is JsonArray ? "recipe.inherit.list" : "recipe.inherit.string");
                    foreach (var target in StringsOf(value))
                    {
                        if (RelativeInheritTargets.Contains(target.Trim()))
                        {
                            add($"recipe.inherit.value.{target.Trim()}");
                        }
                        else
                        {
                            add("recipe.inherit.value.findPath");
                            Steps(target, "step", add);
                            Regexes(target, add);
                        }
                    }

                    break;
                case "files":
                    add(value is JsonArray ? "recipe.files.list" : "recipe.files.string");
                    break;
                case "case":
                    add($"recipe.case.{Keyword("case", value)}");
                    break;
                case "read":
                    add("recipe.read");
                    foreach (var readCase in value is JsonArray cases ? cases : new JsonArray())
                    {
                        ReadCase(readCase, add);
                    }

                    if (value is not JsonArray)
                    {
                        // Common.Drift refuses this: read must be an array of cases.
                        add("recipe.read.notAnArray");
                    }

                    break;
                default:
                    add($"recipe.{Safe(name)}");
                    break;
            }
        }

        foreach (var text in AllStrings(obj))
        {
            Placeholders(text, add);
        }
    }

    private static void ReadCase(JsonNode? readCase, Action<string> add)
    {
        if (readCase is not JsonObject obj)
        {
            add("case.notAnObject");
            return;
        }

        foreach (var (name, value) in obj)
        {
            switch (name)
            {
                case "part" or "in" or "case":
                    add($"case.{name}.{Keyword(name, value)}");
                    break;
                case "match":
                    add("case.match");
                    foreach (var text in StringsOf(value))
                    {
                        Regexes(text, add);
                    }

                    break;
                default:
                    add($"case.{Safe(name)}");
                    break;
            }
        }
    }

    private static void Own(JsonObject entry, Action<string> add)
    {
        foreach (var (name, value) in entry)
        {
            add($"own.{Safe(name)}");
            switch (name)
            {
                case "find":
                    foreach (var path in StringsOf(value))
                    {
                        Steps(path, "own.step", add);
                        Regexes(path, add);
                    }

                    break;
                case "name" when StringsOf(value).Any(x => Regex.IsMatch(x, @"\$[2-9]")):
                    add("own.name.group2plus");
                    break;
            }
        }

        foreach (var text in AllStrings(entry))
        {
            Placeholders(text, add);
        }
    }

    private static void Exception(JsonNode? exception, Action<string> add)
    {
        string? match;
        switch (exception)
        {
            case JsonValue value when value.TryGetValue<string>(out var text):
                add("exception.string");
                match = text;
                break;
            case JsonObject obj:
                add("exception.object");
                match = obj.Text("match");
                if (string.IsNullOrWhiteSpace(obj.Text("reason")))
                {
                    add("exception.noReason");
                }

                break;
            default:
                add("exception.invalid");
                return;
        }

        if (match is null)
        {
            add("exception.noMatch");
            return;
        }

        // As Common.Drift's Glob.LooksLikeGlob decides.
        add(match.IndexOfAny(['/', '\\', '*', '?']) >= 0 || match.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
            ? "exception.file"
            : match.Contains('.') ? "exception.member" : "exception.typeOrName");
    }

    /// <summary>A find path's steps, as Common.Drift's FindPath.Parse reads them.</summary>
    private static void Steps(string path, string prefix, Action<string> add)
    {
        foreach (var part in path.Split(" > "))
        {
            var step = part.Trim();
            var kindEnd = 0;
            while (kindEnd < step.Length && char.IsLetter(step[kindEnd]))
            {
                kindEnd++;
            }

            var kind = step[..kindEnd];
            var rest = step[kindEnd..];
            if (!StepKinds.Contains(kind))
            {
                add($"{prefix}.invalid");
            }
            else if (rest.Length == 0)
            {
                add($"{prefix}.bare.{kind}");
            }
            else if (rest.StartsWith(':'))
            {
                add(kind == "method" && rest.Contains('(') ? $"{prefix}.method.overload" : $"{prefix}.{kind}");
            }
            else if (rest.StartsWith(" with ", StringComparison.Ordinal))
            {
                add($"{prefix}.with.{kind}");
            }
            else
            {
                add($"{prefix}.invalid");
            }
        }
    }

    private static void Regexes(string text, Action<string> add)
    {
        if (text.Contains("(?<=", StringComparison.Ordinal) || text.Contains("(?<!", StringComparison.Ordinal))
        {
            add("regex.lookbehind");
        }

        if (BackReference.IsMatch(text))
        {
            add("regex.backreference");
        }

        if (text.Contains("(?i)", StringComparison.Ordinal))
        {
            add("regex.inlineIgnoreCase");
        }

        if (text.Contains("(?i:", StringComparison.Ordinal))
        {
            add("regex.scopedIgnoreCase");
        }

        if (NamedGroup.IsMatch(text))
        {
            add("regex.namedGroup");
        }
    }

    private static void Placeholders(string text, Action<string> add)
    {
        foreach (Match match in Placeholder.Matches(text))
        {
            var filters = match.Groups["filters"].Value.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var filter in filters)
            {
                var name = filter.Split(':', 2)[0];
                add(Filters.Contains(name) ? $"filter.{name}" : "filter.unknown");
            }

            if (filters.Length >= 2)
            {
                add($"filter.chain.{filters.Length}");
            }
        }
    }

    private static IEnumerable<string> StringsOf(JsonNode? node)
    {
        return node switch
        {
            JsonValue value when value.TryGetValue<string>(out var text) => [text],
            JsonArray array => array.OfType<JsonValue>().Select(x => x.TryGetValue<string>(out var text) ? text : null).OfType<string>(),
            _ => []
        };
    }

    private static IEnumerable<string> AllStrings(JsonNode? node)
    {
        return node switch
        {
            JsonValue value when value.TryGetValue<string>(out var text) => [text],
            JsonArray array => array.SelectMany(AllStrings),
            JsonObject obj => obj.SelectMany(x => AllStrings(x.Value)),
            _ => []
        };
    }

    /// <summary>One of the format's keywords for <paramref name="property"/>; anything else is <c>other</c>, so no recipe text leaks into a key.</summary>
    private static string Keyword(string property, JsonNode? value)
    {
        return value is JsonValue v && v.TryGetValue<string>(out var text) && Keywords[property].Contains(text) ? text : "other";
    }

    private static string Safe(string name) => Word.IsMatch(name) ? name : "other";
}
