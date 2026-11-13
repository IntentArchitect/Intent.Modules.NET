using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>A place in a file. Lines and columns count from 1.</summary>
public sealed record Span(string File, int Line, int? Column = null, int? EndLine = null, int? EndColumn = null)
{
    public override string ToString() => $"{File}:{Line}";
}

/// <summary>A rule's result for one claim: the value read and where it came from, or why there is none.</summary>
public sealed class Reading
{
    public string? Value { get; init; }

    /// <summary><c>code</c>, or <c>absent</c> when the code says nothing (silence).</summary>
    public string? From { get; init; }

    public Span? At { get; init; }

    public string? Text { get; init; }

    public string? Detail { get; init; }

    public IReadOnlyList<string>? Searched { get; init; }

    public string? ShadowFile { get; init; }

    public string? ScopeRow { get; init; }

    public IReadOnlyList<Span>? Candidates { get; init; }

    /// <summary>The finding's text as the claim reader worded it; null when the engine words it.</summary>
    public string? Message { get; init; }
}

/// <summary>The claim a blocked claim waits on, by its identity.</summary>
public sealed record ClaimRef(string Designer, string Element, string Kind);

/// <summary>One checkable statement the model makes about the code (claims-and-findings.md, The claim).</summary>
public sealed class Claim
{
    public required string Kind { get; init; }

    public required string Designer { get; init; }

    public required string Element { get; init; }

    public required IReadOnlyList<string> Path { get; init; }

    public required string Expected { get; set; }

    public required string Stated { get; init; }

    public IReadOnlyDictionary<string, string>? Bindings { get; init; }

    public string? Origin { get; init; }

    public string Outcome { get; set; } = "cantVerify";

    public string? Reason { get; set; }

    public ClaimRef? DependsOn { get; set; }

    public Reading? Reading { get; set; }

    internal IElement Source { get; init; } = null!;

    internal KindDefinition Definition { get; init; } = null!;

    /// <summary>The claim it waits on, as stated: a pack's <c>dependsOn</c> resolved to its element, or a claim source's.</summary>
    internal ClaimRef? Needs { get; init; }

    internal object? ExpectedValue { get; init; }

    internal string SourceText { get; init; } = "";

    /// <summary>The report Area its finding goes in: the vocabulary's <c>area</c>, or the designer.</summary>
    internal string Area { get; init; } = "";
}

/// <summary>What a finding points at: a model element, a file and line, or plain text.</summary>
public sealed record Anchor(string? Element = null, string? File = null, int? Line = null, string? Text = null);

/// <summary>A row in the drift report (claims-and-findings.md, The finding).</summary>
/// <param name="Category"><c>ModelCode</c>, <c>CodeCode</c> or <c>Code</c>.</param>
/// <param name="Severity"><c>error</c>, <c>warning</c>, or <c>info</c> for the Software Factory log only.</param>
/// <param name="Subject">What the finding is about: the element, or for code against code, the file.</param>
/// <param name="Against">What else it involves, such as the Layers an import crosses.</param>
public sealed record Finding(
    string Rule,
    string Area,
    string Category,
    string Severity,
    string Message,
    Anchor Subject,
    IReadOnlyList<string>? Claims = null,
    IReadOnlyList<Anchor>? Against = null);

/// <summary>Per scope row, how many files it decides (R15).</summary>
public sealed record CoverageRow(string Host, int Row, IReadOnlyList<string>? Glob, IReadOnlyList<string>? Types, string Scope, string? Reason, int Files);

public sealed record Coverage(IReadOnlyList<CoverageRow> Rows, IReadOnlyDictionary<string, int> Builtin, IReadOnlyDictionary<string, int> Unmatched);

public sealed record PackRef(string Id, string Version);

public sealed record FileRead(string Path, string Reader);

/// <summary>What one run produces (run-result.schema.json).</summary>
public sealed record RunResult(IReadOnlyList<Claim> Claims, IReadOnlyList<Finding> Findings, Coverage Coverage, IReadOnlyList<PackRef> Packs, IReadOnlyList<FileRead> Files);

/// <summary>
/// One run's input: the application's designers as Intent model objects (decision 2), the folder holding its code, and
/// the packs and extensions it uses.
/// </summary>
/// <param name="Designers">Each designer's packages, by designer name.</param>
/// <param name="OutputFolder">The application's output folder; file paths in the result are relative to it.</param>
/// <param name="Packs">The packs the model lists. The core pack is always used.</param>
/// <param name="Extensions">The extensions installed modules add, in order: a later one's claim reader comes first.</param>
public sealed record DriftInput(
    IReadOnlyDictionary<string, IReadOnlyList<IPackage>> Designers,
    string OutputFolder,
    IReadOnlyList<Pack> Packs,
    IReadOnlyList<IDriftExtension>? Extensions = null);
