using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>
/// <c>{Name}</c> placeholders, optionally filtered and chained (<c>{Name|trimEnd:Service}Controller</c>, <c>{Name|camel}</c>):
/// the placeholder filters of rules.md, Find paths, worded as Common.Drift's. A placeholder with no value is left as written.
/// </summary>
internal static class Naming
{
    private static readonly Regex Placeholder = new(@"\{(\w+)((?:\|[^|{}]+)*)\}", RegexOptions.CultureInvariant);

    public static string Fill(string text, IReadOnlyDictionary<string, string> values) => Placeholder.Replace(text, x =>
        values.TryGetValue(x.Groups[1].Value, out var value)
            ? x.Groups[2].Value.Split('|', StringSplitOptions.RemoveEmptyEntries).Aggregate(value, Filter)
            : x.Value);

    // A brace a backslash escapes is a regular expression's own: \{ is a literal brace.
    private static readonly Regex Unescaped = new(@"(?<!\\)\{(\w+)((?:\|[^|{}]+)*)\}", RegexOptions.CultureInvariant);

    /// <summary>
    /// As <see cref="Fill(string, IReadOnlyDictionary{string, string})"/>, each value escaped for where it goes, such as
    /// <see cref="Regex.Escape"/> for a find path's regular expression; a brace a backslash escapes is no placeholder.
    /// </summary>
    public static string Fill(string text, IReadOnlyDictionary<string, string> values, Func<string, string> escape) => Unescaped.Replace(text, x =>
        values.TryGetValue(x.Groups[1].Value, out var value)
            ? escape(x.Groups[2].Value.Split('|', StringSplitOptions.RemoveEmptyEntries).Aggregate(value, Filter))
            : x.Value);

    /// <summary>
    /// The name a model name has in the code, through the first of a language profile's <c>naming</c> patterns for these
    /// kinds of code (<c>type</c>, <c>interface</c>, <c>property</c>, <c>field</c>, <c>method</c>, ...), or as modelled when
    /// the profile has none.
    /// </summary>
    public static string InCode(JsonNode profile, string name, IReadOnlyList<string> kinds) =>
        kinds.Select(x => profile["naming"]?[x]?.GetValue<string>()).FirstOrDefault(x => x is not null) is { } pattern
            ? Fill(pattern, new Dictionary<string, string> { ["Name"] = name })
            : name;

    private static string Filter(string value, string filter)
    {
        var colon = filter.IndexOf(':');
        var (name, argument) = colon < 0 ? (filter, "") : (filter[..colon], filter[(colon + 1)..]);
        return name switch
        {
            "lower" => value.ToLowerInvariant(),
            "upper" => value.ToUpperInvariant(),
            "camel" => value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..],
            "pascal" => value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..],
            "kebab" => Kebab(value),
            "snake" => Kebab(value).Replace('-', '_'),
            "trimStart" => argument.Length > 0 && value.StartsWith(argument, StringComparison.Ordinal) ? value[argument.Length..] : value,
            "trimEnd" => argument.Length > 0 && value.EndsWith(argument, StringComparison.Ordinal) ? value[..^argument.Length] : value,
            "replace" => argument.IndexOf(':') is var at and > 0 ? value.Replace(argument[..at], argument[(at + 1)..], StringComparison.Ordinal) : value,
            _ => throw new InvalidOperationException($"'{name}' isn't a placeholder filter (filters: lower, upper, camel, pascal, kebab, snake, trimStart:<text>, trimEnd:<text>, replace:<text>:<with>).")
        };
    }

    /// <summary><c>CustomerAddPage</c> as <c>customer-add-page</c>; a run of capitals ends one word before the capital that starts the next.</summary>
    private static string Kebab(string value)
    {
        var words = Regex.Replace(value, @"(\p{Lu}+)(\p{Lu}\p{Ll})", "$1-$2");
        words = Regex.Replace(words, @"([\p{Ll}\d])(\p{Lu})", "$1-$2");
        return Regex.Replace(words, @"[_\s]", "-").ToLowerInvariant();
    }
}
