using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>
/// Code links: each element linked to the code its verified claims read, for Intent Architect's View Code and Changes
/// Review, which counts a linked line as verified (user decision, 2026-11-17: only verified, modelled code). So a
/// declaration inside what a claim read is its own element's to link, and no element is linked to a line that holds drift
/// or code no claim located.
/// </summary>
public sealed partial class DriftEngine
{
    private List<CodeLink> Links(IReadOnlyList<Finding> findings)
    {
        // What each element's verified claims read, by file. Silence states nothing at a place, and a library type's use
        // isn't its code.
        var read = new Dictionary<string, Dictionary<string, List<Span>>>(StringComparer.Ordinal);
        var whole = new HashSet<(string File, string Element)>();
        foreach (var claim in _claims)
        {
            if (claim.Outcome != "verified" || claim.Reading?.From == "absent" || claim.Reading?.At is not { File.Length: > 0 } at || _referenced.Contains(claim))
            {
                continue;
            }

            // Intent Architect links an association, not its ends.
            var element = claim.Source is IAssociationEnd { Association: { } association } ? association.Id : claim.Element;
            // A file a claim located is the element's as a whole: a component's Owned File.
            if (_located.GetValueOrDefault(claim) is string)
            {
                whole.Add((at.File, element));
                continue;
            }

            // With the other code the reader read it through: the calls that pick out a relationship its foreign key configures.
            foreach (var span in claim.Reading.Through.Prepend(at).Where(x => x.File.Length > 0))
            {
                if (!read.TryGetValue(span.File, out var elements))
                {
                    elements = read[span.File] = new Dictionary<string, List<Span>>(StringComparer.Ordinal);
                }

                if (!elements.TryGetValue(element, out var spans))
                {
                    spans = elements[element] = [];
                }

                spans.Add(span);
            }
        }

        // No link may hold a line with a contradicted claim's reading, a finding's line, or code no claim located.
        var members = _graph.Types.SelectMany(x => x.Members).ToList();
        var declared = _graph.Types.Select(x => x.At).Concat(members.Select(x => x.At)).ToLookup(x => x.File, StringComparer.Ordinal);
        var located = LocatedNodes;
        var unverified = _graph.Types.Where(x => !located.Types.Contains(x)).Select(x => x.At)
            .Concat(members.Where(x => !located.Members.Contains(x)).Select(x => x.At))
            .Concat(_claims.Where(x => x.Outcome == "contradicted").Select(x => x.Reading?.At).OfType<Span>())
            .ToLookup(x => x.File, StringComparer.Ordinal);
        var findingLines = findings.Where(x => x.Severity != "info")
            .SelectMany(x => (x.Against ?? []).Prepend(x.Subject))
            .Where(x => x.File is not null && x.Line is not null)
            .ToLookup(x => x.File!, x => x.Line!.Value, StringComparer.Ordinal);

        var links = new List<CodeLink>();
        foreach (var file in read.Keys.Order(StringComparer.Ordinal))
        {
            var code = FileText.Read(Path.Combine(_input.OutputFolder, file));
            if (code is null)
            {
                continue;
            }

            var declarations = declared[file].Select(code.Range).OfType<(int Start, int End)>().Distinct().ToList();
            var cut = Unverified(code, declarations, unverified[file], findingLines[file]);
            foreach (var (element, spans) in read[file])
            {
                var ranges = Merge(spans.Select(code.Range).OfType<(int Start, int End)>().SelectMany(x => code.Own(x, declarations))
                    .SelectMany(x => Without(x, cut))
                    .Select(code.Trim)
                    .Where(x => x.Start < x.End));
                if (ranges.Count == 0)
                {
                    whole.Add((file, element));
                }

                links.AddRange(ranges.Select(x => new CodeLink(element, file, x.Start, x.End - x.Start)));
            }
        }

        // An element whose code in a file is all drift or undescribed code is still linked to the file, with no lines verified.
        links.AddRange(whole.Where(x => !links.Any(l => l.File == x.File && l.Element == x.Element))
            .OrderBy(x => x.File, StringComparer.Ordinal)
            .Select(x => new CodeLink(x.Element, x.File)));
        return links;
    }

    /// <summary>
    /// The whole lines of a file no link may hold: each span's code, without the declarations inside it, which are their
    /// own, and each finding's line.
    /// </summary>
    private static List<(int Start, int End)> Unverified(FileText code, IReadOnlyList<(int Start, int End)> declarations, IEnumerable<Span> spans, IEnumerable<int> lines) =>
        Merge(spans.Select(code.Range)
            .OfType<(int Start, int End)>()
            .SelectMany(x => code.Own(x, declarations))
            .Select(code.Trim)
            .Where(x => x.Start < x.End)
            .Select(code.Lines)
            .Concat(lines.Select(code.Line).OfType<(int Start, int End)>()));

    /// <summary>A range without the parts any of the others cover.</summary>
    private static IEnumerable<(int Start, int End)> Without((int Start, int End) range, IReadOnlyList<(int Start, int End)> others)
    {
        var pieces = new List<(int Start, int End)> { range };
        foreach (var other in others.Where(x => x.Start < range.End && x.End > range.Start))
        {
            pieces = pieces.SelectMany(x => other.End <= x.Start || other.Start >= x.End
                    ? new[] { x }
                    : new (int Start, int End)[] { (x.Start, other.Start), (other.End, x.End) }.Where(p => p.Start < p.End))
                .ToList();
        }

        return pieces;
    }

    /// <summary>The ranges in order, with those that overlap or touch joined.</summary>
    private static List<(int Start, int End)> Merge(IEnumerable<(int Start, int End)> ranges)
    {
        var merged = new List<(int Start, int End)>();
        foreach (var range in ranges.OrderBy(x => x.Start))
        {
            if (merged.Count > 0 && range.Start <= merged[^1].End)
            {
                merged[^1] = (merged[^1].Start, Math.Max(merged[^1].End, range.End));
            }
            else
            {
                merged.Add(range);
            }
        }

        return merged;
    }

    /// <summary>
    /// A file's text as Intent Architect reads it to link it (<c>File.ReadAllText</c>), and where its lines start, so a span's
    /// 1-based lines and columns become character offsets. A line ends at <c>\r\n</c>, <c>\n</c> or <c>\r</c>.
    /// </summary>
    private sealed class FileText
    {
        private readonly string _text;
        private readonly List<int> _starts = [0];

        private FileText(string text)
        {
            _text = text;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                {
                    i++;
                }

                if (text[i] is '\n' or '\r')
                {
                    _starts.Add(i + 1);
                }
            }
        }

        public static FileText? Read(string path)
        {
            try
            {
                return new FileText(File.ReadAllText(path));
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>The span's characters; a span with no columns is its lines.</summary>
        public (int Start, int End)? Range(Span span)
        {
            var endLine = span.EndLine ?? span.Line;
            if (span.Line < 1 || endLine < span.Line || endLine > _starts.Count)
            {
                return null;
            }

            var start = Offset(span.Line, span.Column ?? 1);
            var end = span.EndColumn is { } column ? Offset(endLine, column) : ContentEnd(endLine - 1);
            return end > start ? (start, end) : null;
        }

        /// <summary>A line's characters, its line break included.</summary>
        public (int Start, int End)? Line(int line) => line >= 1 && line <= _starts.Count ? (_starts[line - 1], Next(line - 1)) : null;

        /// <summary>The whole lines a range is on, line breaks included.</summary>
        public (int Start, int End) Lines((int Start, int End) range) => (_starts[IndexOf(range.Start)], Next(IndexOf(range.End - 1)));

        /// <summary>The range without the declarations strictly inside it, each of which is its own element's.</summary>
        public IEnumerable<(int Start, int End)> Own((int Start, int End) range, IReadOnlyList<(int Start, int End)> declarations) =>
            Without(range, declarations.Where(x => x.Start >= range.Start && x.End <= range.End && x != range).ToList());

        /// <summary>The range without the whitespace at its ends, so a link starts and ends on lines that hold its code.</summary>
        public (int Start, int End) Trim((int Start, int End) range)
        {
            var (start, end) = range;
            while (start < end && char.IsWhiteSpace(_text[start]))
            {
                start++;
            }

            while (end > start && char.IsWhiteSpace(_text[end - 1]))
            {
                end--;
            }

            return (start, end);
        }

        private int Offset(int line, int column) => Math.Min(_starts[line - 1] + Math.Max(column - 1, 0), ContentEnd(line - 1));

        /// <summary>Where the line's text ends, before its line break.</summary>
        private int ContentEnd(int index)
        {
            var end = Next(index);
            if (end > _starts[index] && _text[end - 1] == '\n')
            {
                end--;
            }

            if (end > _starts[index] && _text[end - 1] == '\r')
            {
                end--;
            }

            return end;
        }

        private int Next(int index) => index + 1 < _starts.Count ? _starts[index + 1] : _text.Length;

        private int IndexOf(int offset)
        {
            var index = _starts.BinarySearch(offset);
            return index >= 0 ? index : ~index - 1;
        }
    }
}
