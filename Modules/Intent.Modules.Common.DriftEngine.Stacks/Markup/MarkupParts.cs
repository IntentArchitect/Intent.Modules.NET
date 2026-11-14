using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.Markup;

/// <summary>
/// What every language's markup has in common, for its reader: an element's content and an attribute's value read from
/// their parts, text and values' code, into the literals <see cref="MarkupNode"/> and <see cref="MarkupAttributeNode"/>
/// give.
/// </summary>
internal static class MarkupParts
{
    /// <summary>A part of an element's content or an attribute's value: text as written, or a value's code.</summary>
    public sealed record Part(bool Code, string Text, Span At);

    /// <summary>
    /// An element's content: its text trimmed, each run of whitespace in it reduced to one space and its character references
    /// resolved; a <c>string</c> literal for text alone, else <c>interpolated</c>.
    /// </summary>
    public static LiteralNode Content(IReadOnlyList<Part> parts)
    {
        var normalised = new List<Part>();
        foreach (var part in Merged(parts))
        {
            normalised.Add(part.Code ? part with { Text = Collapse(part.Text).Trim() } : part with { Text = Collapse(part.Text) });
        }

        if (normalised is [{ Code: false } first, ..])
        {
            normalised[0] = first with { Text = first.Text.TrimStart() };
        }

        if (normalised is [.., { Code: false } last])
        {
            normalised[^1] = last with { Text = last.Text.TrimEnd() };
        }

        return Literal(normalised.Where(x => x.Code || x.Text.Length > 0).ToList());
    }

    /// <summary>An attribute's value: a <c>string</c> literal for text alone, its character references resolved, else <c>interpolated</c>.</summary>
    public static LiteralNode Value(IReadOnlyList<Part> parts) => Literal(Merged(parts).Where(x => x.Code || x.Text.Length > 0).ToList());

    private static LiteralNode Literal(IReadOnlyList<Part> parts)
    {
        if (parts.All(x => !x.Code))
        {
            return new LiteralNode { Kind = "string", Value = Decode(string.Concat(parts.Select(x => x.Text))) };
        }

        return new LiteralNode
        {
            Kind = "interpolated",
            Parts = parts.Select(x => x.Code
                    ? new ValueNode { Text = x.Text, At = x.At }
                    : new ValueNode { Text = x.Text, Literal = new LiteralNode { Kind = "string", Value = Decode(x.Text) }, At = x.At })
                .ToList()
        };
    }

    /// <summary>Adjacent text parts as one, as a reader may give a run of text in pieces.</summary>
    private static IEnumerable<Part> Merged(IReadOnlyList<Part> parts)
    {
        Part? text = null;
        foreach (var part in parts)
        {
            if (!part.Code)
            {
                text = text is null ? part : text with { Text = text.Text + part.Text, At = text.At with { EndLine = part.At.EndLine, EndColumn = part.At.EndColumn } };
                continue;
            }

            if (text is not null)
            {
                yield return text;
                text = null;
            }

            yield return part;
        }

        if (text is not null)
        {
            yield return text;
        }
    }

    private static string Collapse(string text) => System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");

    private static string Decode(string text) => System.Net.WebUtility.HtmlDecode(text);
}
