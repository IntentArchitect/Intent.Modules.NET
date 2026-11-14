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

    public IEnumerable<IClaimSource> ClaimSources { get; } = [new AttributeIndexes()];

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
