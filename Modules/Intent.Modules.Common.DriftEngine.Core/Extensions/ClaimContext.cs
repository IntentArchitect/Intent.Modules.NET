using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>One claim, as a claim reader sees it: the model element and value it states, and the code graph.</summary>
public sealed class ClaimContext
{
    private readonly DriftEngine _engine;

    internal ClaimContext(DriftEngine engine, Claim claim, CodeGraph code, CodeScope scope)
    {
        _engine = engine;
        Claim = claim;
        Code = code;
        Scope = scope;
        Model = new ModelContext(engine);
    }

    public Claim Claim { get; }

    public string Kind => Claim.Kind;

    /// <summary>The value the model states, as canonical text.</summary>
    public string Expected => Claim.Expected;

    /// <summary>The value the model states as the vocabulary evaluated it: text, a bool, an element or a type reference.</summary>
    public object? ExpectedValue => Claim.ExpectedValue;

    /// <summary>The model element the claim is about; an association end for a navigation.</summary>
    public IElement Element => Claim.Source;

    public CodeGraph Code { get; }

    /// <summary>The model, read as a claim source reads it: elements by type, their links and stereotypes.</summary>
    public ModelContext Model { get; }

    /// <summary>Which files are read: code out of scope can't verify a claim, but a reader can say it is there.</summary>
    public CodeScope Scope { get; }

    /// <summary>The type the claim's dependency located, such as a class for its table name.</summary>
    public TypeNode? Type => _engine.LocatedType(Claim);

    /// <summary>The member the claim's dependency located, such as an attribute's property or an operation's method.</summary>
    public MemberNode? Member => _engine.LocatedMember(Claim);

    /// <summary>
    /// What the claims this one waits on located, nearest first, that is a <typeparamref name="T"/>: a type, a member, or
    /// whatever an extension's existence claim found (<see cref="ClaimReading.Found(object, Span)"/>).
    /// </summary>
    public T? Located<T>() where T : class => _engine.Located<T>(Claim);

    /// <summary>What an element is a member of: its parent, or for an association end, the element at the other end.</summary>
    public IElement? Owner(IElement element) => DriftEngine.Owner(element);

    /// <summary>The type an element's <c>type.exists</c> claim located, or null when none did.</summary>
    public TypeNode? TypeOf(IElement element) => _engine.TypeOf(Claim.Designer, element);

    /// <summary>The member an element's <c>member.exists</c> claim located, or null when none did.</summary>
    public MemberNode? MemberOf(IElement element) => _engine.MemberOf(Claim.Designer, element);

    /// <summary>What an element's existence claim of <paramref name="kind"/> located, in any designer; null when nothing did.</summary>
    public object? LocatedOf(IElement element, string kind, string? designer = null) => _engine.Locate(designer ?? Claim.Designer, element, kind);

    /// <summary>Another claim on an element, of this claim's designer or another.</summary>
    public Claim? ClaimOn(IElement element, string kind, string? designer = null) => _engine.ClaimOn(designer ?? Claim.Designer, element, kind);

    /// <summary>
    /// The code's type is the model's, as the engine compares types: through the language profile's type names and
    /// collection forms, without nullability.
    /// </summary>
    public bool TypeMatches(ITypeReference model, TypeRefNode code) => _engine.TypeMatches(model, code);
}

/// <summary>What a claim reader found in the code (rules.md: every rule returns the same reading).</summary>
public sealed class ClaimReading
{
    private ClaimReading(
        string from,
        string? value,
        Span? at,
        string? reason = null,
        string? detail = null,
        IReadOnlyList<Span>? candidates = null,
        string? searched = null,
        object? located = null,
        string? message = null)
    {
        From = from;
        Value = value;
        At = at;
        Reason = reason;
        Detail = detail;
        Candidates = candidates;
        Searched = searched;
        Located = located;
        Message = message;
    }

    internal string From { get; }

    internal string? Value { get; }

    internal Span? At { get; }

    internal string? Reason { get; }

    internal string? Detail { get; }

    internal IReadOnlyList<Span>? Candidates { get; }

    internal string? Searched { get; }

    internal object? Located { get; }

    internal string? Message { get; }

    /// <summary>
    /// For <c>type.exists</c>: the type that realises the element, found where the engine's own rule wouldn't look. The
    /// claims that depend on this one read it.
    /// </summary>
    public static ClaimReading Found(TypeNode type) => new("found", "present", type.At, located: type);

    /// <summary>
    /// For <c>member.exists</c>: the member that realises the element, found where the engine's own rule wouldn't look,
    /// such as a private field a house stereotype names. It may be one the reader made for what the syntax doesn't declare,
    /// such as an EF Core shadow property. The claims that depend on this one read it.
    /// </summary>
    public static ClaimReading Found(MemberNode member) => new("found", "present", member.At, located: member);

    /// <summary>
    /// For an existence kind: what realises the element, in whatever form the extension's own reader keeps it, such as its
    /// own model of a method. The claims that wait on this one have it (<see cref="ClaimContext.Located{T}"/>).
    /// </summary>
    public static ClaimReading Found(object located, Span at) => new("found", "present", at, located: located);

    /// <summary>The code states the value at <paramref name="at"/>.</summary>
    public static ClaimReading Stated(string value, Span at) => new("code", value, at);

    /// <summary>
    /// The code states <paramref name="actual"/>, which the reader compared with the model itself (<paramref name="same"/>),
    /// as for a type (<see cref="ClaimContext.TypeMatches"/>), or by any rule of its own. When they differ, the finding says
    /// <paramref name="message"/> if given, so the reader words what is wrong and the fix; otherwise the engine words it.
    /// </summary>
    public static ClaimReading Compared(string actual, Span at, bool same, string? message = null) =>
        new(same ? "compared:same" : "compared:different", actual, at, message: message);

    /// <summary>
    /// The code doesn't state it; <paramref name="at"/> is where it would. With a value, the silence is that value (no
    /// <c>[AllowAnonymous]</c> is <c>false</c>); without one, the claim is contradicted.
    /// </summary>
    public static ClaimReading Absent(Span at, string? value = null) => new("absent", value, at);

    /// <summary>
    /// For an existence kind: what the claim says exists isn't in the code. The finding says where was searched
    /// (<paramref name="where"/>, such as <c>Ordering.API/</c>) for what (<paramref name="searched"/>, such as <c>C# files</c>).
    /// </summary>
    public static ClaimReading Missing(string where, string searched) => new("missing", null, null, detail: where, searched: searched);

    /// <summary>The code doesn't state it, so the framework's default applies.</summary>
    public static ClaimReading Defaulted(string value, Span at) => new("default", value, at);

    /// <summary>The code doesn't state it, so the default applies; <paramref name="unstated"/> says what it lacks, for the finding.</summary>
    public static ClaimReading Defaulted(string value, Span at, string? unstated) => new("default", value, at, detail: unstated);

    /// <summary>The value can't be read: <paramref name="reason"/> as claims-and-findings.md names it (<c>unresolved</c>, <c>ambiguous</c>, ...).</summary>
    public static ClaimReading CantVerify(string reason, string detail, Span? at = null, IReadOnlyList<Span>? candidates = null) =>
        new("cantVerify", null, at, reason, detail, candidates);
}
