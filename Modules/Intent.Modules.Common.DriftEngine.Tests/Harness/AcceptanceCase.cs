using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>One fenced block of a case: <c>```&lt;language&gt; &lt;role&gt; [&lt;argument&gt;]</c>.</summary>
internal sealed record CaseBlock(string Role, string Language, string? Argument, string Content, int Line)
{
    public const string Case = "case";
    public const string Model = "model";
    public const string File = "file";
    public const string Pack = "pack";
    public const string Expect = "expect";

    public static readonly IReadOnlySet<string> Roles = new HashSet<string>(StringComparer.Ordinal) { Case, Model, File, Pack, Expect };

    /// <summary>What a case's own block replaces in its base: the block of the same role and path.</summary>
    public string Key => Argument is null ? Role : $"{Role} {Argument}";

    /// <summary>The block's lines, numbered from 1 as an <c>at</c> counts them.</summary>
    public IReadOnlyList<string> Lines => Content.Length == 0 ? [] : Content.Split('\n');
}

/// <summary>One <c>## &lt;case-id&gt;</c> section of a case file, with its base applied.</summary>
internal sealed class AcceptanceCase
{
    public required string SourceFile { get; init; }

    public required string Area { get; init; }

    public required string Name { get; init; }

    public required string Id { get; init; }

    public required int Line { get; init; }

    public required string Prose { get; init; }

    /// <summary>The blocks written under this case's heading.</summary>
    public required IReadOnlyList<CaseBlock> OwnBlocks { get; init; }

    /// <summary>The blocks the case runs with: its base's, with its own replacing those of the same role and path.</summary>
    public IReadOnlyList<CaseBlock> Blocks { get; set; } = [];

    /// <summary>The parsed JSON of each JSON block, by block; a block that doesn't parse has none.</summary>
    public Dictionary<CaseBlock, JsonNode> Parsed { get; } = [];

    public string FullId => $"{Area}/{Name}#{Id}";

    public JsonNode? Meta => Json(CaseBlock.Case);

    public JsonNode? Model => Json(CaseBlock.Model);

    public JsonNode? Expect => Json(CaseBlock.Expect);

    public IEnumerable<JsonNode> Packs => Blocks.Where(x => x.Role == CaseBlock.Pack).Select(x => Parsed.GetValueOrDefault(x)).OfType<JsonNode>();

    /// <summary>The case's code files, by path relative to the output folder.</summary>
    public IReadOnlyDictionary<string, CaseBlock> Files => Blocks
        .Where(x => x.Role == CaseBlock.File)
        .GroupBy(x => x.Argument!, StringComparer.Ordinal)
        .ToDictionary(x => x.Key, x => x.Last(), StringComparer.Ordinal);

    public string? BaseId => OwnBlocks.FirstOrDefault(x => x.Role == CaseBlock.Case) is { } own && Parsed.TryGetValue(own, out var meta)
        ? meta.Text("base")
        : null;

    public override string ToString() => FullId;

    private JsonNode? Json(string role)
    {
        return Blocks.FirstOrDefault(x => x.Role == role) is { } block ? Parsed.GetValueOrDefault(block) : null;
    }
}

/// <summary>A case file: <c>cases/&lt;area&gt;/&lt;name&gt;.cases.md</c>.</summary>
internal sealed class CaseFile
{
    public required string Path { get; init; }

    public required string Area { get; init; }

    public required string Name { get; init; }

    public List<AcceptanceCase> Cases { get; } = [];

    /// <summary>What is wrong with the file's shape: unclosed fences, blocks with no role, bad or missing bases.</summary>
    public List<string> Errors { get; } = [];
}
