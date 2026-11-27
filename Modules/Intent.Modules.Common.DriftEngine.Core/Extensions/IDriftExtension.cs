using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>
/// What a module adds to the engine, in code (plan.md, Objective). The engine knows no technology: every stack, and every
/// module's own designer or stereotype, is an extension, and a module's extension has the same reach as the stock ones.
/// Each part is optional.
/// </summary>
public interface IDriftExtension
{
    string Id { get; }

    /// <summary>
    /// The designers it reads, by the name its claims give them, each to its designer id: a module's own designer, which
    /// the engine doesn't know of.
    /// </summary>
    IReadOnlyDictionary<string, string> Designers => new Dictionary<string, string>();

    /// <summary>Its claim kinds, in code; a pack's kinds are the same thing as data.</summary>
    IEnumerable<KindDefinition> Kinds => [];

    /// <summary>
    /// What the model states that the code must match, worked out in code: the model side of a check, with nothing it
    /// can't express. A pack's vocabulary is the same thing as data.
    /// </summary>
    IEnumerable<IClaimSource> ClaimSources => [];

    /// <summary>Its claim kinds, vocabulary, conventions and language profiles, as data.</summary>
    IEnumerable<Pack> Packs => [];

    /// <summary>
    /// The languages it reads. The engine knows none itself: a language reads its files into the graph's types, members
    /// and files, and keeps its own syntax there for the extensions that read that language.
    /// </summary>
    IEnumerable<ILanguage> Languages => [];

    /// <summary>Readers run over the in-scope code once the language readers have built the graph.</summary>
    IEnumerable<ICodeReader> Readers => [];

    IEnumerable<IClaimReader> ClaimReaders => [];

    /// <summary>What its house's own code means to the claims others read (<see cref="IMemberRefiner"/>).</summary>
    IEnumerable<IMemberRefiner> Refiners => [];

    /// <summary>Comparers for its kinds, named by a kind's <c>comparer</c>.</summary>
    IEnumerable<IValueComparer> Comparers => [];

    /// <summary>Rules over the model and the code together, for what isn't one claim's value.</summary>
    IEnumerable<IDriftRule> Rules => [];

    /// <summary>"Must be modelled" rules a team turns on in the model (<see cref="IOptInRule"/>).</summary>
    IEnumerable<IOptInRule> OptInRules => [];

    /// <summary>
    /// What its claim sources, claim readers and rules read of the model in code, besides the elements they claim on: element
    /// types, stereotypes (one property, or all of them) and association types (vocabulary.md, Coverage of the model). A
    /// model feature nothing reads, and nothing declares non-checkable, is an <c>engine/noVocabulary</c> finding.
    /// </summary>
    IEnumerable<ModelFeature> Reads => [];

    /// <summary>The model features it declares non-checkable, each with its reason (R32), as a pack's <c>nonClaims</c> are.</summary>
    IEnumerable<ModelFeature> NonClaims => [];
}

/// <summary>
/// A model feature of one designer: an element type, a stereotype (all its properties when <see cref="Property"/> is
/// null), or an association type. Give one of them. <see cref="Reason"/> is what a non-claim says.
/// </summary>
public sealed record ModelFeature(string Designer, string? Element = null, string? Stereotype = null, string? Property = null, string? Association = null, string? Reason = null);

/// <summary>
/// A rule over the model and the code together (plan.md, block 4): a check that isn't one claim's value, such as every
/// import of a Layer against its May Depend On arrows. It runs once the claims are evaluated, over in-scope code only,
/// and never writes anything.
/// </summary>
public interface IDriftRule
{
    /// <summary>
    /// The rule's id: <c>messaging/unmodelled</c>. A module's rule of the same id replaces it, the last registered winning,
    /// so a house that disagrees with a stock rule copies it and changes it, or turns it off with one that reports nothing.
    /// </summary>
    string Id { get; }

    /// <returns>
    /// Its findings. Each names its Area, as the report shows it; the engine marks the Area as its own, so the findings
    /// sit apart from another checker's. A finding whose subject is a file a Context scope row decides isn't reported: that
    /// code is read for context, not checked.
    /// </returns>
    IEnumerable<Finding> Check(RuleContext context);
}

/// <summary>
/// A "must be modelled" rule (rules.md, Opt-in rules): it finds code of a kind the model can describe that no claim
/// located. Code the model doesn't mention is never drift on its own, so the rule runs only when the Root Folder's
/// <c>Drift Check</c> lists <c>optin/{Id}</c> under Opt-in Rules. The engine does the rest, the same for every such rule:
/// it keeps what the row's Glob covers, drops what an exception matches and what a Context row decides, flags exceptions
/// that match nothing, and reports each as an error unless the model overrides its severity.
/// </summary>
public interface IOptInRule
{
    /// <summary>The rule's id, without <c>optin/</c>: <c>unmodelled.type</c>.</summary>
    string Id { get; }

    /// <summary>
    /// What it finds, in checked code (<see cref="CodeScope.Checks"/>). <see cref="RuleContext.IsLocated(TypeNode)"/> and its
    /// overloads say what the claims located.
    /// </summary>
    IEnumerable<UnmodelledCode> Find(RuleContext context);
}

/// <summary>Code an opt-in rule finds the model doesn't describe.</summary>
/// <param name="Designer">The designer that would describe it: the report Area. Null for none, such as a file.</param>
/// <param name="At">Where its node starts: its first attribute or annotation if it has any; line 1 for a file.</param>
/// <param name="Name">What an exception's Match names it by: a type's name, <c>Type.Member</c>, or the rule's own name for it.</param>
/// <param name="Message">The finding's text: what isn't modelled, and what would model it.</param>
/// <param name="Owner">The nearest modelled element it belongs to, by id, if any.</param>
/// <param name="Type">The type it is in, so an exception naming the type covers its members.</param>
public sealed record UnmodelledCode(string? Designer, Span At, string Name, string Message, string? Owner = null, string? Type = null);

/// <summary>
/// A language's reader (code-reader.md). It parses each in-scope file of its language once, adds the files, types and
/// members it declares to the graph, and keeps its syntax on the graph (<see cref="CodeGraph.Set{T}"/>) so every extension
/// that reads the language shares the one parse. It never builds, loads or runs the code.
/// </summary>
public interface ILanguage
{
    /// <summary>The name files and profiles give it: <c>csharp</c>.</summary>
    string Name { get; }

    /// <summary>The name findings give it: <c>C#</c>.</summary>
    string Title { get; }

    /// <summary>It reads the file, by its path.</summary>
    bool Reads(string path);

    /// <summary>
    /// A file reaches a library's type only through an import of a file the engine doesn't read, so a type it uses that
    /// nothing in scope declares, with no such import, isn't anywhere: Protobuf, where even a well-known type is imported.
    /// False for C# and Java, which reach a library's types without naming a file.
    /// </summary>
    bool LibrariesNeedImports => false;

    /// <param name="root">The output folder.</param>
    /// <param name="files">Its in-scope files, relative to <paramref name="root"/>, with <c>/</c> separators, in order.</param>
    /// <param name="graph">The graph to add to; a file it can't read goes in <see cref="CodeGraph.Failures"/>.</param>
    /// <param name="cancellationToken">
    /// The run's: check it file by file (<see cref="CancellationToken.ThrowIfCancellationRequested"/>), so a cancelled run
    /// stops reading. A cancelled read is never kept for the next run.
    /// </param>
    void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken);
}

/// <summary>
/// A framework's language for files of an extension several frameworks share, such as <c>.html</c>: a file is its own only
/// where the code already read says so, file by file, never by its extension alone (code-reader.md, Claimed files). A file
/// its own extension names (<see cref="ILanguage.Reads"/>, such as <c>.jinja</c>) is its own as any language's is.
/// <para>
/// The engine asks it once every language that reads by extension has read, so the code it needs is on the graph, and
/// gives each file to one framework: the strongest evidence wins; on equal evidence, the one whose syntax the file is
/// written in (<see cref="WrittenIn"/>); then the higher <see cref="Priority"/>; then the extension registered last, as a
/// module's language replaces a stock one. It then reads its own files and the ones it was given in one
/// <see cref="ILanguage.Read"/>. An in-scope file nothing reads is a <c>scope/unread</c> finding.
/// </para>
/// </summary>
public interface IClaimingLanguage : ILanguage
{
    /// <summary>Breaks a tie between frameworks with equally strong evidence for a file: the higher one reads it. 0 by default.</summary>
    int Priority => 0;

    /// <summary>A file of this kind can be its own, when the code says so: <c>.html</c> for a template language.</summary>
    bool MayClaim(string path);

    /// <summary>
    /// The files its code says are its own, of <paramref name="files"/>: the in-scope files no language reads by extension
    /// that it <see cref="MayClaim"/>, relative to <paramref name="root"/>, each with its evidence.
    /// </summary>
    IEnumerable<FileClaim> Claim(string root, IReadOnlyList<string> files, CodeGraph graph);

    /// <summary>
    /// The file's own text is written in its syntax (Jinja's <c>{% %}</c>, Angular's <c>@if</c> or <c>[binding]</c>). It only
    /// breaks a tie between frameworks whose code claims the file alike; it never claims a file on its own. False by default.
    /// </summary>
    bool WrittenIn(string text) => false;
}

/// <summary>How strongly the code says a file is a framework's: a folder it declares holds it, or it names the file itself.</summary>
public enum ClaimEvidence
{
    /// <summary>A folder the framework's code declares as its own holds it: <c>Jinja2Templates(directory="templates")</c>.</summary>
    Folder = 1,

    /// <summary>The code names the file itself: <c>templateUrl: './cart.html'</c>, <c>TemplateResponse(request, "cart.html")</c>.</summary>
    Named = 2
}

/// <summary>A file a framework's code claims, the evidence, and where it is, as the run result reports it: <c>templateUrl of CartPage</c>.</summary>
public sealed record FileClaim(string Path, ClaimEvidence Evidence, string Why);

/// <summary>
/// States claims about the model in code: for each element it checks, what the code must say. The claims are evaluated
/// like a pack's: a claim reader reads the code's value, the kind's comparer compares, and the finding is reported.
/// </summary>
public interface IClaimSource
{
    IEnumerable<ClaimSpec> Claims(ModelContext model);
}

/// <summary>
/// One claim an <see cref="IClaimSource"/> states. One with the same designer, element and kind as a pack's replaces it, so
/// a module can restate a stock claim.
/// </summary>
public sealed class ClaimSpec
{
    /// <summary>The designer the element is in, by the name the engine reads it under.</summary>
    public required string Designer { get; init; }

    public required IElement Element { get; init; }

    public required string Kind { get; init; }

    /// <summary>
    /// Which of several claims of the kind on the element this is: a class states one <c>index.columns</c> claim per index,
    /// each by the index's name, or its columns when it has none. Null for the only one.
    /// </summary>
    public string? Discriminator { get; init; }

    /// <summary>
    /// The value the model states, as canonical text. Null is no value: no claim for a kind stated only when set, otherwise
    /// <c>false</c>, <c>none</c> or empty by the kind's value type.
    /// </summary>
    public string? Expected { get; init; }

    /// <summary>
    /// Anything the source worked out that its claim reader needs, such as the module's own model of the element; the
    /// reader has it as <see cref="ClaimContext.ExpectedValue"/>.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// The claim it waits on, on any element of any designer: until that one is verified, this one is blocked. A claim on a
    /// member usually waits on the member's <c>member.exists</c>, and its reader then has the member.
    /// </summary>
    public ClaimRef? DependsOn { get; init; }

    /// <summary>
    /// Further claims it waits on, once <see cref="DependsOn"/> is verified, where the model states them: they don't locate
    /// its code, and block it only when contradicted. A persisted class's columns wait on its <c>entity.persisted</c>, so a
    /// table nothing maps is one finding.
    /// </summary>
    public IReadOnlyList<ClaimRef> WaitsOn { get; init; } = [];

    /// <summary>Where the model states it, for the finding: <c>its Perform Invocation links</c>.</summary>
    public string? Source { get; init; }

    /// <summary>
    /// Named values from the model that a claim reader reads, as a vocabulary entry's <c>bindings</c> give them: the
    /// <c>Repository</c> a repository's implementation is named for, or the <c>Marker</c> a marker claim looks for.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Bindings { get; init; }

    /// <summary>The report Area its finding goes in; the designer by default.</summary>
    public string? Area { get; init; }
}

/// <summary>The model, as a claim source reads it.</summary>
public sealed class ModelContext
{
    private readonly DriftEngine _engine;

    internal ModelContext(DriftEngine engine) => _engine = engine;

    /// <summary>The designers the run has, by name.</summary>
    public IReadOnlyCollection<string> Designers => _engine.DesignerNames;

    public IReadOnlyList<IPackage> Packages(string designer) => _engine.Packages(designer);

    /// <summary>Every element of a designer, each before its children.</summary>
    public IReadOnlyList<IElement> Elements(string designer) => _engine.Elements(designer);

    /// <summary>A designer's elements of one type, such as its <c>Operation</c>s, each before its children.</summary>
    public IReadOnlyList<IElement> Elements(string designer, string type) => _engine.Elements(designer).Where(x => x.SpecializationType == type).ToList();

    /// <summary>
    /// The packages a designer references from other applications, such as the Services a FrontEnd's proxies call. They
    /// are read for what they say; nothing claims on them, since their code is another application's.
    /// </summary>
    public IReadOnlyList<IPackage> References(string designer) => _engine.References(designer);

    /// <summary>Every element of the packages a designer references from other applications, each before its children.</summary>
    public IReadOnlyList<IElement> ReferencedElements(string designer) => _engine.ReferencedElements(designer);

    /// <summary>What an element is a member of: its parent, or for an association end, the element at the other end.</summary>
    public IElement? Owner(IElement element) => DriftEngine.Owner(element);

    /// <summary>
    /// What an element, or an association end, links to through associations of one type, such as an Operation's
    /// <c>Perform Invocation</c> targets: each link's target end, for a finding to point at, and the element it targets.
    /// </summary>
    public IReadOnlyList<ModelLink> Links(IElement element, string association) =>
        (element.AssociatedElements ?? [])
        .Where(x => x.Association?.SpecializationType == association && IsTarget(x) && x.TypeReference?.Element is IElement)
        .Select(x => new ModelLink(x, (IElement)x.TypeReference.Element))
        .ToList();

    /// <summary>Whether an element has a stereotype, by its name.</summary>
    public bool HasStereotype(IElement element, string stereotype) => (element.Stereotypes ?? []).Any(x => x.Name == stereotype);

    /// <summary>A stereotype property's value, by their names; null when the element hasn't the stereotype.</summary>
    public string? Stereotype(IElement element, string stereotype, string property) =>
        (element.Stereotypes ?? []).FirstOrDefault(x => x.Name == stereotype)?.Properties?.FirstOrDefault(x => x.Key == property)?.Value;

    private static bool IsTarget(IAssociationEnd end) =>
        end.Association is { } association && (ReferenceEquals(association.TargetEnd, end) || association.TargetEnd?.Id == end.Id && association.SourceEnd?.Id != end.Id);
}

/// <summary>A link in the model: the association's target end, and the element at it.</summary>
public sealed record ModelLink(IAssociationEnd End, IElement Target);

/// <summary>
/// Reads the in-scope code into what its claim readers need, and keeps it on the graph (<see cref="CodeGraph.Set{T}"/>).
/// It sees only in-scope code, through the graph, and never writes anything.
/// </summary>
public interface ICodeReader
{
    void Read(CodeGraph graph);
}

/// <summary>Reads the code's value for claims of its kinds.</summary>
public interface IClaimReader
{
    IReadOnlyCollection<string> Kinds { get; }

    /// <summary>
    /// The kinds it reads from code other than the member's own declaration, such as a framework's configuration found by the
    /// member's name: those are read even when the member isn't in the code, with <see cref="ClaimContext.Member"/> null,
    /// since what that other code lacks is a fix of its own, reported in the same run as the missing member. Every other kind
    /// waits on its member. None by default.
    /// </summary>
    IReadOnlyCollection<string> KindsWithoutMember => [];

    /// <returns>The reading, or null when this reader doesn't read the claim, so the next rule does.</returns>
    ClaimReading? Read(ClaimContext context);
}

/// <summary>
/// What a house's own code means to the claims others read: a handler's <c>String status</c> that its body turns into an
/// <c>OrderStatus</c> with a house helper binds an <c>OrderStatus</c>. Before a claim of its kinds is read, it may give the
/// member the claim's dependency located as the house reads it (<c>member with { ... }</c>), and every reader then reads that
/// one: a stack's, another module's and the engine's own rules alike. So a house adds only what it knows, and the framework's
/// reading stays the stack's. Refiners run in extension order, each on the member as the one before left it.
/// </summary>
public interface IMemberRefiner
{
    IReadOnlyCollection<string> Kinds { get; }

    /// <returns>The member as the house reads it; null to leave it as it is.</returns>
    MemberNode? Refine(ClaimContext context);
}

/// <summary>Whether a value read from the code is the value the model states.</summary>
public interface IValueComparer
{
    string Name { get; }

    bool Same(string expected, string actual);

    /// <summary>
    /// A value's canonical text (claims-and-findings.md, Values), which the report and run result show for both sides once
    /// they are compared: a route's without its leading and trailing <c>/</c> or its parameters' constraints. As read, by default.
    /// </summary>
    string Canonical(string value) => value;
}

/// <summary>A comparer from a function, and its canonical text from another.</summary>
public sealed class ValueComparer(string name, Func<string, string, bool> same, Func<string, string>? canonical = null) : IValueComparer
{
    public string Name => name;

    public bool Same(string expected, string actual) => same(expected, actual);

    public string Canonical(string value) => canonical is null ? value : canonical(value);
}
