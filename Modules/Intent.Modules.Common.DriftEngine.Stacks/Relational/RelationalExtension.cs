using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Relational;

/// <summary>
/// What a model says about a relational database, whatever framework maps it: tables, keys, columns, indexes and foreign
/// keys, from Intent's RDBMS stereotypes on the Domain designer. It reads no code; a framework's extension (EF Core,
/// JPA) reads these claims from its own code.
/// </summary>
public sealed class RelationalExtension : IDriftExtension
{
    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);

    public string Id => "intent.rdbms";

    public IEnumerable<Pack> Packs { get; } = [StockExtensions.LoadPack("intent.rdbms")];

    public IEnumerable<IClaimSource> ClaimSources { get; } = [new AttributeIndexes(), new ForeignKeys(), new Multiplicities()];

    // AttributeIndexes reads the Index stereotype, and a Foreign Key's Column Name, and ForeignKeys its Association, in code
    // rather than vocabulary. Multiplicities reads an unnavigated association end, which no vocabulary selects.
    public IEnumerable<ModelFeature> Reads { get; } =
    [
        new("Domain", Stereotype: "Index"),
        new("Domain", Stereotype: "Foreign Key", Property: "Column Name"),
        new("Domain", Stereotype: "Foreign Key", Property: "Association")
    ];

    public IEnumerable<IValueComparer> Comparers { get; } =
    [
        // SQL store types compare without whitespace or case: decimal(18, 2) is DECIMAL(18,2).
        new ValueComparer("storeType", (expected, actual) =>
            string.Equals(Whitespace.Replace(actual, ""), Whitespace.Replace(expected, ""), StringComparison.OrdinalIgnoreCase)),
        // The model may write a text default quoted or not.
        new ValueComparer("literal", (expected, actual) => Unquote(actual) == Unquote(expected))
    ];

    private static string Unquote(string value) =>
        value.Trim() is ['"', .. var inner, '"'] ? new string(inner) : value.Trim();
}

/// <summary>
/// A relationship's foreign key as the model states it: the attributes whose Foreign Key stereotype names the association,
/// in model order, on <paramref name="Dependent"/>, the class whose table holds them. The code configures it by the
/// dependent's navigation to the principal, <paramref name="DependentNavigation"/>, or by the principal's back to it,
/// <paramref name="PrincipalNavigation"/>; either may be absent.
/// </summary>
public sealed record ModelForeignKey(IElement Dependent, IElement? Principal, IReadOnlyList<IElement> Attributes, string? DependentNavigation, string? PrincipalNavigation)
{
    public IReadOnlyList<string> Columns { get; } = Attributes.Select(x => x.Name).ToList();

    public IReadOnlyList<string> Navigations { get; } = new[] { DependentNavigation, PrincipalNavigation }.OfType<string>().ToList();
}

/// <summary>
/// The foreign keys the Foreign Key stereotype states: one <c>relationship.foreignKey</c> claim per association its
/// attributes name, on the association (its target end, which Intent gives the association's id), valued with the
/// attributes' names in model order and the <see cref="ModelForeignKey"/> the framework reader needs.
/// </summary>
internal sealed class ForeignKeys : IClaimSource
{
    private const string Designer = "Domain";

    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Designer))
        {
            yield break;
        }

        var classes = model.Elements(Designer, "Class");
        var ends = classes.SelectMany(x => x.AssociatedElements ?? []).DistinctBy(x => x.Id).ToDictionary(x => x.Id);
        var keyed = classes
            .SelectMany(type => type.ChildElements.Where(x => x.SpecializationType == "Attribute" && model.HasStereotype(x, "Foreign Key"))
                .Select(attribute => (Type: type, Attribute: attribute, End: Association(attribute))))
            .Where(x => x.End is not null)
            .GroupBy(x => (x.Type.Id, x.End!.Id));
        foreach (var group in keyed)
        {
            var (type, _, end) = group.First();
            var columns = group.Select(x => x.Attribute.Name).ToList();
            // An end is named for the navigation on the other class that leads to its own: the end whose type is the
            // principal is the dependent's navigation to it.
            var toPrincipal = (end!.OtherEnd()?.TypeReference?.Element as IElement)?.Id == type.Id ? end : end.OtherEnd()!;
            var principal = toPrincipal.TypeReference?.Element as IElement;
            static string? Named(IAssociationEnd? navigation) => string.IsNullOrEmpty(navigation?.Name) ? null : navigation.Name;
            yield return new ClaimSpec
            {
                Designer = Designer,
                Element = end,
                Kind = "relationship.foreignKey",
                Expected = string.Join(", ", columns),
                Value = new ModelForeignKey(type, principal, group.Select(x => x.Attribute).ToList(), Named(toPrincipal), Named(toPrincipal.OtherEnd())),
                DependsOn = new ClaimRef(Designer, type.Id, "type.exists"),
                WaitsOn = [new ClaimRef(Designer, type.Id, "entity.persisted")],
                Source = $"the Foreign Key stereotype on {string.Join(" and ", columns)}"
            };
        }

        // The association end the Foreign Key's Association names, as its target end: the association's own element. A lookup
        // is handed over as the element, or as its id, as Intent stores it.
        IAssociationEnd? Association(IElement attribute)
        {
            var property = attribute.Stereotypes?.FirstOrDefault(x => x.Name == "Foreign Key")?.Properties.FirstOrDefault(x => x.Key == "Association");
            var named = (property as IStereotypeProperty<IElement>)?.Value as IAssociationEnd
                        ?? (property?.Value is { Length: > 0 } id ? ends.GetValueOrDefault(id) : null);
            return named is null ? null : named.IsTargetEnd() ? named : named.OtherEnd();
        }
    }
}

/// <summary>
/// A relationship's end that no navigation property states (user decision, 2026-11-15): the navigable end is
/// <paramref name="Navigation"/>, a property of <paramref name="Owner"/> leading to <paramref name="Target"/>, and
/// <paramref name="Unnavigated"/> says how many <paramref name="Owner"/>s each <paramref name="Target"/> has. The navigable
/// end's own multiplicity is its property's: <c>member.type</c> says whether it is a collection, <c>member.nullable</c>
/// whether it is optional.
/// </summary>
public sealed record ModelMultiplicity(IAssociationEnd Navigation, IAssociationEnd Unnavigated, IElement Owner, IElement Target);

/// <summary>
/// The <c>relationship.multiplicity</c> claims: one per association whose one end is navigable and other isn't, from a
/// class the model persists (a Table or View), on the association (its target end), valued with the unnavigated end's
/// multiplicity (<c>1</c>, <c>0..1</c>, <c>*</c> or <c>1..*</c>). Each payment method has exactly one buyer:
/// <c>Buyer</c> is <c>1</c> where only <c>Buyer.PaymentMethods</c> is navigable. It waits on the navigation's
/// <c>member.exists</c>, as the framework reads the relationship from it, then on its owner's <c>entity.persisted</c>. A
/// navigation from a class the model doesn't persist states no mapped relationship, so it has no claim (user decision,
/// 2026-11-15).
/// </summary>
internal sealed class Multiplicities : IClaimSource
{
    private const string Designer = "Domain";

    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Designer))
        {
            yield break;
        }

        var associations = model.Elements(Designer, "Class")
            .SelectMany(x => x.AssociatedElements ?? [])
            .Where(x => x.Association?.SpecializationType == "Association")
            .DistinctBy(x => x.Association!.Id);
        foreach (var end in associations)
        {
            var (navigation, unnavigated) = end.IsNavigable ? (end, end.OtherEnd()) : (end.OtherEnd(), end);
            if (navigation is null || unnavigated is null || !navigation.IsNavigable || unnavigated.IsNavigable
                || navigation.TypeReference?.Element is not IElement target || unnavigated.TypeReference?.Element is not IElement owner
                || !model.HasStereotype(owner, "Table") && !model.HasStereotype(owner, "View") || Multiplicity(unnavigated) is not { } expected)
            {
                continue;
            }

            yield return new ClaimSpec
            {
                Designer = Designer,
                Element = navigation.IsTargetEnd() ? navigation : unnavigated,
                Kind = "relationship.multiplicity",
                Expected = expected,
                Value = new ModelMultiplicity(navigation, unnavigated, owner, target),
                DependsOn = new ClaimRef(Designer, navigation.Id, "member.exists"),
                WaitsOn = [new ClaimRef(Designer, owner.Id, "entity.persisted")],
                Source = $"the multiplicity of the {(string.IsNullOrEmpty(unnavigated.Name) ? owner.Name : unnavigated.Name)} end, which no navigation property of {target.Name} states"
            };
        }
    }

    private static string? Multiplicity(IAssociationEnd end) => end.TypeReference is not { } type ? null
        : type.IsCollection ? type.IsNullable ? "*" : "1..*"
        : type.IsNullable ? "0..1" : "1";
}

/// <summary>
/// An index the model states with the Index stereotype on a class's attributes and association ends, as its framework
/// reader finds it.
/// </summary>
/// <param name="Name">Its UniqueKey; null for an unnamed index on one attribute.</param>
/// <param name="Columns">
/// In their Order: an attribute's name, and an association end's Foreign Key Column Name, else its name.
/// </param>
public sealed record ModelIndex(string? Name, IReadOnlyList<string> Columns, bool Unique);

/// <summary>
/// The indexes the Index stereotype states on a class's attributes, and on the association target ends whose foreign key
/// column is in its table: those sharing a UniqueKey are one index, in their Order; one with no UniqueKey is an index of
/// its own. Each is a claim on the class, told apart by its name, else its columns, with the <see cref="ModelIndex"/> as
/// the claim's value.
/// </summary>
internal sealed class AttributeIndexes : IClaimSource
{
    private const string Designer = "Domain";

    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Designer))
        {
            yield break;
        }

        var classes = model.Elements(Designer, "Class");
        var ends = classes.SelectMany(x => x.AssociatedElements ?? [])
            .Where(x => x.IsTargetEnd() && model.HasStereotype(x, "Index"))
            .DistinctBy(x => x.Id)
            .ToLookup(x => TableOf(x)?.Id);
        foreach (var type in classes)
        {
            var indexed = type.ChildElements.Where(x => x.SpecializationType == "Attribute" && model.HasStereotype(x, "Index")).Concat(ends[type.Id]).ToList();
            var indexes = indexed
                .GroupBy(x => model.Stereotype(x, "Index", "UniqueKey") is { Length: > 0 } key ? key : $"\0{x.Id}")
                .Select(group =>
                {
                    var columns = group.OrderBy(x => int.TryParse(model.Stereotype(x, "Index", "Order"), out var order) ? order : int.MaxValue).ToList();
                    var name = group.Key.StartsWith('\0') ? null : group.Key;
                    return new ModelIndex(name, columns.Select(x => Column(model, x)).ToList(),
                        columns.Any(x => string.Equals(model.Stereotype(x, "Index", "IsUnique"), "true", StringComparison.OrdinalIgnoreCase)));
                });
            foreach (var index in indexes)
            {
                var source = index.Name is null ? $"the Index stereotype on {index.Columns[0]}" : $"the Index stereotypes with UniqueKey {index.Name}";
                foreach (var (kind, expected) in new[] { ("index.columns", string.Join(", ", index.Columns)), ("index.unique", index.Unique ? "true" : "false") })
                {
                    yield return new ClaimSpec
                    {
                        Designer = Designer,
                        Element = type,
                        Kind = kind,
                        Discriminator = index.Name ?? string.Join(",", index.Columns),
                        Expected = expected,
                        Value = index,
                        DependsOn = new ClaimRef(Designer, type.Id, "type.exists"),
                        WaitsOn = [new ClaimRef(Designer, type.Id, "entity.persisted")],
                        Source = source
                    };
                }
            }
        }
    }

    /// <summary>
    /// The class whose table holds an association end's foreign key column: a one-to-many's "many" side, else the class the
    /// end navigates from. A many-to-many's is a join table, which no class's index covers.
    /// </summary>
    private static IElement? TableOf(IAssociationEnd end) =>
        end.TypeReference?.IsCollection == true
            ? end.OtherEnd()?.TypeReference?.IsCollection == true ? null : end.TypeReference.Element as IElement
            : end.OtherEnd()?.TypeReference?.Element as IElement;

    // An association end is named for its navigation, so its column is its Foreign Key Column Name, when the model states it.
    private static string Column(ModelContext model, IElement element) =>
        element is IAssociationEnd && model.Stereotype(element, "Foreign Key", "Column Name") is { Length: > 0 } column ? column : element.Name;
}
