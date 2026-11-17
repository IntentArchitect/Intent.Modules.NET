using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>A place in a file. Lines and columns count from 1.</summary>
public sealed record Span(string File, int Line, int? Column = null, int? EndLine = null, int? EndColumn = null)
{
    /// <summary>The other span lies within this one: a call within the value it is part of. A missing column is the line's edge.</summary>
    public bool Contains(Span inner) =>
        inner.File == File
        && (inner.Line, inner.Column ?? 0).CompareTo((Line, Column ?? 0)) >= 0
        && (inner.EndLine ?? inner.Line, inner.EndColumn ?? 0).CompareTo((EndLine ?? Line, EndColumn ?? int.MaxValue)) <= 0;

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

    /// <summary>The other code the value was read through (<c>ClaimReading.Through</c>), linked to the element with <see cref="At"/>; not part of the result's schema.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyList<Span> Through { get; internal set; } = [];

    /// <summary>The finding's text as the claim reader worded it; null when the engine words it. The finding carries it, not the result.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Message { get; init; }
}

/// <summary>The claim a blocked claim waits on, by its identity.</summary>
/// <param name="Discriminator">Which of several claims of the kind on the element: an index's name, say.</param>
public sealed record ClaimRef(string Designer, string Element, string Kind, string? Discriminator = null)
{
    // Extensions built against the three-part reference keep loading.
    public ClaimRef(string designer, string element, string kind) : this(designer, element, kind, null)
    {
    }
}

/// <summary>One checkable statement the model makes about the code (claims-and-findings.md, The claim).</summary>
public sealed class Claim
{
    public required string Kind { get; init; }

    public required string Designer { get; init; }

    public required string Element { get; init; }

    /// <summary>Which of several claims of its kind on the element this is, such as an index by its name; null when it's the one.</summary>
    public string? Discriminator { get; init; }

    public required IReadOnlyList<string> Path { get; init; }

    [System.Text.Json.Serialization.JsonIgnore]
    public required string Expected { get; set; }

    /// <summary>The value the model states; none when it states none, such as a class with no Generalization.</summary>
    [System.Text.Json.Serialization.JsonPropertyName("expected")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Never)]
    public string? ExpectedValueText => Expected.Length == 0 ? null : Expected;

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

    /// <summary>The claims it also waits on, once <see cref="Needs"/> is verified: a pack's <c>waitsOn</c>, or a claim source's.</summary>
    internal IReadOnlyList<ClaimRef> WaitsOn { get; init; } = [];

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
public sealed record RunResult(IReadOnlyList<Claim> Claims, IReadOnlyList<Finding> Findings, Coverage Coverage, IReadOnlyList<PackRef> Packs, IReadOnlyList<FileRead> Files)
{
    /// <summary>How long each part of the run took, for the log; not part of the result's schema.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyList<Timing> Timings { get; init; } = [];

    /// <summary>The code each element is linked to, for Intent Architect's View Code and Changes Review; not part of the result's schema.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyList<CodeLink> Links { get; init; } = [];
}

/// <summary>
/// Code that realises a model element and agrees with it: a range of a file, or the whole file when <see cref="Start"/> is
/// null, for an element whose code in the file holds drift or code the model doesn't describe.
/// </summary>
/// <param name="Element">The element's id; an association end's is its association's, as Intent Architect links associations.</param>
/// <param name="File">The file, relative to the output folder.</param>
/// <param name="Start">The zero-based character offset of the code in the file's text, as <c>File.ReadAllText</c> reads it.</param>
/// <param name="Length">How many characters the code spans.</param>
public sealed record CodeLink(string Element, string File, int? Start = null, int? Length = null);

/// <summary>How long a part of the run took: reading code (<c>code</c>, each language and reader), building and reading claims
/// (<c>kind X</c> is one kind's own reading, with its claim count), rules, opt-in and the total.</summary>
public sealed record Timing(string What, double Milliseconds, int? Count = null);

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
    IReadOnlyList<IDriftExtension>? Extensions = null)
{
    /// <summary>
    /// Each designer's packages from other applications that it references, by designer name: read for what they say, as
    /// a Service Proxy's target operations, and never claimed on, since their code is another application's.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<IPackage>> References { get; init; } = new Dictionary<string, IReadOnlyList<IPackage>>();
}
