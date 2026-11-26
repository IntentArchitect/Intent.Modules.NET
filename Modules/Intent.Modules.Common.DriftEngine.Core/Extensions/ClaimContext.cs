using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>One claim, as a claim reader sees it: the model element and value it states, and the code graph.</summary>
public sealed class ClaimContext
{
    private readonly DriftEngine _engine;
    private readonly Func<ClaimReading?>? _next;

    internal ClaimContext(DriftEngine engine, Claim claim, CodeGraph code, CodeScope scope, Func<ClaimReading?>? next = null)
    {
        _engine = engine;
        Claim = claim;
        Code = code;
        Scope = scope;
        Model = new ModelContext(engine);
        _next = next;
    }

    /// <summary>
    /// What the claim readers after this one read, as if this one had passed the claim on: a later extension's reader comes
    /// first, so this is an earlier extension's and then the stock stacks'. A reader that only adjusts a reading (narrows where
    /// a lookup looks, rewords a finding, turns a can't-verify into a miss for its own code) calls it and returns what it makes
    /// of the result, rather than redoing the stack's reading. Null when no later reader answers: if this reader returns null
    /// too, the engine's own rule for the kind reads the claim as usual. A later reader that fails is reported as failing,
    /// and its reading is can't verify.
    /// </summary>
    public ClaimReading? ReadNext() => _next?.Invoke();

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

    /// <summary>
    /// The model's name for the code's type, through its language profile's type names (<c>int</c>, <c>guid</c>), for a
    /// correction that adds a member of it; null for a type the profile doesn't map.
    /// </summary>
    public string? ModelTypeName(TypeRefNode code) => _engine.ModelTypeName(code);
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

    // Readable by extensions, so a reader that asks the readers after it (ClaimContext.ReadNext) can see what they read and
    // adjust it; made only through the factory methods below.

    /// <summary>
    /// What the reading is: <c>found</c>, <c>code</c> (stated), <c>compared:same</c> or <c>compared:different</c>,
    /// <c>absent</c>, <c>missing</c>, <c>default</c>, <c>outOfScope</c> or <c>cantVerify</c>, as the factory method that made it.
    /// </summary>
    public string From { get; }

    /// <summary>The value the code states, compared or defaults to; null for none.</summary>
    public string? Value { get; }

    /// <summary>Where the code states it, or would.</summary>
    public Span? At { get; }

    /// <summary>Why it can't be verified (<c>unresolved</c>, <c>ambiguous</c>, <c>outOfScope</c>, ...); null for a reading that verifies or contradicts.</summary>
    public string? Reason { get; }

    /// <summary>What the finding says beyond the value: what can't be read, where a missing thing was searched, the out-of-scope file.</summary>
    public string? Detail { get; }

    /// <summary>The places an ambiguous reading could be.</summary>
    public IReadOnlyList<Span>? Candidates { get; }

    /// <summary>For a missing reading, what was searched for.</summary>
    public string? Searched { get; }

    /// <summary>What an existence reading located: a type, a member, or an extension's own model of it.</summary>
    public object? Located { get; }

    /// <summary>The reader's own finding text for a contradiction; null for the engine's wording.</summary>
    public string? Message { get; }

    /// <summary>The other code the reading was read through (<see cref="Through"/>).</summary>
    public IReadOnlyList<Span> Code { get; private init; } = [];

    /// <summary>The model changes that settle a contradiction (<see cref="Correcting"/>).</summary>
    public IReadOnlyList<ModelCorrection> Corrections { get; private init; } = [];

    /// <summary>
    /// The reading with the other code it was read through, such as the <c>HasOne</c> and <c>WithMany</c> that pick out the
    /// relationship a <c>HasForeignKey</c> configures. A finding still points at the reading's own place; a verified claim's
    /// element is linked to both, for View Code and Changes Review.
    /// </summary>
    public ClaimReading Through(IEnumerable<Span> code) =>
        new(From, Value, At, Reason, Detail, Candidates, Searched, Located, Message) { Code = [.. Code, .. code], Corrections = Corrections };

    /// <summary>
    /// The reading with the model changes that make the model state what the code does (<see cref="ModelCorrection"/>), for a
    /// contradiction the code settles: the claim's finding carries them. Give them only when the reading is certain, never
    /// for a value the reader had to guess.
    /// </summary>
    public ClaimReading Correcting(params ModelCorrection[] corrections) =>
        new(From, Value, At, Reason, Detail, Candidates, Searched, Located, Message) { Code = Code, Corrections = [.. Corrections, .. corrections] };

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
    /// The code doesn't state it; <paramref name="at"/> is where it would, or null when no code is that place (nothing derives
    /// the handler that would say so). With a value, the silence is that value (no <c>[AllowAnonymous]</c> is <c>false</c>);
    /// without one, the claim is contradicted.
    /// </summary>
    public static ClaimReading Absent(Span? at, string? value = null) => new("absent", value, at);

    /// <summary>
    /// For an existence kind: what the claim says exists isn't in the code. The finding says where was searched
    /// (<paramref name="where"/>, such as <c>Ordering.API/</c>) for what (<paramref name="searched"/>, such as <c>C# files</c>).
    /// </summary>
    public static ClaimReading Missing(string where, string searched) => new("missing", null, null, detail: where, searched: searched);

    /// <summary>The code doesn't state it, so the framework's default applies.</summary>
    public static ClaimReading Defaulted(string value, Span at) => new("default", value, at);

    /// <summary>The code doesn't state it, so the default applies; <paramref name="unstated"/> says what it lacks, for the finding.</summary>
    public static ClaimReading Defaulted(string value, Span at, string? unstated) => new("default", value, at, detail: unstated);

    /// <summary>
    /// The code that would state it is in <paramref name="file"/>, which the Drift Scope leaves out (<see cref="CodeScope.Includes"/>):
    /// can't verify, <c>outOfScope</c>, naming the scope row that excludes the file, so the finding says which row to change.
    /// </summary>
    public static ClaimReading OutOfScope(string file) => new("outOfScope", null, null, "outOfScope", detail: file);

    /// <summary>The value can't be read: <paramref name="reason"/> as claims-and-findings.md names it (<c>unresolved</c>, <c>ambiguous</c>, ...).</summary>
    public static ClaimReading CantVerify(string reason, string detail, Span? at = null, IReadOnlyList<Span>? candidates = null) =>
        new("cantVerify", null, at, reason, detail, candidates);
}
