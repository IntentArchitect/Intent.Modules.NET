using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>What a rule sees: the designers' elements, the claims and their outcomes, the in-scope code and the scope.</summary>
public sealed class RuleContext
{
    private readonly DriftEngine _engine;

    internal RuleContext(DriftEngine engine, CodeGraph code, CodeScope scope, IReadOnlyList<Claim> claims)
    {
        _engine = engine;
        Code = code;
        Scope = scope;
        Claims = claims;
    }

    public CodeGraph Code { get; }

    public CodeScope Scope { get; }

    /// <summary>The model, as a claim source reads it: designers, links, stereotypes, and packages other applications own.</summary>
    public ModelContext Model => new(_engine);

    /// <summary>Every claim of the run, evaluated.</summary>
    public IReadOnlyList<Claim> Claims { get; }

    /// <summary>
    /// The model's type and a type in code are the same, under the profile of the code's language, as the engine compares
    /// <c>member.type</c>.
    /// </summary>
    public bool TypeMatches(ITypeReference model, TypeRefNode code) => _engine.TypeMatches(model, code);

    /// <summary>A designer's elements at any depth, by the name packs give it; none when the application hasn't the designer.</summary>
    public IReadOnlyList<IElement> Elements(string designer) => _engine.Elements(designer);

    /// <summary>A claim of a designer on an element.</summary>
    public Claim? ClaimOn(string designer, IElement element, string kind) => _engine.ClaimOn(designer, element, kind);

    /// <summary>The type an element's <c>type.exists</c> claim located, or null when none did.</summary>
    public TypeNode? TypeOf(string designer, IElement element) => _engine.TypeOf(designer, element);

    /// <summary>The member an element's <c>member.exists</c> claim located, or null when none did.</summary>
    public MemberNode? MemberOf(string designer, IElement element) => _engine.MemberOf(designer, element);

    /// <summary>What an element's existence claim of <paramref name="kind"/> located; null when nothing did.</summary>
    public object? LocatedOf(string designer, IElement element, string kind) => _engine.Locate(designer, element, kind);

    /// <summary>
    /// A claim located the type: it is an element's type, or some claim's reading came from code inside it (rules.md, Opt-in
    /// rules). Code no claim located is code the model doesn't describe.
    /// </summary>
    public bool IsLocated(TypeNode type) => _engine.LocatedNodes.Types.Contains(type);

    /// <summary>A claim located the member: it is an element's member, or some claim's reading came from code inside it.</summary>
    public bool IsLocated(MemberNode member) => _engine.LocatedNodes.Members.Contains(member);

    /// <summary>A claim located something in the file.</summary>
    public bool IsLocated(string file) => _engine.LocatedNodes.Files.Contains(file);

    /// <summary>Some claim's reading came from code at this place: a parameter's, a call's, any node's span.</summary>
    public bool IsLocated(Span span) => _engine.LocatedNodes.Readings[span.File].Any(x => DriftEngine.Inside(span, x));

    /// <summary>The element whose <c>type.exists</c> claim located the type, with its designer; null when none did.</summary>
    public (string Designer, IElement Element)? ElementOf(TypeNode type) => _engine.ElementOf(type);

    internal DriftEngine Engine => _engine;
}

/// <summary>Which files the Drift Scope reads. Paths are relative to the output folder, with <c>/</c>.</summary>
public sealed class CodeScope
{
    private readonly IReadOnlyDictionary<string, (bool In, bool Checked)> _files;

    internal CodeScope(IEnumerable<(string Path, bool In, bool Checked)> files)
    {
        _files = files.ToDictionary(x => x.Path, x => (x.In, x.Checked), StringComparer.Ordinal);
        Files = _files.Keys.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    /// <summary>Every file under the output folder, in or out of scope, in path order.</summary>
    public IReadOnlyList<string> Files { get; }

    /// <summary>The file is in scope, so it is read: an In row or a Context row decides it.</summary>
    public bool Includes(string path) => _files.GetValueOrDefault(path).In;

    /// <summary>
    /// The file's code is held to the model: an In row decides it. A Context row's file is read, so rules and claims can use
    /// what it says, but a finding about its own code (an opt-in rule's, or a rule's whose subject is the file) isn't reported.
    /// </summary>
    public bool Checks(string path) => _files.GetValueOrDefault(path).Checked;
}
