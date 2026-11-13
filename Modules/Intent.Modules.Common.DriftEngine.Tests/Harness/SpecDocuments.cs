using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>A check in check-classification.md, with the id the claim catalog gives it.</summary>
internal sealed record ClassifiedCheck(string Id, string Source, int Number, string Text, string Category);

/// <summary>Reads what the suite checks against from the spec's Markdown.</summary>
internal static class SpecDocuments
{
    /// <summary>The check tables' id prefixes, in the order the tables appear in check-classification.md.</summary>
    public static readonly IReadOnlyList<string> Sources = ["ESC-DOM", "ESC-SVC", "ESC-FE", "ESC-ARC", "JAV-DOM", "JAV-SVC", "JAV-FE", "JAV-ARC"];

    private static readonly Regex RequirementRow = new(@"^\|\s*(?<id>R\d+)\s*\|", RegexOptions.CultureInvariant);
    private static readonly Regex Backticked = new("`(?<text>[^`]+)`", RegexOptions.CultureInvariant);

    /// <summary>Every check row of the eight <c>| # | Check |</c> tables.</summary>
    public static IReadOnlyList<ClassifiedCheck> Checks(string path)
    {
        var checks = new List<ClassifiedCheck>();
        var table = -1;
        var inTable = false;
        foreach (var line in File.ReadLines(path))
        {
            if (line.StartsWith("| # | Check |", StringComparison.Ordinal))
            {
                table++;
                inTable = true;
                continue;
            }

            if (!inTable)
            {
                continue;
            }

            if (!line.StartsWith('|'))
            {
                inTable = false;
                continue;
            }

            var cells = Cells(line);
            if (cells.Count < 4 || !int.TryParse(cells[0], out var number))
            {
                continue;
            }

            if (table >= Sources.Count)
            {
                throw new InvalidOperationException($"check-classification.md has more check tables than the {Sources.Count} the catalog names.");
            }

            checks.Add(new ClassifiedCheck($"{Sources[table]}-{number:00}", Sources[table], number, cells[1], cells[3]));
        }

        return checks;
    }

    /// <summary>The Totals table's check count per source, in table order, then the overall total.</summary>
    public static (IReadOnlyList<int> PerSource, int Total) CheckTotals(string path)
    {
        var lines = File.ReadAllLines(path);
        var start = Array.FindIndex(lines, x => x.StartsWith("| Module | Checks |", StringComparison.Ordinal));
        if (start < 0)
        {
            throw new InvalidOperationException("check-classification.md has no '| Module | Checks |' totals table.");
        }

        var perSource = new List<int>();
        var total = 0;
        foreach (var line in lines.Skip(start + 2).TakeWhile(x => x.StartsWith('|')))
        {
            var cells = Cells(line);
            var count = int.Parse(cells[1].Trim('*'));
            if (cells[0].Contains("Total", StringComparison.Ordinal))
            {
                total = count;
            }
            else
            {
                perSource.Add(count);
            }
        }

        return (perSource, total);
    }

    /// <summary>Every requirement id in requirements.md's tables, in order, with repeats kept.</summary>
    public static IReadOnlyList<string> RequirementIds(string path)
    {
        return File.ReadLines(path).Select(x => RequirementRow.Match(x)).Where(x => x.Success).Select(x => x.Groups["id"].Value).ToList();
    }

    /// <summary>The Common.Drift fact kinds conversion.md lists under Kinds, which recipes use by name.</summary>
    public static IReadOnlyList<string> CommonDriftKinds(string path)
    {
        var kinds = new List<string>();
        var inKinds = false;
        foreach (var line in File.ReadLines(path))
        {
            if (line.StartsWith('#'))
            {
                inKinds = line.Trim() == "### Kinds";
                continue;
            }

            if (inKinds && line.StartsWith("- **", StringComparison.Ordinal))
            {
                // Only the list before "with placeholders": the kinds, not the placeholder names.
                var listed = line.Split(", with placeholders", 2)[0].Split(" The ", 2)[0];
                kinds.AddRange(Backticked.Matches(listed).Select(x => x.Groups["text"].Value));
            }
        }

        return kinds.Distinct(StringComparer.Ordinal).ToList();
    }

    /// <summary>claim-catalog.md's summary table: per source, the count of each disposition type.</summary>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> CatalogSummary(string path)
    {
        var lines = File.ReadAllLines(path);
        var start = Array.FindIndex(lines, x => x.StartsWith("| Source ", StringComparison.Ordinal) && x.Contains("| Claim ", StringComparison.Ordinal));
        if (start < 0)
        {
            throw new InvalidOperationException("claim-catalog.md has no '| Source | … | Claim |' summary table.");
        }

        var headers = Cells(lines[start]);
        var summary = new Dictionary<string, IReadOnlyDictionary<string, int>>(StringComparer.Ordinal);
        foreach (var line in lines.Skip(start + 2).TakeWhile(x => x.StartsWith('|')))
        {
            var cells = Cells(line);
            var row = new Dictionary<string, int>(StringComparer.Ordinal);
            for (var i = 1; i < cells.Count && i < headers.Count; i++)
            {
                row[headers[i]] = int.Parse(cells[i].Trim('*'));
            }

            summary[cells[0].Trim('*')] = row;
        }

        return summary;
    }

    /// <summary>A Markdown table row's cells, trimmed. A pipe inside backticks or escaped as <c>\|</c> stays in its cell.</summary>
    public static IReadOnlyList<string> Cells(string line)
    {
        var cells = new List<string>();
        var current = new System.Text.StringBuilder();
        var inCode = false;
        for (var i = 1; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '`')
            {
                inCode = !inCode;
            }

            if (c == '|' && !inCode && line[i - 1] != '\\')
            {
                cells.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }

            current.Append(c);
        }

        return cells;
    }
}
