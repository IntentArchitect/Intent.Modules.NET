using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// A node of TypeScript's own syntax tree, as its parser gives it (<c>ts.createSourceFile</c>): its <c>SyntaxKind</c>'s name
/// and its properties by TypeScript's own names (<c>name</c>, <c>type</c>, <c>members</c>, <c>body</c>, ...), so an
/// extension reads it as TypeScript's API documents it.
/// </summary>
public sealed class TypeScriptNode
{
    private readonly Dictionary<string, TypeScriptNode> _nodes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IReadOnlyList<TypeScriptNode>> _lists = new(StringComparer.Ordinal);
    private List<TypeScriptNode>? _children;

    internal TypeScriptNode(TypeScriptFile file, string kind, int start, int end)
    {
        File = file;
        Kind = kind;
        Start = start;
        End = end;
    }

    /// <summary>Its <c>SyntaxKind</c>'s name: <c>ClassDeclaration</c>, <c>Identifier</c>, <c>CallExpression</c>.</summary>
    public string Kind { get; }

    /// <summary>Where it starts in <see cref="TypeScriptFile.Text"/>, comments and whitespace before it excluded.</summary>
    public int Start { get; }

    public int End { get; }

    /// <summary>An identifier's name; a literal's or a template part's value, with its escapes resolved.</summary>
    public string? Value { get; internal init; }

    /// <summary>
    /// The <c>SyntaxKind</c> name of a prefix or postfix operator (<c>MinusToken</c>), a type operator (<c>KeyOfKeyword</c>),
    /// a heritage clause's keyword (<c>ExtendsKeyword</c>) or <c>import.meta</c>'s.
    /// </summary>
    public string? Operator { get; internal init; }

    /// <summary>How a variable list is declared (<c>const</c>, <c>let</c>, <c>var</c>, <c>using</c>), or a module (<c>namespace</c>, <c>module</c>, <c>global</c>).</summary>
    public string? Declared { get; internal init; }

    /// <summary>Its properties that are <c>true</c>: <c>isTypeOnly</c> on an <c>import type</c>'s clause.</summary>
    public IReadOnlySet<string> Flags { get; internal init; } = new HashSet<string>();

    public TypeScriptNode? Parent { get; internal set; }

    public TypeScriptFile File { get; }

    /// <summary>The node a property holds: <c>node["name"]</c>; null when it holds none.</summary>
    public TypeScriptNode? this[string property] => _nodes.GetValueOrDefault(property);

    /// <summary>The nodes a list property holds: <c>node.List("members")</c>; empty when it holds none.</summary>
    public IReadOnlyList<TypeScriptNode> List(string property) => _lists.GetValueOrDefault(property) ?? [];

    /// <summary>Every node it holds, in the order they are written.</summary>
    public IReadOnlyList<TypeScriptNode> Children => _children ??= _nodes.Values.Concat(_lists.Values.SelectMany(x => x)).OrderBy(x => x.Start).ToList();

    /// <summary>The nodes inside it, depth first, in the order they are written.</summary>
    public IEnumerable<TypeScriptNode> Descendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var nested in child.Descendants())
            {
                yield return nested;
            }
        }
    }

    public IEnumerable<TypeScriptNode> Ancestors()
    {
        for (var at = Parent; at is not null; at = at.Parent)
        {
            yield return at;
        }
    }

    public bool Is(string kind) => Kind == kind;

    /// <summary>It has the modifier, by its kind: <c>ExportKeyword</c>, <c>StaticKeyword</c>.</summary>
    public bool Has(string modifier) => List("modifiers").Any(x => x.Kind == modifier);

    /// <summary>The node as written.</summary>
    public string Source => File.Text[Start..End];

    /// <summary>Its canonical text (code-reader.md, Canonical text): its tokens, with only the spaces words need.</summary>
    public string Canonical => File.Canonical(Start, End);

    public Span Span => File.SpanOf(Start, End);

    internal void Add(string property, TypeScriptNode node)
    {
        node.Parent = this;
        _nodes[property] = node;
    }

    internal void Add(string property, IReadOnlyList<TypeScriptNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.Parent = this;
        }

        _lists[property] = nodes;
    }

    public override string ToString() => $"{Kind} {Source}";
}

/// <summary>A file TypeScript's parser read: its tree, its tokens and what the parser couldn't read.</summary>
public sealed class TypeScriptFile
{
    private readonly int[] _lines;
    private readonly IReadOnlyList<(int Start, int End)> _tokens;

    internal TypeScriptFile(string path, string text, IReadOnlyList<(int Start, int End)> tokens)
    {
        Path = path;
        Text = text;
        _tokens = tokens;
        var lines = new List<int> { 0 };
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                lines.Add(i + 1);
            }
        }

        _lines = [..lines];
    }

    /// <summary>Its path relative to the output folder, with <c>/</c> separators.</summary>
    public string Path { get; }

    /// <summary>What the parser read: the file, or a <c>.vue</c> file with all but its scripts blanked, so positions are the file's.</summary>
    public string Text { get; }

    /// <summary>Its <c>SourceFile</c>: <c>Root.List("statements")</c>.</summary>
    public TypeScriptNode Root { get; internal set; } = null!;

    /// <summary>What the parser couldn't read, with where; empty when it read it all.</summary>
    public IReadOnlyList<string> Errors { get; internal init; } = [];

    /// <summary>The canonical text of the code between two positions: its tokens, with only the spaces words need.</summary>
    public string Canonical(int start, int end)
    {
        // The tokens are in order: the first at or after the start, up to the end.
        var (low, high) = (0, _tokens.Count);
        while (low < high)
        {
            var middle = (low + high) / 2;
            (low, high) = _tokens[middle].Start < start ? (middle + 1, high) : (low, middle);
        }

        return CanonicalText.Of(_tokens.Skip(low).TakeWhile(x => x.End <= end).Select(x => (Text[x.Start..x.End], false)));
    }

    /// <summary>Where code is, as findings give it: 1-based lines and columns.</summary>
    public Span SpanOf(int start, int end)
    {
        var (line, column) = LineOf(start);
        var (endLine, endColumn) = LineOf(end);
        return new Span(Path, line, column, endLine, endColumn);
    }

    private (int Line, int Column) LineOf(int position)
    {
        var line = Array.BinarySearch(_lines, position);
        if (line < 0)
        {
            line = ~line - 1;
        }

        return (line + 1, position - _lines[line] + 1);
    }
}
