using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.Markup;

/// <summary>
/// The subject of a CSS selector, the last compound after any combinator (<c>.catalog-product</c> of
/// <c>:scope &gt; .catalog-product</c>): an optional tag, its classes, an id and attribute tests, matched against a markup
/// element as its language writes it. Script code passes selectors to the DOM to find the elements it acts on.
/// </summary>
internal sealed record CssSelector(string Text, string? Tag, IReadOnlyList<string> Classes, string? Id, IReadOnlyList<(string Name, string? Value)> Attributes)
{
    private static readonly Regex Part = new(
        @"^(?<tag>[A-Za-z][\w-]*|\*)|\.(?<class>[\w-]+)|#(?<id>[\w-]+)|\[\s*(?<attr>[\w:-]+)\s*(?:[~|^$*]?=\s*[""']?(?<value>[^""'\]]*)[""']?\s*)?\]|(?<pseudo>::?[\w-]+(?:\([^)]*\))?)",
        RegexOptions.Compiled);

    private const char Hole = '\u0001';

    /// <summary>Each comma-separated selector of a selector list, by its subject; one this doesn't read is left out.</summary>
    public static IEnumerable<CssSelector> ParseList(string text) => text.Split(',').Select(x => Parse(x.Trim())).OfType<CssSelector>();

    public static CssSelector? Parse(string text)
    {
        var compound = Regex.Split(text.Trim(), @"\s*[>+~]\s*|\s+").LastOrDefault(x => x.Length > 0);
        if (compound is null)
        {
            return null;
        }

        string? tag = null, id = null;
        var classes = new List<string>();
        var attributes = new List<(string, string?)>();
        var position = 0;
        foreach (Match match in Part.Matches(compound))
        {
            if (match.Index != position)
            {
                return null;
            }

            position = match.Index + match.Length;
            if (match.Groups["tag"].Success && match.Groups["tag"].Value != "*")
            {
                tag = match.Groups["tag"].Value;
            }
            else if (match.Groups["class"].Success)
            {
                classes.Add(match.Groups["class"].Value);
            }
            else if (match.Groups["id"].Success)
            {
                id = match.Groups["id"].Value;
            }
            else if (match.Groups["attr"].Success)
            {
                attributes.Add((match.Groups["attr"].Value, match.Groups["value"].Success ? match.Groups["value"].Value : null));
            }
        }

        return position == compound.Length ? new CssSelector(compound, tag, classes, id, attributes) : null;
    }

    /// <summary>
    /// It marks the elements it is written for: it names a class, an id or a <c>data-</c> attribute. A bare tag or an ARIA
    /// attribute (<c>form</c>, <c>[role="option"]</c>) matches any markup of its kind, so it marks nothing in particular.
    /// </summary>
    public bool Marks => Classes.Count > 0 || Id is not null || Attributes.Any(x => x.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// The element matches it as written: its tag, each class among the classes its <c>class</c> attribute writes out in full
    /// (one a value runs into, <c>btn-@size</c>, isn't), its literal <c>id</c>, and each attribute present with the value
    /// tested.
    /// </summary>
    public bool Matches(MarkupNode node)
    {
        if (Tag is not null && !string.Equals(node.Tag, Tag, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Classes.Count > 0)
        {
            var written = Written(Attribute(node, "class")?.Literal) ?? "";
            var tokens = written.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Where(x => !x.Contains(Hole)).ToHashSet(StringComparer.Ordinal);
            if (!Classes.All(tokens.Contains))
            {
                return false;
            }
        }

        if (Id is not null && !(Attribute(node, "id")?.Literal is { Kind: "string" } literal && literal.Value == Id))
        {
            return false;
        }

        foreach (var (name, value) in Attributes)
        {
            if (Attribute(node, name) is not { } attribute)
            {
                return false;
            }

            if (value is not null && (attribute.Literal is { Kind: "string", Value: { } written } ? written : attribute.Value) != value)
            {
                return false;
            }
        }

        return true;
    }

    private static MarkupAttributeNode? Attribute(MarkupNode node, string name) =>
        node.Attributes.FirstOrDefault(x => x.Directive is null && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

    /// <summary>An attribute's text, each value written in it a hole.</summary>
    private static string? Written(LiteralNode? literal) => literal switch
    {
        { Kind: "string", Value: { } text } => text,
        { Kind: "interpolated", Parts: { } parts } => string.Concat(parts.Select(x => x.Literal is { Kind: "string", Value: { } text } ? text : Hole.ToString())),
        _ => null
    };

    public bool Equals(CssSelector? other) => other is not null && other.Text == Text;

    public override int GetHashCode() => Text.GetHashCode(StringComparison.Ordinal);
}
