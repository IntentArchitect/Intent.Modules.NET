namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>
/// Text a recipe reads (rules.md, Recipes), with where in the file each of its characters came from, so a read case's match
/// is placed on its line: a node's canonical text, or a file's text as Common.Drift read any file, with whitespace runs
/// removed except one space between two words (<see cref="CanonicalText.OfText"/>).
/// </summary>
internal sealed class SourceText
{
    private readonly int[] _lines;
    private readonly int[] _columns;

    private SourceText(string file, string text, int[] lines, int[] columns)
    {
        File = file;
        Text = text;
        _lines = lines;
        _columns = columns;
    }

    public string File { get; }

    public string Text { get; }

    /// <summary>Where the character at <paramref name="index"/> is: its line, or the last one's for an index past the end.</summary>
    public Span At(int index) =>
        _lines.Length == 0 ? new Span(File, 1) : new Span(File, _lines[Math.Clamp(index, 0, _lines.Length - 1)], _columns[Math.Clamp(index, 0, _columns.Length - 1)]);

    /// <summary>The index of the first character at or after a place in the file; the text's length when none is.</summary>
    public int IndexAt(int line, int column)
    {
        for (var i = 0; i < _lines.Length; i++)
        {
            if (_lines[i] > line || _lines[i] == line && _columns[i] >= column)
            {
                return i;
            }
        }

        return _lines.Length;
    }

    public SourceText Slice(int start, int end)
    {
        start = Math.Clamp(start, 0, Text.Length);
        end = Math.Clamp(end, start, Text.Length);
        return new SourceText(File, Text[start..end], _lines[start..end], _columns[start..end]);
    }

    /// <summary>A file's source, or a part of it, read as text: <see cref="CanonicalText.OfText"/>, each character placed.</summary>
    public static SourceText OfText(string file, IReadOnlyList<SourceCharacter> source)
    {
        var text = new System.Text.StringBuilder();
        var lines = new List<int>();
        var columns = new List<int>();
        for (var i = 0; i < source.Count; i++)
        {
            if (!char.IsWhiteSpace(source[i].Value))
            {
                Append(source[i].Value, source[i]);
                continue;
            }

            var end = i;
            while (end + 1 < source.Count && char.IsWhiteSpace(source[end + 1].Value))
            {
                end++;
            }

            if (text.Length > 0 && end + 1 < source.Count && IsWordChar(text[^1]) && IsWordChar(source[end + 1].Value))
            {
                Append(' ', source[end + 1]);
            }

            i = end;
        }

        return new SourceText(file, text.ToString(), lines.ToArray(), columns.ToArray());

        void Append(char c, SourceCharacter at)
        {
            text.Append(c);
            lines.Add(at.Line);
            columns.Add(at.Column);
        }
    }

    /// <summary>
    /// A node's canonical text as its language gave it, each character placed by finding it in the node's source in order:
    /// canonical text is the source's tokens, so its characters other than whitespace are the source's, less comments. A
    /// character a comment also has may be placed in the comment, at most a line early.
    /// </summary>
    public static SourceText Aligned(string file, string text, IReadOnlyList<SourceCharacter> source)
    {
        var placed = source.Where(x => !char.IsWhiteSpace(x.Value)).ToList();
        var lines = new int[text.Length];
        var columns = new int[text.Length];
        var next = 0;
        var last = placed.Count > 0 ? placed[0] : new SourceCharacter(' ', 1, 1);
        for (var i = 0; i < text.Length; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                var found = next;
                while (found < placed.Count && placed[found].Value != text[i])
                {
                    found++;
                }

                if (found < placed.Count)
                {
                    last = placed[found];
                    next = found + 1;
                }
            }
            else if (next < placed.Count)
            {
                // A space between two words is where the second starts.
                last = placed[next];
            }

            lines[i] = last.Line;
            columns[i] = last.Column;
        }

        return new SourceText(file, text, lines, columns);
    }

    /// <summary>
    /// The characters of a file's source a span covers: from its line and column, to its end line and column when the span
    /// has them (an end column is one past its last character), else to the end of its end line.
    /// </summary>
    /// <param name="lines">Where each line starts in <paramref name="source"/> (<see cref="LineStarts"/>), so only the span's lines are walked.</param>
    public static IReadOnlyList<SourceCharacter> Characters(string source, IReadOnlyList<int> lines, Span? span = null)
    {
        var characters = new List<SourceCharacter>();
        var first = span is null ? 1 : Math.Clamp(span.Line, 1, lines.Count);
        var last = span is null ? lines.Count : Math.Clamp(span.EndLine ?? span.Line, first, lines.Count);
        for (var line = first; line <= last; line++)
        {
            var end = line < lines.Count ? lines[line] : source.Length;
            for (int i = lines[line - 1], column = 1; i < end; i++, column++)
            {
                if (span is null || Within(span, line, column))
                {
                    characters.Add(new SourceCharacter(source[i], line, column));
                }
            }
        }

        return characters;
    }

    /// <summary>Where each line of a source starts: line 1 at index 0.</summary>
    public static IReadOnlyList<int> LineStarts(string source)
    {
        var starts = new List<int> { 0 };
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] == '\n')
            {
                starts.Add(i + 1);
            }
        }

        return starts;
    }

    private static bool Within(Span span, int line, int column)
    {
        var endLine = span.EndLine ?? span.Line;
        if (line < span.Line || line > endLine)
        {
            return false;
        }

        if (line == span.Line && span.Column is { } start && column < start)
        {
            return false;
        }

        return line != endLine || span.EndColumn is not { } end || column < end;
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
}

/// <summary>A character of a file's source, and where it is.</summary>
internal readonly record struct SourceCharacter(char Value, int Line, int Column);
