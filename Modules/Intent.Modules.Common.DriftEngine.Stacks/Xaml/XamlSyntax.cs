using System.Text;
using System.Xml;
using System.Xml.Linq;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.Xaml;

/// <summary>
/// What the XAML language read of each in-scope view, kept on the graph for every extension that reads XAML (.NET MAUI's, a
/// WPF, WinUI or Avalonia stack's, a module's): the view's XML document, with where each node is written. A view is a XAML
/// file with an <c>x:Class</c>, the markup half of that class: the C# language reads its code-behind. Get it with
/// <see cref="Of"/>; null when no XAML file is in scope.
/// </summary>
public sealed class XamlSyntax
{
    internal XamlSyntax()
    {
    }

    /// <summary>Each in-scope XAML view, by path relative to the output folder.</summary>
    public SortedDictionary<string, XamlDocument> Documents { get; } = new(StringComparer.Ordinal);

    public static XamlSyntax? Of(CodeGraph graph) => graph.Get<XamlSyntax>();

    /// <summary>The view whose <c>x:Class</c> the type is, once the XAML language's code reader has joined them; null for none.</summary>
    public XamlDocument? DocumentOf(TypeNode? type) => type is null ? null : Documents.Values.FirstOrDefault(x => x.Type == type);
}

/// <summary>One XAML view: its file, its <c>x:Class</c> and its root element, each node with where it is written.</summary>
public sealed class XamlDocument
{
    /// <summary>The XAML language's own namespaces, the <c>x:</c> prefix's: XAML 2006 (WPF, WinUI, Avalonia) and XAML 2009 (.NET MAUI, Xamarin.Forms).</summary>
    public static readonly IReadOnlySet<string> LanguageNamespaces = new HashSet<string>(StringComparer.Ordinal)
    {
        "http://schemas.microsoft.com/winfx/2006/xaml",
        "http://schemas.microsoft.com/winfx/2009/xaml"
    };

    private readonly string _text;
    private readonly IReadOnlyDictionary<XElement, (int Start, int End)> _elements;
    private readonly int[] _lines;

    internal XamlDocument(string path, string xamlClass, XElement root, string text, IReadOnlyDictionary<XElement, (int Start, int End)> elements, int[] lines)
    {
        Path = path;
        Class = xamlClass;
        Root = root;
        _text = text;
        _elements = elements;
        _lines = lines;
    }

    /// <summary>The view's file, relative to the output folder, with <c>/</c> separators.</summary>
    public string Path { get; }

    /// <summary>Its <c>x:Class</c>: its code-behind's full name.</summary>
    public string Class { get; }

    /// <summary>Its root element, whose type is its code-behind's base.</summary>
    public XElement Root { get; }

    /// <summary>Its code-behind, the type its <c>x:Class</c> names, once the XAML language's code reader has found it.</summary>
    public TypeNode? Type { get; internal set; }

    /// <summary>
    /// Where a node is written: an element from its start tag to its end tag, an attribute or any other node where it starts.
    /// </summary>
    public Span At(XObject node)
    {
        if (node is XElement element && _elements.TryGetValue(element, out var range))
        {
            var (line, column) = Position(range.Start);
            var (endLine, endColumn) = Position(range.End);
            return new Span(Path, line, column, endLine, endColumn + 1);
        }

        var info = (IXmlLineInfo)node;
        return info.HasLineInfo() ? new Span(Path, info.LineNumber, info.LinePosition) : new Span(Path, 1);
    }

    /// <summary>An element as written, from its start tag to its end tag.</summary>
    public string TextOf(XElement element) => _elements.TryGetValue(element, out var range) ? _text[range.Start..(range.End + 1)] : element.ToString();

    /// <summary>
    /// The type an object element creates: its local name in its namespace, with the CLR namespace a <c>clr-namespace:</c> or
    /// <c>using:</c> namespace gives (a framework's namespace, such as .NET MAUI's, gives none). Null for a property element
    /// (<c>&lt;ContentPage.BindingContext&gt;</c>) and for the XAML language's own (<c>x:Array</c>, <c>x:String</c>).
    /// </summary>
    public static XamlTypeName? TypeOf(XElement element) =>
        element.Name.LocalName.Contains('.') || LanguageNamespaces.Contains(element.Name.NamespaceName)
            ? null
            : new XamlTypeName(element.Name.LocalName, ClrNamespace(element.Name.NamespaceName), Written(element.Name, element));

    /// <summary>
    /// The type a name written in an attribute or a markup extension stands for (<c>views:LoginView</c>, <c>Label</c>), read
    /// against the prefixes in scope at <paramref name="context"/>; null for the XAML language's own and for a prefix nothing
    /// declares.
    /// </summary>
    public static XamlTypeName? TypeOf(string written, XElement context)
    {
        written = written.Trim();
        if (written.Length == 0)
        {
            return null;
        }

        var colon = written.IndexOf(':');
        var ns = colon < 0 ? context.GetDefaultNamespace() : context.GetNamespaceOfPrefix(written[..colon]);
        if (ns is null || LanguageNamespaces.Contains(ns.NamespaceName))
        {
            return null;
        }

        return new XamlTypeName(written[(colon + 1)..], ClrNamespace(ns.NamespaceName), written);
    }

    /// <summary>An element's or attribute's name with the prefix its file writes it with: <c>views:BadgeView</c>, <c>x:Name</c>, <c>Grid.Row</c>.</summary>
    public static string Written(XName name, XElement scope)
    {
        if (name.NamespaceName.Length == 0)
        {
            return name.LocalName;
        }

        var prefix = scope.GetPrefixOfNamespace(name.Namespace);
        return string.IsNullOrEmpty(prefix) ? name.LocalName : $"{prefix}:{name.LocalName}";
    }

    /// <summary>
    /// The CLR namespace a XAML namespace maps to: <c>clr-namespace:Shop.Views;assembly=Shop</c> (WPF, .NET MAUI) and
    /// <c>using:Shop.Views</c> (WinUI, Avalonia) give <c>Shop.Views</c>; a URI gives none.
    /// </summary>
    public static string? ClrNamespace(string xmlns)
    {
        foreach (var scheme in new[] { "clr-namespace:", "using:" })
        {
            if (xmlns.StartsWith(scheme, StringComparison.Ordinal))
            {
                var value = xmlns[scheme.Length..];
                var semicolon = value.IndexOf(';');
                return (semicolon < 0 ? value : value[..semicolon]).Trim();
            }
        }

        return null;
    }

    /// <summary>The 1-based line and column of an offset in the file.</summary>
    private (int Line, int Column) Position(int offset)
    {
        var index = Array.BinarySearch(_lines, offset);
        var line = index >= 0 ? index : ~index - 1;
        return (line + 1, offset - _lines[line] + 1);
    }

    /// <summary>
    /// Reads a XAML file: its document, with line information, and each element's start and end in the text. An element's start
    /// tag ends at its first <c>&gt;</c> outside a quoted value, as XML's values are always quoted and never hold a <c>&lt;</c>.
    /// </summary>
    internal static (XDocument Document, IReadOnlyDictionary<XElement, (int Start, int End)> Elements, int[] Lines) Parse(string text)
    {
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
        XDocument document;
        using (var reader = XmlReader.Create(new StringReader(text), settings))
        {
            document = XDocument.Load(reader, LoadOptions.SetLineInfo);
        }

        var lines = Lines(text);
        var starts = new List<int>();
        var ends = new Dictionary<int, int>();
        var open = new Stack<int>();
        using (var reader = XmlReader.Create(new StringReader(text), settings))
        {
            var info = (IXmlLineInfo)reader;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var start = TagStart(text, lines, info.LineNumber, info.LinePosition);
                    var index = starts.Count;
                    starts.Add(start);
                    if (reader.IsEmptyElement)
                    {
                        ends[index] = TagEnd(text, start);
                    }
                    else
                    {
                        open.Push(index);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && open.Count > 0)
                {
                    ends[open.Pop()] = TagEnd(text, TagStart(text, lines, info.LineNumber, info.LinePosition));
                }
            }
        }

        var elements = new Dictionary<XElement, (int Start, int End)>();
        var all = document.Root is null ? [] : document.Root.DescendantsAndSelf().ToList();
        if (all.Count == starts.Count)
        {
            for (var i = 0; i < all.Count; i++)
            {
                elements[all[i]] = (starts[i], ends.GetValueOrDefault(i, TagEnd(text, starts[i])));
            }
        }

        return (document, elements, lines);
    }

    /// <summary>The offset each line starts at; a line ends at <c>\n</c>, <c>\r\n</c> or <c>\r</c>, as XML counts them.</summary>
    private static int[] Lines(string text)
    {
        var lines = new List<int> { 0 };
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n' || text[i] == '\r' && (i + 1 >= text.Length || text[i + 1] != '\n'))
            {
                lines.Add(i + 1);
            }
        }

        return lines.ToArray();
    }

    /// <summary>The <c>&lt;</c> a tag the reader places at a line and column starts with: the reader places it at its name.</summary>
    private static int TagStart(string text, int[] lines, int line, int column)
    {
        var offset = Math.Min(text.Length - 1, lines[Math.Min(line, lines.Length) - 1] + column - 1);
        while (offset > 0 && text[offset] != '<')
        {
            offset--;
        }

        return offset;
    }

    /// <summary>The <c>&gt;</c> that ends the tag starting at <paramref name="start"/>.</summary>
    private static int TagEnd(string text, int start)
    {
        char? quote = null;
        for (var i = start; i < text.Length; i++)
        {
            var c = text[i];
            if (quote is not null)
            {
                quote = c == quote ? null : quote;
            }
            else if (c is '"' or '\'')
            {
                quote = c;
            }
            else if (c == '>')
            {
                return i;
            }
        }

        return text.Length - 1;
    }
}

/// <summary>
/// A type named in XAML: its name, the CLR namespace its XAML namespace maps it to (null for a framework's namespace, which
/// names the framework's own types), and the name as written.
/// </summary>
public sealed record XamlTypeName(string Name, string? Namespace, string Written)
{
    /// <summary>Its full name where its namespace is a CLR namespace, else its name.</summary>
    public string Qualified => Namespace is null ? Name : $"{Namespace}.{Name}";

    /// <summary>As a type reference, which <see cref="CodeGraph.TypeFor"/> resolves by its full name.</summary>
    public TypeRefNode Reference() => new() { Text = Written, Name = Name, QualifiedName = Qualified };
}

/// <summary>
/// A markup extension, a XAML attribute value in braces (<c>{Binding Name, Mode=TwoWay}</c>, <c>{x:Type local:Routes}</c>,
/// <c>{DataTemplate views:LoginView}</c>): its name as written, its positional and its named arguments, each a text or a
/// markup extension of its own. Parsed by the XAML grammar: values may be quoted with <c>'</c> or <c>"</c>, a <c>\</c> escapes
/// the character after it, and a value starting <c>{}</c> is text, not an extension.
/// </summary>
public sealed class XamlMarkupExtension
{
    private XamlMarkupExtension(string name, IReadOnlyList<XamlValue> positional, IReadOnlyList<(string Name, XamlValue Value)> named)
    {
        Name = name;
        Positional = positional;
        Named = named;
    }

    /// <summary>Its name as written: <c>x:Type</c>, <c>StaticResource</c>.</summary>
    public string Name { get; }

    public IReadOnlyList<XamlValue> Positional { get; }

    public IReadOnlyList<(string Name, XamlValue Value)> Named { get; }

    /// <summary>Its first positional argument's text: what <c>{x:Type local:Routes}</c> names, the key of <c>{StaticResource Accent}</c>.</summary>
    public string? Argument => Positional.FirstOrDefault()?.Text;

    /// <summary>It and the markup extensions its arguments are, at any depth, outermost first.</summary>
    public IEnumerable<XamlMarkupExtension> DescendantsAndSelf() =>
        Positional.Concat(Named.Select(x => x.Value)).Select(x => x.Extension).OfType<XamlMarkupExtension>().SelectMany(x => x.DescendantsAndSelf()).Prepend(this);

    /// <summary>The markup extension an attribute value is; null for a plain value, or one escaped with <c>{}</c>.</summary>
    public static XamlMarkupExtension? Parse(string value)
    {
        var start = 0;
        while (start < value.Length && char.IsWhiteSpace(value[start]))
        {
            start++;
        }

        if (start >= value.Length || value[start] != '{' || start + 1 < value.Length && value[start + 1] == '}')
        {
            return null;
        }

        var position = start + 1;
        return Extension(value, ref position);
    }

    /// <summary>A plain value's text: one escaped with <c>{}</c> without it.</summary>
    public static string Unescaped(string value) => value.TrimStart().StartsWith("{}", StringComparison.Ordinal) ? value.TrimStart()[2..] : value;

    /// <summary>An extension from after its <c>{</c> to after its <c>}</c>.</summary>
    private static XamlMarkupExtension? Extension(string text, ref int i)
    {
        SkipWhitespace(text, ref i);
        var name = new StringBuilder();
        while (i < text.Length && !char.IsWhiteSpace(text[i]) && text[i] is not ('}' or ','))
        {
            name.Append(text[i++]);
        }

        if (name.Length == 0)
        {
            return null;
        }

        var positional = new List<XamlValue>();
        var named = new List<(string, XamlValue)>();
        while (true)
        {
            SkipWhitespace(text, ref i);
            if (i >= text.Length)
            {
                return null;
            }

            if (text[i] == '}')
            {
                i++;
                return new XamlMarkupExtension(name.ToString(), positional, named);
            }

            var value = Value(text, ref i, out var stoppedAtEquals);
            if (value is null)
            {
                return null;
            }

            if (stoppedAtEquals)
            {
                i++;
                var assigned = Value(text, ref i, out _);
                if (assigned is null)
                {
                    return null;
                }

                named.Add((value.Text, assigned));
            }
            else
            {
                positional.Add(value);
            }

            SkipWhitespace(text, ref i);
            if (i < text.Length && text[i] == ',')
            {
                i++;
            }
        }
    }

    /// <summary>
    /// A value up to the <c>,</c>, <c>}</c> or (for a member's name) <c>=</c> after it: a nested extension, a quoted text or
    /// a bare one, trimmed.
    /// </summary>
    private static XamlValue? Value(string text, ref int i, out bool stoppedAtEquals)
    {
        stoppedAtEquals = false;
        SkipWhitespace(text, ref i);
        if (i >= text.Length)
        {
            return null;
        }

        if (text[i] == '{' && !(i + 1 < text.Length && text[i + 1] == '}'))
        {
            var start = i;
            i++;
            return Extension(text, ref i) is { } nested ? new XamlValue(text[start..i], nested) : null;
        }

        if (text[i] is '\'' or '"')
        {
            var quote = text[i++];
            var quoted = new StringBuilder();
            while (i < text.Length && text[i] != quote)
            {
                if (text[i] == '\\' && i + 1 < text.Length)
                {
                    i++;
                }

                quoted.Append(text[i++]);
            }

            i++;
            return new XamlValue(quoted.ToString(), null);
        }

        var bare = new StringBuilder();
        while (i < text.Length && text[i] is not (',' or '}'))
        {
            if (text[i] == '=')
            {
                stoppedAtEquals = true;
                break;
            }

            if (text[i] == '\\' && i + 1 < text.Length)
            {
                i++;
            }

            bare.Append(text[i++]);
        }

        return new XamlValue(bare.ToString().Trim(), null);
    }

    private static void SkipWhitespace(string text, ref int i)
    {
        while (i < text.Length && char.IsWhiteSpace(text[i]))
        {
            i++;
        }
    }
}

/// <summary>A markup extension's argument: its text, and the markup extension it is, if it is one.</summary>
public sealed record XamlValue(string Text, XamlMarkupExtension? Extension);
