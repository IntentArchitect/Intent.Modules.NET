using System.Text.RegularExpressions;
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
