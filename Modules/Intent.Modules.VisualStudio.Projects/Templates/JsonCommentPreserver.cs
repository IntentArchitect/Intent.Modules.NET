using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Intent.Modules.VisualStudio.Projects.Templates;

/// <summary>
/// Extracts JSON comments before parsing and re-injects them after serialization,
/// since Newtonsoft.Json does not preserve comments through a serialize round-trip.
/// Comments are anchored to the property key that immediately follows them.
/// Inline end-of-line comments are anchored to the property key on the same line.
/// </summary>
internal static class JsonCommentPreserver
{
    internal sealed record CommentBlock(
        IReadOnlyList<string> Lines,
        string AnchorKey,
        bool IsInline);

    public static (string CleanJson, IReadOnlyList<CommentBlock> Blocks) ExtractAndStrip(string json)
    {
        var rawLines = json.ReplaceLineEndings("\n").Split('\n');
        var cleanLines = new List<string>(rawLines.Length);
        var blocks = new List<CommentBlock>();
        var pending = new List<string>();

        foreach (var rawLine in rawLines)
        {
            var trimmed = rawLine.Trim();

            if (IsPureCommentLine(trimmed))
            {
                pending.Add(rawLine);
                continue;
            }

            var key = TryExtractPropertyKey(trimmed);

            if (key != null && TryStripInlineComment(rawLine, out var cleanedLine, out var inlineComment))
            {
                if (pending.Count > 0)
                {
                    blocks.Add(new CommentBlock(pending.ToArray(), key, IsInline: false));
                    pending.Clear();
                }
                blocks.Add(new CommentBlock(new[] { inlineComment }, key, IsInline: true));
                cleanLines.Add(cleanedLine);
            }
            else if (key != null && pending.Count > 0)
            {
                blocks.Add(new CommentBlock(pending.ToArray(), key, IsInline: false));
                pending.Clear();
                cleanLines.Add(rawLine);
            }
            else if (key == null && TryStripInlineComment(rawLine, out var cleanedLine2, out var inlineComment2))
            {
                // Inline comment on a non-property line (e.g. an array element)
                var valueAnchor = cleanedLine2.Trim().TrimEnd(',');
                pending.Clear();
                blocks.Add(new CommentBlock(new[] { inlineComment2 }, valueAnchor, IsInline: true));
                cleanLines.Add(cleanedLine2);
            }
            else
            {
                // Discard orphaned pending comments (e.g. before a closing brace)
                pending.Clear();
                cleanLines.Add(rawLine);
            }
        }

        return (string.Join("\n", cleanLines), blocks);
    }

    public static string Restore(string json, IReadOnlyList<CommentBlock> blocks)
    {
        if (blocks.Count == 0)
            return json;

        var lines = json.ReplaceLineEndings("\n").Split('\n');
        var result = new List<string>(lines.Length + blocks.Count);

        var queues = new Dictionary<string, Queue<CommentBlock>>(StringComparer.Ordinal);
        foreach (var block in blocks)
        {
            if (!queues.TryGetValue(block.AnchorKey, out var q))
                queues[block.AnchorKey] = q = new Queue<CommentBlock>();
            q.Enqueue(block);
        }

        foreach (var line in lines)
        {
            var key = TryExtractPropertyKey(line.Trim());

            if (key != null && queues.TryGetValue(key, out var queue))
            {
                while (queue.Count > 0 && !queue.Peek().IsInline)
                    foreach (var commentLine in queue.Dequeue().Lines)
                        result.Add(commentLine);

                if (queue.Count > 0 && queue.Peek().IsInline)
                    result.Add(line.TrimEnd() + " " + queue.Dequeue().Lines[0]);
                else
                    result.Add(line);

                if (queue.Count == 0)
                    queues.Remove(key);
            }
            else
            {
                // Check for non-property inline comment (e.g. an array element)
                var valueAnchor = line.Trim().TrimEnd(',');
                if (queues.TryGetValue(valueAnchor, out var valueQueue) && valueQueue.Peek().IsInline)
                {
                    result.Add(line.TrimEnd() + " " + valueQueue.Dequeue().Lines[0]);
                    if (valueQueue.Count == 0)
                        queues.Remove(valueAnchor);
                }
                else
                {
                    result.Add(line);
                }
            }
        }

        return string.Join("\n", result);
    }

    private static bool IsPureCommentLine(string trimmed)
        => trimmed.StartsWith("//")
        || trimmed.StartsWith("/*")
        || trimmed.StartsWith("*/")
        || (trimmed.StartsWith("*") && (trimmed.Length == 1 || trimmed[1] != '"'));

    private static string? TryExtractPropertyKey(string trimmed)
    {
        var m = Regex.Match(trimmed, @"^""((?:[^""\\]|\\.)*)""\s*:");
        return m.Success ? m.Groups[1].Value : null;
    }

    private static bool TryStripInlineComment(string line, out string cleanedLine, out string comment)
    {
        var inString = false;
        for (var i = 0; i < line.Length - 1; i++)
        {
            var c = line[i];
            if (inString)
            {
                if (c == '\\') { i++; continue; }
                if (c == '"') inString = false;
                continue;
            }
            if (c == '"') { inString = true; continue; }

            if (c == '/' && line[i + 1] == '/')
            {
                cleanedLine = line[..i].TrimEnd();
                comment = line[i..].Trim();
                return true;
            }

            if (c == '/' && line[i + 1] == '*')
            {
                var end = line.IndexOf("*/", i + 2, StringComparison.Ordinal);
                if (end >= 0)
                {
                    cleanedLine = (line[..i] + line[(end + 2)..]).TrimEnd();
                    comment = line[i..(end + 2)].Trim();
                    return true;
                }
            }
        }

        cleanedLine = line;
        comment = string.Empty;
        return false;
    }
}
