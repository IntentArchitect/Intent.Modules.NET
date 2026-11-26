using System.Text;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Jinja;

/// <summary>A <c>{% block name %}...{% endblock %}</c>, with its body as written.</summary>
internal sealed record JinjaBlock(string Name, int Line, string Body);

/// <summary>An HTML element's start tag, with each attribute's value and the Jinja expressions written in it.</summary>
internal sealed record HtmlTag(string Name, int Line, IReadOnlyDictionary<string, HtmlAttribute> Attributes)
{
    public HtmlAttribute? this[string name] => Attributes.GetValueOrDefault(name);
}

/// <summary>An attribute's value as written (<see cref="Raw"/>), and each <c>{{ ... }}</c> in it parsed as a Python expression.</summary>
internal sealed record HtmlAttribute(string Name, string? Raw, IReadOnlyList<PythonExpression> Expressions)
{
    /// <summary>The <c>url_for(name, **params)</c> (or <c>request.url_for</c>) the value is built from: the route's name and the parameters given.</summary>
    public (string Name, IReadOnlyList<string> Parameters, IReadOnlyDictionary<string, PythonExpression> Values)? UrlFor =>
        Expressions.SelectMany(x => x.Descendants().Prepend(x)).OfType<PythonCall>().Select(JinjaTemplate.UrlFor).FirstOrDefault(x => x is not null);

    /// <summary>The value when it is plain text, with no expression in it.</summary>
    public string? Text => Raw is not null && Expressions.Count == 0 && !Raw.Contains("{{", StringComparison.Ordinal) && !Raw.Contains("{%", StringComparison.Ordinal) ? Raw : null;
}

/// <summary>
/// A Jinja template, parsed: its <c>{% extends %}</c>, <c>{% include %}</c>s and blocks, and the HTML start tags with the
/// <c>{{ ... }}</c> expressions in their attributes, which Python's parser reads (Jinja's expressions are Python's, filters
/// aside). Its own tokenizer for Jinja's delimiters and HTML's tags; Django's and Nunjucks' templates share the syntax it reads.
/// </summary>
internal sealed class JinjaTemplate
{
    public required string Path { get; init; }

    public string? Extends { get; private set; }

    public int ExtendsLine { get; private set; }

    public List<(string Name, int Line)> Includes { get; } = [];

    public List<JinjaBlock> Blocks { get; } = [];

    public List<HtmlTag> Tags { get; } = [];

    /// <summary>Its name as the User Interface designer names a page: its file's, without the extension.</summary>
    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

    public JinjaBlock? Block(string name) => Blocks.FirstOrDefault(x => x.Name == name);

    /// <summary>Whether a template name, as <c>{% extends %}</c> or <c>TemplateResponse</c> gives it relative to a templates folder, is this one.</summary>
    public bool Is(string name) => Path == name || Path.EndsWith("/" + name.TrimStart('/'), StringComparison.Ordinal);

    /// <summary>A file Jinja's own extension names as a template: <c>.jinja</c>, <c>.jinja2</c>, <c>.j2</c>.</summary>
    public static bool IsJinjaFile(string path) =>
        path.EndsWith(".jinja", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".jinja2", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".j2", StringComparison.OrdinalIgnoreCase);

    /// <summary>A <c>url_for('name', param=...)</c> call: the route's name, and the parameters' names and values.</summary>
    public static (string Name, IReadOnlyList<string> Parameters, IReadOnlyDictionary<string, PythonExpression> Values)? UrlFor(PythonCall call) =>
        call.Function is PythonName { Id: "url_for" } or PythonAttribute { Name: "url_for" } && call.Arguments.FirstOrDefault(x => x.Name is null)?.Value is PythonString { Value: { } name }
            ? (name, call.Arguments.Where(x => x.Name is not null).Select(x => x.Name!).ToList(), call.Arguments.Where(x => x.Name is not null).ToDictionary(x => x.Name!, x => x.Value))
            : null;

    public static JinjaTemplate Parse(string path, string text)
    {
        var template = new JinjaTemplate { Path = path };
        var open = new Stack<(string Name, int Line, int Start)>();
        var i = 0;
        while (i < text.Length)
        {
            if (At(text, i, "{#"))
            {
                i = Skip(text, i, "#}");
            }
            else if (At(text, i, "{%"))
            {
                var end = text.IndexOf("%}", i + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    break;
                }

                var statement = text[(i + 2)..end].Trim().Trim('-', '+').Trim();
                var line = LineOf(text, i);
                var keyword = statement.Split((char[]?)null, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                var rest = statement.Length > keyword.Length ? statement[keyword.Length..].Trim() : "";
                switch (keyword)
                {
                    case "extends":
                        template.Extends = Named(rest);
                        template.ExtendsLine = line;
                        break;
                    case "include":
                        if (Named(rest.Split(" ignore missing")[0].Split(" with ")[0]) is { } included)
                        {
                            template.Includes.Add((included, line));
                        }

                        break;
                    case "block":
                        open.Push((rest.Split(' ')[0], line, end + 2));
                        break;
                    case "endblock" when open.Count > 0:
                    {
                        var block = open.Pop();
                        template.Blocks.Add(new JinjaBlock(block.Name, block.Line, text[block.Start..i]));
                        break;
                    }
                }

                i = end + 2;
            }
            else if (At(text, i, "{{"))
            {
                i = Skip(text, i, "}}");
            }
            else if (At(text, i, "<!--"))
            {
                i = Skip(text, i, "-->");
            }
            else if (text[i] == '<' && i + 1 < text.Length && char.IsLetter(text[i + 1]))
            {
                i = Tag(text, i, template.Tags);
            }
            else
            {
                i++;
            }
        }

        return template;
    }

    /// <summary>A block's text with its HTML removed, its expressions as <c>{...}</c> and its statements left out: what it shows.</summary>
    public static (string Text, bool Static) Shown(string body)
    {
        var text = new StringBuilder();
        var dynamic = false;
        var i = 0;
        while (i < body.Length)
        {
            if (At(body, i, "{{"))
            {
                var end = body.IndexOf("}}", i + 2, StringComparison.Ordinal);
                text.Append('{').Append(end < 0 ? "" : body[(i + 2)..end].Trim()).Append('}');
                dynamic = true;
                i = end < 0 ? body.Length : end + 2;
            }
            else if (At(body, i, "{%"))
            {
                dynamic = true;
                i = Skip(body, i, "%}");
            }
            else if (At(body, i, "{#"))
            {
                i = Skip(body, i, "#}");
            }
            else if (body[i] == '<')
            {
                i = Skip(body, i, ">");
            }
            else
            {
                text.Append(body[i++]);
            }
        }

        return (System.Text.RegularExpressions.Regex.Replace(System.Net.WebUtility.HtmlDecode(text.ToString()), @"\s+", " ").Trim(), !dynamic);
    }

    /// <summary>A template's name as a statement gives it: a string literal, else as written.</summary>
    private static string? Named(string written) =>
        PythonParser.Expression(written.Trim()) is PythonString { Value: { } name } ? name : written.Trim().Trim('"', '\'') is { Length: > 0 } bare ? bare : null;

    private static int Tag(string text, int start, List<HtmlTag> tags)
    {
        var line = LineOf(text, start);
        var i = start + 1;
        var nameStart = i;
        while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '-'))
        {
            i++;
        }

        var name = text[nameStart..i].ToLowerInvariant();
        var attributes = new Dictionary<string, HtmlAttribute>(StringComparer.OrdinalIgnoreCase);
        while (i < text.Length && text[i] != '>')
        {
            if (char.IsWhiteSpace(text[i]) || text[i] == '/')
            {
                i++;
                continue;
            }

            if (At(text, i, "{{") || At(text, i, "{%"))
            {
                // A Jinja expression or statement between attributes: {{ "selected" if ... }}.
                i = Skip(text, i, At(text, i, "{{") ? "}}" : "%}");
                continue;
            }

            var attributeStart = i;
            while (i < text.Length && !char.IsWhiteSpace(text[i]) && text[i] != '=' && text[i] != '>' && text[i] != '/')
            {
                i++;
            }

            var attribute = text[attributeStart..i];
            string? raw = null;
            if (i < text.Length && text[i] == '=')
            {
                i++;
                if (i < text.Length && text[i] is '"' or '\'')
                {
                    var quote = text[i++];
                    var valueStart = i;
                    while (i < text.Length && text[i] != quote)
                    {
                        i = At(text, i, "{{") ? Skip(text, i, "}}") : At(text, i, "{%") ? Skip(text, i, "%}") : i + 1;
                    }

                    raw = text[valueStart..Math.Min(i, text.Length)];
                    i++;
                }
                else
                {
                    var valueStart = i;
                    while (i < text.Length && !char.IsWhiteSpace(text[i]) && text[i] != '>')
                    {
                        i++;
                    }

                    raw = text[valueStart..i];
                }
            }

            if (attribute.Length > 0)
            {
                attributes[attribute] = new HtmlAttribute(attribute, raw, Expressions(raw));
            }
        }

        tags.Add(new HtmlTag(name, line, attributes));
        i++;
        if (name is "script" or "style")
        {
            var close = text.IndexOf("</" + name, i, StringComparison.OrdinalIgnoreCase);
            i = close < 0 ? text.Length : close;
        }

        return i;
    }

    private static IReadOnlyList<PythonExpression> Expressions(string? raw)
    {
        var expressions = new List<PythonExpression>();
        var i = 0;
        while (raw is not null && (i = raw.IndexOf("{{", i, StringComparison.Ordinal)) >= 0)
        {
            var end = raw.IndexOf("}}", i + 2, StringComparison.Ordinal);
            if (end < 0)
            {
                break;
            }

            if (PythonParser.Expression(raw[(i + 2)..end].Trim().Trim('-', '+').Trim()) is { } expression)
            {
                expressions.Add(expression);
            }

            i = end + 2;
        }

        return expressions;
    }

    private static bool At(string text, int i, string token) => string.CompareOrdinal(text, i, token, 0, token.Length) == 0;

    private static int Skip(string text, int i, string close)
    {
        var end = text.IndexOf(close, i + 2, StringComparison.Ordinal);
        return end < 0 ? text.Length : end + close.Length;
    }

    private static int LineOf(string text, int index)
    {
        var line = 1;
        for (var j = 0; j < index && j < text.Length; j++)
        {
            if (text[j] == '\n')
            {
                line++;
            }
        }

        return line;
    }
}
