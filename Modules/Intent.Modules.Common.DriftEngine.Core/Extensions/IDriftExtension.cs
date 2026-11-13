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

    /// <summary>Its claim kinds, vocabulary, conventions, recipes and language profiles, as data.</summary>
    IEnumerable<Pack> Packs => [];

    /// <summary>
    /// The languages it reads. The engine knows none itself: a language reads its files into the graph's types, members
    /// and files, and keeps its own syntax there for the extensions that read that language.
    /// </summary>
    IEnumerable<ILanguage> Languages => [];

    /// <summary>Readers run over the in-scope code once the language readers have built the graph.</summary>
    IEnumerable<ICodeReader> Readers => [];

    IEnumerable<IClaimReader> ClaimReaders => [];

    /// <summary>Comparers for its kinds, named by a kind's <c>comparer</c>.</summary>
    IEnumerable<IValueComparer> Comparers => [];

    /// <summary>Rules over the model and the code together, for what isn't one claim's value.</summary>
    IEnumerable<IDriftRule> Rules => [];
}

/// <summary>
/// A rule over the model and the code together (plan.md, block 4): a check that isn't one claim's value, such as every
/// import of a Layer against its May Depend On arrows. It runs once the claims are evaluated, over in-scope code only,
/// and never writes anything.
/// </summary>
public interface IDriftRule
{
    /// <returns>
    /// Its findings. Each names its Area, as the report shows it; the engine marks the Area as its own, so the findings
    /// sit apart from another checker's.
    /// </returns>
    IEnumerable<Finding> Check(RuleContext context);
}

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

    /// <param name="root">The output folder.</param>
    /// <param name="files">Its in-scope files, relative to <paramref name="root"/>, with <c>/</c> separators, in order.</param>
    /// <param name="graph">The graph to add to; a file it can't read goes in <see cref="CodeGraph.Failures"/>.</param>
    void Read(string root, IReadOnlyList<string> files, CodeGraph graph);
}

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

    /// <summary>Where the model states it, for the finding: <c>its Perform Invocation links</c>.</summary>
    public string? Source { get; init; }

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

    /// <returns>The reading, or null when this reader doesn't read the claim, so the next rule does.</returns>
    ClaimReading? Read(ClaimContext context);
}

/// <summary>Whether a value read from the code is the value the model states.</summary>
public interface IValueComparer
{
    string Name { get; }

    bool Same(string expected, string actual);
}

/// <summary>A comparer from a function.</summary>
public sealed class ValueComparer(string name, Func<string, string, bool> same) : IValueComparer
{
    public string Name => name;

    public bool Same(string expected, string actual) => same(expected, actual);
}
