namespace Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

public enum PythonTokenKind
{
    Name,
    Number,
    String,
    Operator,
    Newline,
    Indent,
    Dedent,
    EndOfFile
}

/// <summary>
/// A token of Python source: where it is as offsets into the file's text, and as lines and 1-based columns, its end column
/// just past its last character, as findings give it.
/// </summary>
public sealed record PythonToken(PythonTokenKind Kind, string Text, int Start, int End, int Line, int Column, int EndLine, int EndColumn)
{
    /// <summary>Written in the code: not a new line, an indent, a dedent or the end of the file, which the tokenizer makes.</summary>
    public bool IsWritten => Kind is PythonTokenKind.Name or PythonTokenKind.Number or PythonTokenKind.String or PythonTokenKind.Operator;

    public bool Is(string text) => Kind is PythonTokenKind.Operator or PythonTokenKind.Name && Text == text;
}

/// <summary>A place a tokenizer or parser couldn't read, and why.</summary>
internal sealed record PythonIssue(int Line, int Column, string Message)
{
    public override string ToString() => $"line {Line}:{Column} {Message}";
}

/// <summary>
/// Python's tokenizer (the language reference's lexical analysis): names, numbers, strings with their prefixes, f-strings
/// whole with the code in their replacement fields, operators, and the new lines, indents and dedents of the logical lines.
/// Inside brackets a line break is whitespace, as is a backslash before one; comments are dropped.
/// </summary>
/// <param name="from">Where to start in <paramref name="source"/>; the tokens' offsets are the source's.</param>
/// <param name="to">Where to stop.</param>
/// <param name="expression">An f-string replacement field's code: one expression, with no lines or indentation of its own.</param>
internal sealed class PythonLexer(string source, PythonLines lines, int from, int to, bool expression)
{
    private static readonly string[] Operators3 = ["**=", "//=", ">>=", "<<=", "..."];

    private static readonly string[] Operators2 = ["->", ":=", "**", "//", "<<", ">>", "<=", ">=", "==", "!=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "@="];

    private const string Operators1 = "+-*/%@&|^~<>()[]{},:;.=!";

    private static readonly HashSet<string> Prefixes = new(StringComparer.OrdinalIgnoreCase) { "r", "u", "b", "f", "t", "br", "rb", "fr", "rf", "tr", "rt" };

    private readonly List<PythonToken> _tokens = [];

    public List<PythonIssue> Issues { get; } = [];

    public PythonLexer(string source, PythonLines lines) : this(source, lines, 0, source.Length, false)
    {
    }

    public List<PythonToken> Tokenize()
    {
        var i = from;
        var depth = expression ? 1 : 0;
        var indents = new Stack<int>([0]);
        var lineStart = !expression;
        while (true)
        {
            if (lineStart && depth == 0)
            {
                var column = 0;
                var j = i;
                while (j < to && source[j] is ' ' or '\t' or '\f')
                {
                    column = source[j] switch { '\t' => (column / 8 + 1) * 8, '\f' => 0, _ => column + 1 };
                    j++;
                }

                // A line with only a comment or whitespace is no logical line.
                if (j < to && source[j] is '#' or '\r' or '\n')
                {
                    i = SkipComment(j);
                    i = SkipLineBreak(i);
                    continue;
                }

                i = j;
                if (j >= to)
                {
                    break;
                }

                lineStart = false;
                if (column > indents.Peek())
                {
                    indents.Push(column);
                    Add(PythonTokenKind.Indent, i, i);
                }
                else
                {
                    while (column < indents.Peek())
                    {
                        indents.Pop();
                        Add(PythonTokenKind.Dedent, i, i);
                    }

                    if (column != indents.Peek())
                    {
                        Issue(i, "the indentation matches no outer level");
                    }
                }
            }

            if (i >= to)
            {
                break;
            }

            var c = source[i];
            if (c is ' ' or '\t' or '\f')
            {
                i++;
            }
            else if (c == '#')
            {
                i = SkipComment(i);
            }
            else if (c == '\\' && i + 1 < to && source[i + 1] is '\r' or '\n')
            {
                i = SkipLineBreak(i + 1);
            }
            else if (c is '\r' or '\n')
            {
                var start = i;
                i = SkipLineBreak(i);
                if (depth > 0 && !expression && StartsDeclaration(i))
                {
                    Issue(start, "a bracket is never closed");
                    depth = 0;
                }

                if (depth == 0 && !expression)
                {
                    Add(PythonTokenKind.Newline, start, i);
                    lineStart = true;
                }
            }
            else if (IsNameStart(c))
            {
                var end = i + 1;
                while (end < to && IsNamePart(source[end]))
                {
                    end++;
                }

                if (end < to && source[end] is '"' or '\'' && Prefixes.Contains(source[i..end]))
                {
                    i = String(i, end);
                }
                else
                {
                    Add(PythonTokenKind.Name, i, end);
                    i = end;
                }
            }
            else if (c is '"' or '\'')
            {
                i = String(i, i);
            }
            else if (char.IsAsciiDigit(c) || c == '.' && i + 1 < to && char.IsAsciiDigit(source[i + 1]))
            {
                i = Number(i);
            }
            else if (Operator(i) is { } op)
            {
                depth = op switch
                {
                    "(" or "[" or "{" => depth + 1,
                    ")" or "]" or "}" => Math.Max(expression ? 1 : 0, depth - 1),
                    _ => depth
                };
                Add(PythonTokenKind.Operator, i, i + op.Length);
                i += op.Length;
            }
            else
            {
                Issue(i, $"'{c}' isn't Python");
                i++;
            }
        }

        if (!expression)
        {
            if (_tokens.Count > 0 && _tokens[^1].Kind is not (PythonTokenKind.Newline or PythonTokenKind.Dedent))
            {
                Add(PythonTokenKind.Newline, to, to);
            }

            while (indents.Count > 1)
            {
                indents.Pop();
                Add(PythonTokenKind.Dedent, to, to);
            }
        }

        Add(PythonTokenKind.EndOfFile, to, to);
        return _tokens;
    }

    private string? Operator(int i)
    {
        foreach (var candidates in new[] { Operators3, Operators2 })
        {
            foreach (var op in candidates)
            {
                if (i + op.Length <= to && string.CompareOrdinal(source, i, op, 0, op.Length) == 0)
                {
                    return op;
                }
            }
        }

        return Operators1.Contains(source[i]) ? source[i].ToString() : null;
    }

    /// <summary>
    /// A line that starts, unindented, as a declaration does (<c>def</c>, <c>class</c>, <c>@</c>, <c>import</c>, <c>async</c>):
    /// inside brackets, the bracket before it was never closed, so the file is read on from there rather than as one line.
    /// </summary>
    private bool StartsDeclaration(int i)
    {
        if (i >= to || source[i] == '@')
        {
            return i < to;
        }

        var end = i;
        while (end < to && char.IsAsciiLetter(source[end]))
        {
            end++;
        }

        return end < to && source[end] == ' ' && source[i..end] is "def" or "class" or "import" or "async";
    }

    private int SkipComment(int i)
    {
        if (i < to && source[i] == '#')
        {
            while (i < to && source[i] is not ('\r' or '\n'))
            {
                i++;
            }
        }

        return i;
    }

    private int SkipLineBreak(int i) =>
        i < to && source[i] == '\r' ? i + 1 < to && source[i + 1] == '\n' ? i + 2 : i + 1
        : i < to && source[i] == '\n' ? i + 1
        : i;

    /// <summary>A decimal, hexadecimal, octal or binary integer, a float or an imaginary number, with its underscores.</summary>
    private int Number(int i)
    {
        var start = i;
        if (source[i] == '0' && i + 1 < to && source[i + 1] is 'x' or 'X' or 'o' or 'O' or 'b' or 'B')
        {
            i += 2;
            while (i < to && (char.IsAsciiHexDigit(source[i]) || source[i] == '_'))
            {
                i++;
            }
        }
        else
        {
            i = Digits(i);
            if (i < to && source[i] == '.')
            {
                i = Digits(i + 1);
            }

            if (i < to && source[i] is 'e' or 'E' && (i + 1 < to && char.IsAsciiDigit(source[i + 1]) || i + 2 < to && source[i + 1] is '+' or '-' && char.IsAsciiDigit(source[i + 2])))
            {
                i = Digits(i + 2);
            }

            if (i < to && source[i] is 'j' or 'J')
            {
                i++;
            }
        }

        Add(PythonTokenKind.Number, start, i);
        return i;
    }

    private int Digits(int i)
    {
        while (i < to && (char.IsAsciiDigit(source[i]) || source[i] == '_'))
        {
            i++;
        }

        return i;
    }

    /// <param name="start">Where its prefix starts.</param>
    /// <param name="quote">Where its opening quote is.</param>
    private int String(int start, int quote)
    {
        var formatted = source[start..quote].Any(x => x is 'f' or 'F' or 't' or 'T');
        var end = StringEnd(quote, formatted);
        if (end < 0)
        {
            Issue(start, "the string is never closed");
            end = -end;
        }

        Add(PythonTokenKind.String, start, end);
        return end;
    }

    /// <summary>
    /// Where a string that opens at <paramref name="quote"/> ends, just past its closing quote; negated when it isn't closed.
    /// A backslash keeps the character after it in the string, even in a raw string. An f-string's replacement fields are
    /// code, which can hold strings of its own with the same quotes.
    /// </summary>
    internal int StringEnd(int quote, bool formatted)
    {
        var mark = source[quote];
        var triple = quote + 2 < to && source[quote + 1] == mark && source[quote + 2] == mark;
        var i = quote + (triple ? 3 : 1);
        while (i < to)
        {
            var c = source[i];
            if (c == '\\')
            {
                i += 2;
            }
            else if (c == mark && (!triple || i + 2 < to && source[i + 1] == mark && source[i + 2] == mark))
            {
                return i + (triple ? 3 : 1);
            }
            else if (!triple && c is '\r' or '\n')
            {
                return -i;
            }
            else if (formatted && c == '{')
            {
                if (i + 1 < to && source[i + 1] == '{')
                {
                    i += 2;
                    continue;
                }

                var end = FieldEnd(i, triple);
                if (end < 0)
                {
                    return end;
                }

                i = end;
            }
            else
            {
                i++;
            }
        }

        return -Math.Min(i, to);
    }

    /// <summary>
    /// Where an f-string's replacement field that opens at <paramref name="open"/> ends, just past its <c>}</c>: its code,
    /// with brackets and strings of its own, then an optional <c>!r</c> and a format spec, which can hold fields too.
    /// Negated when the string's line ends first.
    /// </summary>
    internal int FieldEnd(int open, bool triple)
    {
        var depth = 0;
        var spec = false;
        var i = open + 1;
        while (i < to)
        {
            var c = source[i];
            if (!triple && c is '\r' or '\n')
            {
                return -i;
            }

            if (spec)
            {
                if (c == '{')
                {
                    var end = FieldEnd(i, triple);
                    if (end < 0)
                    {
                        return end;
                    }

                    i = end;
                    continue;
                }

                if (c == '}')
                {
                    return i + 1;
                }

                i++;
                continue;
            }

            switch (c)
            {
                case '"' or '\'':
                {
                    var prefix = i;
                    while (prefix > open + 1 && char.IsAsciiLetter(source[prefix - 1]))
                    {
                        prefix--;
                    }

                    var end = StringEnd(i, source[prefix..i].Any(x => x is 'f' or 'F' or 't' or 'T'));
                    if (end < 0)
                    {
                        return end;
                    }

                    i = end;
                    continue;
                }
                case '(' or '[' or '{':
                    depth++;
                    break;
                case ')' or ']':
                    depth--;
                    break;
                case '}' when depth == 0:
                    return i + 1;
                case '}':
                    depth--;
                    break;
                case ':' when depth == 0:
                    spec = true;
                    break;
            }

            i++;
        }

        return -Math.Min(i, to);
    }

    private void Add(PythonTokenKind kind, int start, int end)
    {
        var (line, column) = lines.At(start);
        var (endLine, endColumn) = end > start ? lines.At(end - 1) : (line, column - 1);
        _tokens.Add(new PythonToken(kind, source[start..end], start, end, line, column, endLine, endColumn + 1));
    }

    private void Issue(int at, string message)
    {
        var (line, column) = lines.At(Math.Min(at, Math.Max(0, source.Length - 1)));
        Issues.Add(new PythonIssue(line, column, message));
    }

    private static bool IsNameStart(char c) => c == '_' || char.IsLetter(c);

    private static bool IsNamePart(char c) =>
        c == '_' || char.IsLetterOrDigit(c) || char.GetUnicodeCategory(c) is System.Globalization.UnicodeCategory.NonSpacingMark
            or System.Globalization.UnicodeCategory.SpacingCombiningMark or System.Globalization.UnicodeCategory.ConnectorPunctuation;
}

/// <summary>A file's line starts, for an offset's line and 1-based column. A line ends at <c>\r\n</c>, <c>\n</c> or <c>\r</c>.</summary>
internal sealed class PythonLines
{
    private readonly List<int> _starts = [0];

    public PythonLines(string source)
    {
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] == '\n' || source[i] == '\r' && (i + 1 >= source.Length || source[i + 1] != '\n'))
            {
                _starts.Add(i + 1);
            }
        }
    }

    public (int Line, int Column) At(int offset)
    {
        var index = _starts.BinarySearch(offset);
        if (index < 0)
        {
            index = ~index - 1;
        }

        return (index + 1, offset - _starts[index] + 1);
    }
}
