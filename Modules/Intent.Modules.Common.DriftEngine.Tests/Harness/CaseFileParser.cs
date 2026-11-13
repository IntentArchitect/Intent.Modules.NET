using System.Text.Json;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// Reads case files (acceptance-suite.md, Case files): each <c>## &lt;case-id&gt;</c> starts a case, and each fenced
/// block within it is <c>&lt;language&gt; &lt;role&gt; [&lt;argument&gt;]</c>.
/// </summary>
internal static class CaseFileParser
{
    public const string Extension = ".cases.md";

    private static readonly Regex CaseId = new("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.CultureInvariant);
    private static readonly Regex Fence = new("^(?<fence>`{3,}|~{3,})(?<info>.*)$", RegexOptions.CultureInvariant);

    /// <summary>Every case file under <paramref name="casesFolder"/>, parsed.</summary>
    public static IReadOnlyList<CaseFile> ParseFolder(string casesFolder)
    {
        if (!Directory.Exists(casesFolder))
        {
            return [];
        }

        return Directory.GetFiles(casesFolder, "*" + Extension, SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(casesFolder, x).Replace('\\', '/'))
            .Order(StringComparer.Ordinal)
            .Select(x => Parse(x, File.ReadAllText(Path.Combine(casesFolder, x))))
            .ToList();
    }

    /// <param name="path">The file's path relative to the cases folder: <c>&lt;area&gt;/&lt;name&gt;.cases.md</c>.</param>
    public static CaseFile Parse(string path, string text)
    {
        var segments = path.Split('/');
        var file = new CaseFile
        {
            Path = path,
            Area = segments.Length == 2 ? segments[0] : "",
            Name = segments[^1].EndsWith(Extension, StringComparison.Ordinal) ? segments[^1][..^Extension.Length] : segments[^1]
        };

        if (segments.Length != 2)
        {
            file.Errors.Add($"{path}: a case file sits directly in an area folder, as cases/<area>/<name>{Extension}");
        }

        var lines = text.Replace("\r\n", "\n").Split('\n');
        string? id = null;
        var idLine = 0;
        var prose = new List<string>();
        var blocks = new List<CaseBlock>();

        void Close()
        {
            if (id is null)
            {
                return;
            }

            file.Cases.Add(new AcceptanceCase
            {
                SourceFile = path,
                Area = file.Area,
                Name = file.Name,
                Id = id,
                Line = idLine,
                Prose = string.Join('\n', prose).Trim(),
                OwnBlocks = blocks.ToList()
            });
        }

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var fence = Fence.Match(line);
            if (fence.Success)
            {
                var open = fence.Groups["fence"].Value;
                var start = i + 1;
                var end = start;
                while (end < lines.Length && !IsClosingFence(lines[end], open))
                {
                    end++;
                }

                if (end == lines.Length)
                {
                    file.Errors.Add($"{path}:{start}: the fence is never closed");
                    break;
                }

                var block = Block(path, start, fence.Groups["info"].Value, string.Join('\n', lines[start..end]), id is not null, file.Errors);
                if (block is not null)
                {
                    blocks.Add(block);
                }

                i = end;
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                Close();
                id = line[3..].Trim();
                idLine = i + 1;
                prose.Clear();
                blocks.Clear();
                if (!CaseId.IsMatch(id))
                {
                    file.Errors.Add($"{path}:{idLine}: case id '{id}' isn't kebab case");
                }
                else if (file.Cases.Any(x => x.Id == id))
                {
                    file.Errors.Add($"{path}:{idLine}: case id '{id}' is used twice");
                }

                continue;
            }

            if (id is not null)
            {
                prose.Add(line);
            }
        }

        Close();

        foreach (var @case in file.Cases)
        {
            foreach (var block in @case.OwnBlocks.Where(x => x.Role != CaseBlock.File))
            {
                try
                {
                    @case.Parsed[block] = JsonText.Parse(block.Content);
                }
                catch (JsonException e)
                {
                    file.Errors.Add($"{path}:{block.Line}: {@case.FullId}'s {block.Role} block isn't JSON: {e.Message}");
                }
            }
        }

        foreach (var @case in file.Cases)
        {
            @case.Blocks = Resolve(@case, file, [], file.Errors) ?? @case.OwnBlocks;
            foreach (var inherited in @case.Blocks.Except(@case.OwnBlocks))
            {
                var owner = file.Cases.First(x => x.OwnBlocks.Contains(inherited));
                if (owner.Parsed.TryGetValue(inherited, out var parsed))
                {
                    @case.Parsed[inherited] = parsed;
                }
            }

            foreach (var group in @case.OwnBlocks.GroupBy(x => x.Key).Where(x => x.Count() > 1))
            {
                file.Errors.Add($"{path}:{group.Last().Line}: {@case.FullId} has more than one '{group.Key}' block");
            }

            foreach (var role in new[] { CaseBlock.Case, CaseBlock.Model, CaseBlock.Expect })
            {
                if (@case.Blocks.All(x => x.Role != role))
                {
                    file.Errors.Add($"{path}:{@case.Line}: {@case.FullId} has no {role} block, of its own or from a base");
                }
            }

            if (@case.OwnBlocks.All(x => x.Role != CaseBlock.Case))
            {
                file.Errors.Add($"{path}:{@case.Line}: {@case.FullId} has no case block of its own");
            }
        }

        return file;
    }

    private static bool IsClosingFence(string line, string open)
    {
        var trimmed = line.TrimEnd();
        return trimmed.Length >= open.Length && trimmed.All(x => x == open[0]);
    }

    private static CaseBlock? Block(string path, int fenceLine, string info, string content, bool inCase, List<string> errors)
    {
        if (!inCase)
        {
            errors.Add($"{path}:{fenceLine}: a block before the first case heading belongs to no case");
            return null;
        }

        var tokens = info.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length < 2 || !CaseBlock.Roles.Contains(tokens[1]))
        {
            errors.Add($"{path}:{fenceLine}: the block's info string '{info.Trim()}' isn't '<language> <role> [<argument>]' with a role of {string.Join(", ", CaseBlock.Roles)}");
            return null;
        }

        var role = tokens[1];
        var argument = tokens.Length > 2 ? string.Join(' ', tokens[2..]) : null;
        if (role == CaseBlock.File && argument is null)
        {
            errors.Add($"{path}:{fenceLine}: a file block names its path, as '<language> file <path>'");
            return null;
        }

        if (role == CaseBlock.File && (argument!.StartsWith('/') || argument.Contains('\\') || argument.Split('/').Contains("..")))
        {
            errors.Add($"{path}:{fenceLine}: file path '{argument}' must be relative to the output folder, with forward slashes");
            return null;
        }

        if (role is CaseBlock.Case or CaseBlock.Model or CaseBlock.Expect && argument is not null)
        {
            errors.Add($"{path}:{fenceLine}: a {role} block takes no argument");
            return null;
        }

        // The block's first content line is the line after the fence; its number is 1-based.
        return new CaseBlock(role, tokens[0], argument, content, fenceLine + 1);
    }

    private static IReadOnlyList<CaseBlock>? Resolve(AcceptanceCase @case, CaseFile file, HashSet<string> visiting, List<string> errors)
    {
        var baseId = @case.BaseId;
        if (baseId is null)
        {
            return @case.OwnBlocks;
        }

        if (!visiting.Add(@case.Id))
        {
            errors.Add($"{file.Path}:{@case.Line}: {@case.FullId}'s base chain loops back to it");
            return null;
        }

        var @base = file.Cases.FirstOrDefault(x => x.Id == baseId);
        if (@base is null)
        {
            errors.Add($"{file.Path}:{@case.Line}: {@case.FullId}'s base '{baseId}' isn't a case in the same file");
            return null;
        }

        var baseBlocks = Resolve(@base, file, visiting, errors);
        if (baseBlocks is null)
        {
            return null;
        }

        var without = @case.Parsed.GetValueOrDefault(@case.OwnBlocks.First(x => x.Role == CaseBlock.Case)).Texts("without").ToHashSet(StringComparer.Ordinal);
        foreach (var path in without.Where(x => baseBlocks.All(b => b.Role != CaseBlock.File || b.Argument != x)))
        {
            errors.Add($"{file.Path}:{@case.Line}: {@case.FullId} drops '{path}', which its base has no file block for");
        }

        var ownKeys = @case.OwnBlocks.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
        return baseBlocks
            .Where(x => !ownKeys.Contains(x.Key))
            .Where(x => x.Role != CaseBlock.File || !without.Contains(x.Argument!))
            .Concat(@case.OwnBlocks)
            .ToList();
    }
}
