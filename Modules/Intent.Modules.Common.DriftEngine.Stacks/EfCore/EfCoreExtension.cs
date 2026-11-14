using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// How EF Core code states the relational claims (<see cref="Relational.RelationalExtension"/>): its fluent API and data
/// annotations. It adds no claims of its own, so a JPA extension reads the same claims from its annotations.
/// </summary>
public sealed class EfCoreExtension : IDriftExtension
{
    public string Id => "intent.efcore";

    public IEnumerable<ICodeReader> Readers { get; } = [new EfCoreReader()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new EfCoreClaims()];
}

internal sealed class EfCoreClaims : IClaimReader
{
    /// <summary>A text store type and its length: <c>varchar(50)</c>, <c>nchar(1)</c>, <c>ntext</c>.</summary>
    private static readonly Regex TextType = new(@"^\s*(n?varchar|n?char|n?text)\s*(?:\(\s*(\d+)\s*\)|\(\s*max\s*\))?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex DecimalType = new(@"^\s*(?:decimal|numeric)\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "table.name", "table.schema", "table.key", "index.columns", "index.unique", "index.filter", "column.foreignKey", "column.name", "column.type",
        "column.textType", "column.maxLength", "column.precision", "column.scale", "column.default", "column.valueGeneration", "member.nullable", "member.exists"
    };

    public ClaimReading? Read(ClaimContext context)
    {
        // No EF Core in the code: the claim is another framework's to read, or can't be verified.
        if (context.Code.Get<EfCoreModel>() is not { } model)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "member.exists":
                return Shadow(context, model);
            case "table.name" or "table.schema":
            {
                var type = context.Type!;
                var toTable = EntityOf(context, model, type)?.Calls.FirstOrDefault(x => x.Name is "ToTable" or "ToView");
                return Value(toTable?.Arguments.ElementAtOrDefault(context.Kind == "table.name" ? 0 : 1), toTable?.At ?? type.At);
            }
            case "table.key":
                return Key(context, context.Type!, model);
            case "index.columns" or "index.unique" or "index.filter":
                return Index(context, context.Type!, model);
            case "column.foreignKey":
            {
                var member = context.Member!;
                var (navigation, at) = ForeignKey(context, Owner(context), member, model);
                if (navigation is null && context.ClaimOn(context.Element, context.Kind)?.Expected is { Length: > 0 } expected
                    && ConventionalForeignKey(context, Owner(context), member, model, expected) is { } conventional)
                {
                    return conventional;
                }

                return Value(navigation, at ?? member.At);
            }
            case "member.nullable":
                return Required(context, model);
            default:
            {
                var member = context.Member!;
                var calls = EntityOf(context, model, Owner(context))?.Properties.GetValueOrDefault(member.Name) ?? [];
                var (value, at) = Column(context.Kind, calls, member);
                return Value(value, at ?? member.At);
            }
        }
    }

    /// <summary>The type of the class the claim's attribute belongs to.</summary>
    private static TypeNode Owner(ClaimContext context) => context.TypeOf(context.Owner(context.Element)!)!;

    private static EntityConfig? EntityOf(ClaimContext context, EfCoreModel model, TypeNode type) => model.Of(type, context.Code);

    private static ClaimReading Value(string? value, Span at) => value is null ? ClaimReading.Absent(at) : ClaimReading.Stated(value, at);

    /// <summary>
    /// An EF Core shadow property: the entity's configuration declares <c>Property&lt;int&gt;("BuyerId")</c> and its class has
    /// no member by that name. It is the entity's member as EF Core sees it, typed by the call, so the claims on it read
    /// that. Anything else is left to the engine's own rule.
    /// </summary>
    private static ClaimReading? Shadow(ClaimContext context, EfCoreModel model)
    {
        if (context.Element is IAssociationEnd || context.Owner(context.Element) is not { } owner || context.TypeOf(owner) is not { } type
            || context.Code.MembersOf(type).Any(x => x.Name == context.Element.Name)
            || EntityOf(context, model, type) is not { } entity || !entity.Declared.TryGetValue(context.Element.Name, out var declared))
        {
            return null;
        }

        return ClaimReading.Found(new MemberNode { Name = context.Element.Name, Kind = "property", Type = declared.Type, At = declared.At });
    }

    /// <summary>
    /// HasKey, else [Key] on its properties, else [PrimaryKey] on the type. A key is a set: EF orders a composite key by
    /// HasKey, the model by its attributes.
    /// </summary>
    private static ClaimReading Key(ClaimContext context, TypeNode type, EfCoreModel model)
    {
        var hasKey = EntityOf(context, model, type)?.Calls.LastOrDefault(x => x.Name == "HasKey");
        var keyed = context.Code.MembersOf(type).Where(x => x.Attributes.Any(a => a.Name == "Key")).ToList();
        var primaryKey = type.Attributes.FirstOrDefault(x => x.Name == "PrimaryKey");
        var (columns, at) = hasKey is not null ? (hasKey.Selected, hasKey.At)
            : keyed.Count > 0 ? (keyed.Select(x => x.Name).ToList(), keyed[0].At)
            : primaryKey is not null ? (primaryKey.Arguments.Select(x => x.Trim('"')).ToList(), primaryKey.At)
            : ((IReadOnlyList<string>)[], type.At);
        if (columns.Count > 0)
        {
            return ClaimReading.Stated(string.Join(", ", columns), at);
        }

        // The Domain designer always marks a key, so unlike a Table Name it can't leave a conventional one unstated: with
        // nothing explicit, the code's key is EF Core's, a property named Id or <Entity>Id.
        var conventional = context.Code.MembersOf(type).FirstOrDefault(x => x.Kind == "property"
            && (string.Equals(x.Name, "Id", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, $"{type.Name}Id", StringComparison.OrdinalIgnoreCase)));
        return conventional is null ? ClaimReading.Absent(at) : ClaimReading.Defaulted(conventional.Name, conventional.At);
    }

    /// <summary>
    /// The <c>HasIndex</c> chain an index element states: the one with its name (<c>HasIndex(e =&gt; e.A, "IX_A")</c> or
    /// <c>HasDatabaseName</c>), else the one on its columns.
    /// </summary>
    private static ClaimReading Index(ClaimContext context, TypeNode type, EfCoreModel model)
    {
        var chains = EntityOf(context, model, type)?.Chains.Where(x => x[0].Name == "HasIndex").ToList() ?? [];
        // The Index stereotypes' index, a claim on the class, has its own name and columns; an Index element is named.
        var stated = context.ExpectedValue as Relational.ModelIndex;
        var name = stated is null ? context.Element.Name : stated.Name;
        var columns = stated is null ? context.ClaimOn(context.Element, "index.columns")?.Expected : string.Join(", ", stated.Columns);
        var chain = chains.FirstOrDefault(x => name is not null && (x[0].Arguments.ElementAtOrDefault(1) == name || x.Any(c => c.Name == "HasDatabaseName" && c.Arguments is [var named] && named == name)))
                    ?? chains.FirstOrDefault(x => columns is not null && string.Join(", ", x[0].Selected) == columns);
        if (chain is null)
        {
            return ClaimReading.Absent(type.At);
        }

        switch (context.Kind)
        {
            case "index.columns":
                return ClaimReading.Stated(string.Join(", ", chain[0].Selected), chain[0].At);
            case "index.unique":
                var unique = chain.LastOrDefault(x => x.Name == "IsUnique");
                return unique is null
                    ? ClaimReading.Absent(chain[0].At, "false")
                    : ClaimReading.Stated(unique.Arguments is [] or ["true"] ? "true" : "false", unique.At);
            default:
                var filter = chain.LastOrDefault(x => x.Name == "HasFilter");
                return Value(filter?.Arguments.FirstOrDefault(), filter?.At ?? chain[0].At);
        }
    }

    /// <summary>
    /// The navigation a property is the foreign key of: the entity's own <c>HasOne(d =&gt; d.Nav)...HasForeignKey(d =&gt; d.Fk)</c>,
    /// the principal's <c>...WithOne(d =&gt; d.Nav).HasForeignKey(d =&gt; d.Fk)</c>, or <c>[ForeignKey]</c> on either property.
    /// </summary>
    private static (string? Navigation, Span? At) ForeignKey(ClaimContext context, TypeNode owner, MemberNode member, EfCoreModel model)
    {
        static FluentCall? ForeignKeyCall(IReadOnlyList<FluentCall> chain, string name) =>
            chain.FirstOrDefault(x => x.Name == "HasForeignKey" && x.Selected.Contains(name));

        if (EntityOf(context, model, owner) is { } entity)
        {
            foreach (var chain in entity.Chains.Where(x => x[0].Name == "HasOne"))
            {
                if (ForeignKeyCall(chain, member.Name) is { } call)
                {
                    return (chain[0].Selected.FirstOrDefault(), call.At);
                }
            }
        }

        foreach (var other in model.Entities.Values.Where(x => x.Entity != owner.Name))
        {
            foreach (var chain in other.Chains)
            {
                if (ForeignKeyCall(chain, member.Name) is { } call && chain.LastOrDefault(x => x.Name == "WithOne") is { Selected: [var navigation] })
                {
                    return (navigation, call.At);
                }
            }
        }

        if (member.Attributes.FirstOrDefault(x => x.Name == "ForeignKey") is { Arguments: [var named] } annotation)
        {
            return (named, annotation.At);
        }

        return context.Code.MembersOf(owner).FirstOrDefault(x => x.Attributes.Any(a => a.Name == "ForeignKey" && a.Arguments is [var key] && key == member.Name)) is { } navigationProperty
            ? (navigationProperty.Name, navigationProperty.At)
            : (null, null);
    }

    /// <summary>
    /// EF Core's foreign key discovery, for the relationship the model names (<paramref name="navigation"/>) when the code
    /// gives it no <c>HasForeignKey</c> or <c>[ForeignKey]</c>: the dependent's property named after a navigation on the
    /// dependent or after the principal type, followed by the principal's key or <c>Id</c> (<c>BuyerId</c>). Like a key, the
    /// Domain designer always marks a relationship's foreign key, so it can't leave a conventional one unstated. The
    /// navigation is the dependent's, to the principal, or the principal's, to the dependent; one EF Core ignores relates
    /// nothing.
    /// </summary>
    private static ClaimReading? ConventionalForeignKey(ClaimContext context, TypeNode dependent, MemberNode member, EfCoreModel model, string navigation)
    {
        static bool Holds(MemberNode candidate, string name, TypeNode type) =>
            candidate.Kind == "property" && candidate.Name == name && candidate.Type is { } held && (held.Name == type.Name || held.Arguments.Any(x => x.Name == type.Name));

        var principal = context.Code.MembersOf(dependent).FirstOrDefault(x => x.Kind == "property" && x.Name == navigation) is { Type: { } type }
            ? context.Code.Types.FirstOrDefault(x => x.Name == type.Name)
            : context.Code.Types.FirstOrDefault(x => context.Code.MembersOf(x).Any(m => Holds(m, navigation, dependent)));
        if (principal is null)
        {
            return null;
        }

        var configs = new[] { EntityOf(context, model, dependent), EntityOf(context, model, principal) }.OfType<EntityConfig>().ToList();
        var related = configs.SelectMany(x => x.Chains).Where(chain => chain.Any(c => c.Name is "HasOne" or "HasMany" or "WithOne" or "WithMany" && c.Selected.Contains(navigation))).ToList();
        var navigationMember = context.Code.MembersOf(dependent).Concat(context.Code.MembersOf(principal)).First(x => x.Kind == "property" && x.Name == navigation);
        if (related.Any(chain => chain.Any(c => c.Name == "HasForeignKey"))
            || configs.Any(x => x.Calls.Any(c => c.Name == "Ignore" && c.Selected.Contains(navigation)))
            || navigationMember.Attributes.Any(x => x.Name == "ForeignKey"))
        {
            return null;
        }

        var key = EntityOf(context, model, principal)?.Calls.LastOrDefault(x => x.Name == "HasKey")?.Selected is [var single] ? single : "Id";
        var prefixes = context.Code.MembersOf(dependent).Where(x => Holds(x, x.Name, principal)).Select(x => x.Name).Append(principal.Name);
        return prefixes.Any(prefix => string.Equals(member.Name, prefix + key, StringComparison.OrdinalIgnoreCase) || string.Equals(member.Name, prefix + "Id", StringComparison.OrdinalIgnoreCase))
            ? ClaimReading.Defaulted(navigation, navigationMember.At)
            : null;
    }

    /// <summary>
    /// EF Core's own rule for nullability where the C# type can't state it (outside a nullable context): <c>IsRequired()</c>
    /// or <c>[Required]</c> makes a mapped property's column required; without either, EF Core maps a reference type as
    /// optional (decision 13: an extension adds a rule to another's kind). Anywhere else the engine's own rule reads it, and
    /// association ends are left to their foreign keys.
    /// </summary>
    private static ClaimReading? Required(ClaimContext context, EfCoreModel model)
    {
        if (context.Element is IAssociationEnd || context.Member is not { } member || member.Type?.Nullable is not (null or "unknown")
            || context.Code.UngovernedFiles.ContainsKey(member.At.File)
            || context.Owner(context.Element) is not { } ownerElement || context.TypeOf(ownerElement) is not { } type
            || EntityOf(context, model, type) is not { } entity)
        {
            return null;
        }

        if (entity.Properties.GetValueOrDefault(member.Name)?.LastOrDefault(x => x.Name == "IsRequired") is { } required)
        {
            return ClaimReading.Stated(required.Arguments is ["false"] ? "true" : "false", required.At);
        }

        // A key's columns are always required.
        if (entity.Calls.FirstOrDefault(x => x.Name == "HasKey" && x.Selected.Contains(member.Name)) is { } key)
        {
            return ClaimReading.Stated("false", key.At);
        }

        return member.Attributes.FirstOrDefault(x => x.Name == "Required") is { } annotation
            ? ClaimReading.Stated("false", annotation.At)
            : ClaimReading.Defaulted("true", member.At);
    }

    /// <summary>
    /// What a property's fluent configuration states for a column kind, or a data annotation on the property. A decimal's
    /// precision and scale may be written in its column type, <c>decimal(18, 2)</c>.
    /// </summary>
    private static (string? Value, Span? At) Column(string kind, IReadOnlyList<FluentCall> calls, MemberNode member)
    {
        FluentCall? Last(params string[] names) => calls.LastOrDefault(x => names.Contains(x.Name));
        AttributeNode? Annotation(params string[] names) => member.Attributes.FirstOrDefault(x => names.Contains(x.Name));
        switch (kind)
        {
            case "column.name":
                return Last("HasColumnName") is { } name ? (name.Arguments.FirstOrDefault(), name.At)
                    : Annotation("Column") is { Arguments.Count: > 0 } column ? (column.Arguments[0], column.At) : (null, null);
            case "column.type":
            {
                if (Last("HasColumnType") is { } type)
                {
                    return (type.Arguments.FirstOrDefault(), type.At);
                }

                // A text column's store type stated by its facets: HasMaxLength(1).IsUnicode(false).IsFixedLength() is char(1).
                var fixedLength = Last("IsFixedLength") is { Arguments: [] or ["true"] };
                var ansi = Last("IsUnicode") is { Arguments: ["false"] };
                return (fixedLength || ansi) && Last("HasMaxLength") is { Arguments: [var length] } maxLength
                    ? ($"{(ansi ? "" : "n")}{(fixedLength ? "char" : "varchar")}({length})", maxLength.At)
                    : (null, null);
            }
            case "column.textType":
            {
                // The model's SQL Data Type options: a fixed-length char column is the nearest, VARCHAR or NVARCHAR.
                if (Last("HasColumnType") is { } type && TextType.Match(type.Arguments.FirstOrDefault() ?? "") is { Success: true } match)
                {
                    return (match.Groups[1].Value.ToLowerInvariant() switch
                    {
                        "varchar" or "char" => "VARCHAR",
                        "nvarchar" or "nchar" => "NVARCHAR",
                        var other => other.ToUpperInvariant()
                    }, type.At);
                }

                return Last("IsUnicode") is { } unicode ? (unicode.Arguments is ["false"] ? "VARCHAR" : "NVARCHAR", unicode.At) : (null, null);
            }
            case "column.maxLength":
                return Last("HasMaxLength") is { } max ? (max.Arguments.FirstOrDefault(), max.At)
                    : Annotation("MaxLength", "StringLength") is { Arguments.Count: > 0 } annotation ? (annotation.Arguments[0], annotation.At)
                    : Last("HasColumnType") is { } sized && TextType.Match(sized.Arguments.FirstOrDefault() ?? "") is { Groups: [_, _, { Success: true } size] } ? (size.Value, sized.At)
                    : (null, null);
            case "column.default":
                // A SQL default is the string's content; a value default is the C# literal as written.
                return Last("HasDefaultValueSql", "HasDefaultValue") is { } value
                    ? (value.Name == "HasDefaultValueSql" ? value.Arguments.FirstOrDefault() : value.Written.FirstOrDefault(), value.At)
                    : (null, null);
            case "column.valueGeneration":
            {
                // The Primary Key's Data source: User supplied is ValueGeneratedNever(), Auto-generated a call that has the
                // database generate it.
                if (Last("ValueGeneratedNever", "ValueGeneratedOnAdd", "UseIdentityColumn", "UseHiLo", "UseSequence") is { } generation)
                {
                    return (generation.Name == "ValueGeneratedNever" ? "User supplied" : "Auto-generated", generation.At);
                }

                return Annotation("DatabaseGenerated") is { Arguments: [var option, ..] } generated
                    ? (option.Split('.')[^1] switch { "None" => "User supplied", "Identity" => "Auto-generated", var other => other }, generated.At)
                    : (null, null);
            }
            default:
            {
                var index = kind == "column.precision" ? 0 : 1;
                if (Last("HasPrecision") is { } precision)
                {
                    return (precision.Arguments.ElementAtOrDefault(index), precision.At);
                }

                return Last("HasColumnType") is { } columnType && DecimalType.Match(columnType.Arguments.FirstOrDefault() ?? "") is { Success: true } match
                    ? (match.Groups[index + 1].Value, columnType.At)
                    : (null, null);
            }
        }
    }
}
