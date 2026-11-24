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

    public IEnumerable<IOptInRule> OptInRules { get; } = [new UnmodelledIndexes()];
}

internal sealed partial class EfCoreClaims : IClaimReader
{
    /// <summary>A text store type and its length: <c>varchar(50)</c>, <c>nchar(1)</c>, <c>ntext</c>.</summary>
    private static readonly Regex TextType = new(@"^\s*(n?varchar|n?char|n?text)\s*(?:\(\s*(\d+)\s*\)|\(\s*max\s*\))?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex DecimalType = new(@"^\s*(?:decimal|numeric)\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "entity.persisted", "table.name", "table.schema", "key", "index.columns", "index.unique", "index.filter", "index.name", "relationship.foreignKey", "relationship.multiplicity", "table.kind", "column.name", "column.type",
        "column.textType", "column.maxLength", "column.precision", "column.scale", "column.default", "column.defaultSql", "key.generated", "member.nullable", "member.exists",
        "column.mapped", "column.owned", "column.enumStorage", "column.version", "column.backingField", "column.hilo", "relationship.onDelete",
        "relationship.reference", "relationship.navigable", "relationship.joinColumn"
    };

    /// <summary>
    /// What the entity's configuration states about a column, read by the attribute's name, so a property the code lacks
    /// still has its missing configuration reported in the same run: <c>IsRequired()</c>, <c>HasColumnName</c>,
    /// <c>HasMaxLength</c> and the rest.
    /// </summary>
    public IReadOnlyCollection<string> KindsWithoutMember { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "member.nullable", "column.name", "column.type", "column.textType", "column.maxLength", "column.precision", "column.scale", "column.default", "column.defaultSql"
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
            case "entity.persisted":
                return Persisted(context, model);
            case "table.name" or "table.schema":
            {
                // The fluent configuration, else the data annotation it overrides: [Table("Account", Schema = "crm")], which
                // EF6 and EF Core both read (CrmBus's EF6 entities state their tables only this way).
                var type = context.Type!;
                var toTable = EntityOf(context, model, type)?.Calls.FirstOrDefault(x => x.Name is "ToTable" or "ToView");
                var annotation = toTable is null ? type.Attributes.FirstOrDefault(x => x.Name == "Table") : null;
                var arguments = new Arguments(context);
                var value = toTable is not null ? arguments.Of(toTable, context.Kind == "table.name" ? 0 : 1)
                    : annotation is null ? null
                    : context.Kind == "table.name" ? arguments.Of(annotation, 0) : arguments.Of(annotation, "Schema");
                if (value is null && arguments.Unread is null && context.Kind == "table.schema")
                {
                    return DefaultSchema(context, model, type, toTable, arguments);
                }

                return value is null && arguments.Unread is { } unread ? arguments.Unreadable(unread) : Value(value, toTable?.At ?? annotation?.At ?? type.At);
            }
            case "key":
                return Key(context, context.Type!, model);
            case "index.columns" or "index.unique" or "index.filter" or "index.name":
                return Index(context, context.Type!, model);
            case "relationship.foreignKey":
                return RelationshipForeignKey(context, model);
            case "relationship.multiplicity":
                return Multiplicity(context, model);
            case "table.kind":
            {
                // ToView maps a view; anything else, a table, which [Table] and silence state too. Both are ambiguous.
                var type = context.Type!;
                var mappings = EntityOf(context, model, type)?.Calls.Where(x => x.Name is "ToTable" or "ToView").ToList() ?? [];
                return mappings.Select(x => x.Name).Distinct().Count() > 1
                    ? ClaimReading.CantVerify("ambiguous", "It is mapped both to a table and to a view.", mappings[0].At, mappings.Select(x => x.At).ToList())
                    : mappings.FirstOrDefault() is { } mapping
                        ? ClaimReading.Stated(mapping.Name == "ToView" ? "View" : "Table", mapping.At)
                        : type.Attributes.FirstOrDefault(x => x.Name == "Table") is { } annotation
                            ? ClaimReading.Stated("Table", annotation.At)
                            : ClaimReading.Defaulted("Table", type.At);
            }
            case "member.nullable":
                return Required(context, model);
            case "column.mapped":
                return Mapped(context, model);
            case "column.owned":
                return Owned(context, model);
            case "column.version":
                return Version(context, model);
            case "column.enumStorage":
                return EnumStorage(context, model);
            case "column.hilo":
                return HiLo(context, model);
            case "column.backingField":
                return BackingField(context, model);
            case "relationship.onDelete":
                return OnDelete(context, model);
            case "relationship.reference":
                return Reference(context, model);
            case "relationship.navigable":
                return Navigable(context, model);
            case "relationship.joinColumn":
                return JoinColumn(context, model);
            default:
            {
                // A member the code lacks is configured, if at all, by the attribute's name, and has no annotations.
                var member = context.Member;
                var owner = Owner(context);
                var entity = EntityOf(context, model, owner);
                if (member is null && entity is null)
                {
                    return null;
                }

                var calls = entity?.Properties.GetValueOrDefault(member?.Name ?? context.Element.Name) ?? [];
                // A key configured twice, never generated in one chain and generated in another: which EF Core keeps is its merge's.
                var generations = calls.Where(x => x.Name is "ValueGeneratedNever" or "ValueGeneratedOnAdd" or "UseIdentityColumn" or "UseHiLo" or "UseSequence").ToList();
                if (context.Kind == "key.generated" && generations.Select(x => x.Name == "ValueGeneratedNever").Distinct().Count() > 1)
                {
                    return ClaimReading.CantVerify("ambiguous", $"{generations.Count} calls configure how the key is generated, differently.", generations[0].At, generations.Select(x => x.At).ToList());
                }

                var arguments = new Arguments(context);
                var (value, at) = Column(context.Kind, calls, member, arguments);
                if (value is not null && at is { } given && StatedOtherwise(context, model, calls, value))
                {
                    return ClaimReading.Defaulted(value, given);
                }

                // What the code doesn't state is against the property, or for one it lacks, the entity's configuration. A
                // provider's own text type isn't a fix the missing property needs, so it waits on the property.
                if (member is null && value is null && context.Kind == "column.textType")
                {
                    return null;
                }

                var where = member?.At ?? ConfigurationOf(entity!);
                return value is null && arguments.Unread is { } unread ? arguments.Unreadable(unread)
                    // A string property that states no text type leaves it to the provider: the model's DEFAULT.
                    : value is null && context.Kind == "column.textType" ? ClaimReading.Absent(at ?? where, "DEFAULT")
                    : Value(value, at ?? where);
            }
        }
    }

    /// <summary>
    /// A store type, or the precision and scale in one, that the model states other than as the kind asks, when the model
    /// leaves the kind unset (<c>unmodelled.value</c>'s probe): a text type is its text type and length (column.textType,
    /// column.maxLength), a decimal type its precision and scale, so <c>varchar(50)</c> on an attribute whose Text
    /// Constraints say VARCHAR and 50 isn't a column type the model lacks. A decimal type the model has no precision for is
    /// reported once, as its column type, not again as its precision and scale. A key generation that is EF Core's own
    /// for the key is what the model's Default Data source says.
    /// </summary>
    private static bool StatedOtherwise(ClaimContext context, EfCoreModel model, IReadOnlyList<FluentCall> calls, string value)
    {
        if (context.ClaimOn(context.Element, context.Kind) is not null)
        {
            return false;
        }

        return context.Kind switch
        {
            "column.type" => TextType.IsMatch(value) && context.ClaimOn(context.Element, "column.textType") is not null
                             || DecimalType.IsMatch(value) && context.ClaimOn(context.Element, "column.precision") is not null,
            "column.precision" or "column.scale" => calls.All(x => x.Name != "HasPrecision"),
            // A length the model states in the attribute's Column Type, char(1).
            "column.maxLength" => context.ClaimOn(context.Element, "column.type") is { } typed && TextType.IsMatch(typed.Expected),
            "key.generated" => value == ConventionalGeneration(context, model),
            _ => false
        };
    }

    /// <summary>
    /// How EF Core generates a key's values when the code doesn't say: a key of one integer or Guid property on add, any other
    /// key's never. The key is HasKey's, else the [Key] properties, else, by convention, a property named Id or
    /// <c>&lt;Entity&gt;Id</c>; with a HasKey the convention doesn't apply.
    /// </summary>
    private static string? ConventionalGeneration(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { Type: { } type } member || Owner(context) is not { } owner)
        {
            return null;
        }

        var hasKey = EntityOf(context, model, owner)?.Calls.LastOrDefault(x => x.Name == "HasKey");
        var keyed = context.Code.MembersOf(owner).Where(x => x.Attributes.Any(a => a.Name == "Key")).Select(x => x.Name).ToList();
        var single = hasKey is not null ? hasKey.Selected is [var only] && only == member.Name
            : keyed.Count > 0 ? keyed is [var annotated] && annotated == member.Name
            : string.Equals(member.Name, "Id", StringComparison.OrdinalIgnoreCase) || string.Equals(member.Name, owner.Name + "Id", StringComparison.OrdinalIgnoreCase);
        return single && type.Name.TrimEnd('?') is "int" or "long" or "short" or "Guid" or "Int32" or "Int64" or "Int16" ? "Auto-generated" : "User supplied";
    }

    /// <summary>
    /// The values a configuration's arguments state: a literal's, or the literal a constant the code declares stands for
    /// (<c>HasMaxLength(OrderConstraints.DescriptionMaxLength)</c>). Anything else, such as a field its constructor sets, has
    /// no value without running the code; the first such argument read is <see cref="Unread"/>, so the claim is can't verify,
    /// <c>unreadable</c>, rather than the argument's text compared as a value.
    /// </summary>
    private sealed class Arguments(ClaimContext context)
    {
        private static readonly Regex Literal = new(@"^(@?""([^""\\]|\\.|"""")*""|-?\d+(\.\d+)?[mMdDfFlLuU]*|true|false)$", RegexOptions.CultureInvariant);

        public (string Written, Span At)? Unread { get; private set; }

        // A lambda is an overload's builder (ToTable("Orders", t => ...)), not a value.
        public string? Of(FluentCall call, int index) =>
            call.Written.ElementAtOrDefault(index) is { } written && !written.Contains("=>")
                ? Resolve(written, Literal.IsMatch(written) ? call.Arguments[index] : null, call.At)
                : null;

        public string? Of(AttributeNode attribute, int index) =>
            attribute.Values.Where(x => x.Name is null).ElementAtOrDefault(index) is { } value ? Resolve(value.Text, value.Literal, attribute.At) : null;

        public string? Of(AttributeNode attribute, string name) =>
            attribute.Values.FirstOrDefault(x => x.Name == name) is { } value ? Resolve(value.Text, value.Literal, attribute.At) : null;

        public ClaimReading Unreadable((string Written, Span At) unread) =>
            ClaimReading.CantVerify("unreadable", $"The code states it as `{unread.Written}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", unread.At);

        private string? Resolve(string written, string? literal, Span at)
        {
            var value = literal ?? (TypeAt(at) is { } from ? context.Code.LiteralOf(written, from)?.Value : null);
            if (value is null)
            {
                Unread ??= (written, at);
            }

            return value;
        }

        /// <summary>The type whose code is at a place: the last declared before it in its file.</summary>
        private TypeNode? TypeAt(Span at) => context.Code.Types.Where(x => x.At.File == at.File && x.At.Line <= at.Line).MaxBy(x => x.At.Line);
    }

    /// <summary>
    /// A table with no schema of its own is in its DbContext's default schema: <c>modelBuilder.HasDefaultSchema("ordering")</c>
    /// in the context that maps the class. Without one the code doesn't state it, whatever the provider's default (decision 4).
    /// </summary>
    private static ClaimReading DefaultSchema(ClaimContext context, EfCoreModel model, TypeNode type, FluentCall? toTable, Arguments arguments)
    {
        var contexts = model.ContextsOf(type.Name);
        var stated = contexts.Where(x => x.DefaultSchema is not null).Select(x => (Call: x.DefaultSchema!, Value: arguments.Of(x.DefaultSchema!, 0))).ToList();
        if (arguments.Unread is { } unread)
        {
            return arguments.Unreadable(unread);
        }

        if (stated.Select(x => x.Value).Distinct(StringComparer.Ordinal).Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{stated.Count} DbContexts map it, with different default schemas.", stated[0].Call.At, stated.Select(x => x.Call.At).ToList());
        }

        // A context that maps it and states no default leaves its tables in the provider's.
        return stated.Count > 0 && stated.Count == contexts.Count
            ? ClaimReading.Stated(stated[0].Value!, stated[0].Call.At)
            : ClaimReading.Absent(toTable?.At ?? contexts.FirstOrDefault(x => x.DefaultSchema is null)?.At ?? type.At);
    }

    /// <summary>The type of the class the claim's attribute belongs to.</summary>
    private static TypeNode Owner(ClaimContext context) => context.TypeOf(context.Owner(context.Element)!)!;

    /// <summary>Where an entity is configured: its <c>IEntityTypeConfiguration&lt;T&gt;</c> class, else where its configuration starts.</summary>
    private static Span ConfigurationOf(EntityConfig entity) => entity.ConfigurationClasses.Count > 0 ? entity.ConfigurationClasses[0].At : entity.At;

    /// <summary>
    /// Whether C# writes an attribute's model type as a reference type, whose nullability only a nullable context states: the
    /// C# profile's <c>string</c>, <c>object</c> and <c>binary</c> (<c>byte[]</c>), as against an enum or its other types,
    /// which are value types. Null for any other type (a class, a collection, a house type-definition), whose kind isn't
    /// known without its code.
    /// </summary>
    private static bool? ReferenceType(IElement element) => element.TypeReference switch
    {
        { IsCollection: true } => null,
        { Element.SpecializationType: "Enum" } => false,
        { Element: { SpecializationType: "Type-Definition" } definition } => definition.Name switch
        {
            "string" or "object" or "binary" => true,
            "guid" or "int" or "long" or "short" or "byte" or "decimal" or "double" or "float" or "bool" or "char"
                or "datetime" or "datetimeoffset" or "date" or "time" => false,
            _ => null
        },
        _ => null
    };

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
    /// Whether EF Core maps the class: a DbContext in scope names it (a <c>DbSet</c>, <c>Entity&lt;T&gt;</c>, or applying its
    /// configuration, one by one or with the others in its assembly), else <c>modelBuilder.Entity&lt;T&gt;()</c> configures it
    /// elsewhere, else, by EF Core's convention, a mapped entity navigates to it. With none, the code's silence says it isn't
    /// mapped: a configuration nothing applies is dead code.
    /// </summary>
    private static ClaimReading Persisted(ClaimContext context, EfCoreModel model)
    {
        var type = context.Type!;
        if (model.Contexts.Count == 0)
        {
            return ClaimReading.CantVerify("notFound", "No DbContext is in scope, so what EF Core maps can't be read.", type.At);
        }

        var mapped = model.Contexts.Values.SelectMany(x => Mapped(context, model, x)).ToList();
        if (mapped.FirstOrDefault(x => x.Entity == type.Name) is { Entity: not null } named)
        {
            return ClaimReading.Stated("true", named.At);
        }

        if (EntityOf(context, model, type) is { } entity && !model.Configurations.ContainsValue(type.Name))
        {
            return ClaimReading.Stated("true", entity.At);
        }

        // EF Core maps an entity a mapped one navigates to, through a collection or not, and those it navigates to in turn.
        var seen = mapped.Select(x => x.Entity).ToHashSet(StringComparer.Ordinal);
        var queue = new Queue<TypeNode>(context.Code.Types.Where(x => seen.Contains(x.Name)));
        while (queue.TryDequeue(out var from))
        {
            foreach (var member in context.Code.MembersOf(from).Where(x => x.Kind == "property" && x.Type is not null))
            {
                foreach (var reached in Referenced(member.Type!).Select(x => context.Code.TypeFor(x, from)).OfType<TypeNode>())
                {
                    if (reached == type)
                    {
                        return ClaimReading.Defaulted("true", member.At);
                    }

                    if (seen.Add(reached.Name))
                    {
                        queue.Enqueue(reached);
                    }
                }
            }
        }

        // [Table] says the class is a table's entity, but only a DbContext's registration maps it (user decision, 2026-11-24:
        // the diagram shows a persisted class, so one EF never maps is an error). CrmBus's EF6 lookup classes (ActionType)
        // carry it, and nothing names them.
        if (type.Attributes.FirstOrDefault(x => x.Name == "Table") is { } table)
        {
            var unmapped = model.Contexts.Count == 1
                ? $"`{model.Contexts.Keys.Single()}` doesn't map it"
                : $"none of the DbContexts ({string.Join(", ", model.Contexts.Keys.Order(StringComparer.Ordinal).Select(x => $"`{x}`"))}) maps it";
            return ClaimReading.Compared("false", table.At, context.Expected == "false",
                $"`{type.Name}` has `[Table]`, but {unmapped}: no `DbSet` or `IDbSet` of it, no `Entity<{type.Name}>()` or configuration applied for it, "
                + "and no mapped entity navigates to it, so EF never reads or writes its table. Register it in the DbContext, or remove the class's Table stereotype if it isn't persisted.");
        }

        return ClaimReading.Absent(model.Contexts.Count == 1 ? model.Contexts.Values.Single().At : type.At, "false");

        static IEnumerable<TypeRefNode> Referenced(TypeRefNode reference) => reference.Arguments.SelectMany(Referenced).Prepend(reference);
    }

    /// <summary>
    /// The entities a DbContext maps, each where it does: by its own <c>DbSet</c> or <c>Entity&lt;T&gt;</c>, a configuration it
    /// applies, or those in the assembly it applies, the project of the type it names (<c>typeof(T).Assembly</c>), else its
    /// own. An assembly named by a type that isn't in scope can't be placed, so every configuration counts.
    /// </summary>
    private static IEnumerable<(string Entity, Span At)> Mapped(ClaimContext context, EfCoreModel model, ContextConfig dbContext)
    {
        foreach (var (name, at) in dbContext.MappedAt)
        {
            if (dbContext.Entities.Contains(name))
            {
                yield return (name, at);
            }

            if (dbContext.Configurations.Contains(name) && model.Configurations.GetValueOrDefault(name) is { } configured)
            {
                yield return (configured, at);
            }
        }

        if (dbContext.Assembly is not { } assembly)
        {
            yield break;
        }

        var marker = assembly.Of is { } of ? context.Code.Types.FirstOrDefault(x => x.Name == of)?.At.File : dbContext.At.File;
        foreach (var (configuration, entity) in model.Configurations)
        {
            if (marker is null || context.Code.Types.FirstOrDefault(x => x.Name == configuration) is { } declared && ProjectOf(declared.At.File) == ProjectOf(marker))
            {
                yield return (entity, assembly.At);
            }
        }

        string? ProjectOf(string file) => context.Code.Files.FirstOrDefault(x => x.Path == file)?.Project;
    }

    /// <summary>
    /// HasKey, else [Key] on its properties, else [PrimaryKey] on the type. A key is a set: EF orders a composite key by
    /// HasKey, the model by its attributes. A keyless entity says so, <c>HasNoKey()</c> or <c>[Keyless]</c>: its key is
    /// <c>none</c>, what the model states for a Table or View with no Primary Key. A key wins over a HasNoKey beside it.
    /// </summary>
    private static ClaimReading Key(ClaimContext context, TypeNode type, EfCoreModel model)
    {
        var config = EntityOf(context, model, type);
        var hasKey = config?.Calls.LastOrDefault(x => x.Name == "HasKey");
        var keyed = context.Code.MembersOf(type).Where(x => x.Attributes.Any(a => a.Name == "Key")).ToList();
        var primaryKey = type.Attributes.FirstOrDefault(x => x.Name == "PrimaryKey");
        var (columns, at) = hasKey is not null ? (hasKey.Selected, hasKey.At)
            : keyed.Count > 0 ? (keyed.Select(x => x.Name).ToList(), keyed[0].At)
            : primaryKey is not null ? (primaryKey.Arguments.Select(x => x.Trim('"')).ToList(), primaryKey.At)
            : ((IReadOnlyList<string>)[], type.At);
        // A context can declare both, one per condition (a key for an in-memory test provider, HasNoKey for the database):
        // the one the model states is the code's.
        var keyless = config?.Calls.LastOrDefault(x => x.Name == "HasNoKey")?.At ?? type.Attributes.FirstOrDefault(x => x.Name == "Keyless")?.At;
        if (keyless is not null && (columns.Count == 0 || context.Expected == "none"))
        {
            return ClaimReading.Stated("none", keyless);
        }

        if (columns.Count > 0)
        {
            return ClaimReading.Stated(string.Join(", ", columns), at);
        }

        // The Domain designer always marks a key, so unlike a Table Name it can't leave a conventional one unstated: with
        // nothing explicit, the code's key is EF Core's, a public property named Id or <Entity>Id, its own or inherited.
        // Without one the entity has no key EF Core accepts, so there is nothing to read, not a key to compare.
        var conventional = context.Code.MembersOf(type).FirstOrDefault(x => x.Kind == "property" && x.Access == "public"
            && (string.Equals(x.Name, "Id", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, $"{type.Name}Id", StringComparison.OrdinalIgnoreCase)));
        return conventional is not null ? ClaimReading.Defaulted(conventional.Name, conventional.At)
            // A keyless entity the code doesn't declare keyless: EF Core would refuse it.
            : context.Expected == "none" ? ClaimReading.Absent(type.At)
            : ClaimReading.CantVerify("notFound", $"`{type.Name}` has no `HasKey`, `[Key]` or `[PrimaryKey]`, and no public `Id` or `{type.Name}Id` property EF Core would take as its key.", type.At);
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
        var named = chains.Where(x => name is not null && (x[0].Arguments.ElementAtOrDefault(1) == name || x.Any(c => c.Name == "HasDatabaseName" && c.Arguments is [var given] && given == name))).ToList();
        var matches = named.Count > 0 ? named : chains.Where(x => columns is not null && string.Join(", ", x[0].Selected) == columns).ToList();
        if (matches.Count == 0)
        {
            return ClaimReading.Absent(type.At);
        }

        // Declared twice, in its configuration and again in OnModelCreating, say: which one EF Core keeps is its merge's.
        if (matches.Count > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{matches.Count} HasIndex calls declare it.", matches[0][0].At, matches.Select(x => x[0].At).ToList());
        }

        var chain = matches[0];

        switch (context.Kind)
        {
            case "index.columns":
                return ClaimReading.Stated(string.Join(", ", chain[0].Selected), chain[0].At);
            case "index.unique":
                var unique = chain.LastOrDefault(x => x.Name == "IsUnique");
                return unique is null
                    ? ClaimReading.Absent(chain[0].At, "false")
                    : ClaimReading.Stated(unique.Arguments is [] or ["true"] ? "true" : "false", unique.At);
            case "index.name":
                // HasDatabaseName, else the name a named index is given (HasIndex(e => e.A, "IX_A")), which is its database name.
                var databaseName = chain.LastOrDefault(x => x.Name is "HasDatabaseName" or "HasName");
                return Value(databaseName?.Arguments.FirstOrDefault() ?? chain[0].Arguments.ElementAtOrDefault(1), databaseName?.At ?? chain[0].At);
            default:
                var filter = chain.LastOrDefault(x => x.Name == "HasFilter");
                return Value(filter?.Arguments.FirstOrDefault(), filter?.At ?? chain[0].At);
        }
    }

    /// <summary>
    /// The foreign key the code gives the relationship an association is: the <c>HasForeignKey</c> of the chain that
    /// configures it from either side (the dependent's <c>HasOne(o =&gt; o.Buyer)</c>, the principal's
    /// <c>WithOne(o =&gt; o.Buyer)</c>, or a <c>HasOne&lt;Buyer&gt;()</c> with no navigation), else <c>[ForeignKey]</c> on the
    /// dependent's properties, else the properties EF Core discovers by convention (<see cref="ConventionalForeignKey"/>). Two
    /// chains that configure it are ambiguous: EF Core merges them, and which wins is its merge's.
    /// </summary>
    private static ClaimReading? RelationshipForeignKey(ClaimContext context, EfCoreModel model)
    {
        if (context.ExpectedValue is not Relational.ModelForeignKey key || context.TypeOf(key.Dependent) is not { } dependent)
        {
            return null;
        }

        // A chain is the relationship's by the side it starts on: the dependent's HasOne by its navigation to the principal, the
        // principal's HasMany or HasOne by its navigation back, its WithOne naming the dependent's when it names one. Another
        // relationship's inverse may share a name (several principals' bids), so the far end's name alone never decides.
        var principal = key.Principal is null ? null : context.TypeOf(key.Principal);
        static bool Starts(FluentCall first, string? navigation, string? toward) =>
            first.Name is "HasOne" or "HasMany"
            && (navigation is not null ? first.Selected is [var named] && named == navigation : first.Selected.Count == 0 && first.TypeArguments.Any(x => x.Name == toward));
        bool FromDependent(IReadOnlyList<FluentCall> chain) => Starts(chain[0], key.DependentNavigation, principal?.Name);
        bool FromPrincipal(IReadOnlyList<FluentCall> chain)
        {
            var with = chain.FirstOrDefault(c => c.Name is "WithOne" or "WithMany");
            return Starts(chain[0], key.PrincipalNavigation, dependent.Name) && (with is null || with.Selected.Count == 0 || with.Selected[0] == key.DependentNavigation);
        }
        var dependentConfig = EntityOf(context, model, dependent);
        var principalConfig = principal is null ? null : EntityOf(context, model, principal);
        var chains = (dependentConfig?.Chains.Where(FromDependent) ?? []).Concat(principalConfig?.Chains.Where(FromPrincipal) ?? []).Distinct().ToList();
        var stated = chains.Select(chain => chain.LastOrDefault(c => c.Name == "HasForeignKey")).OfType<FluentCall>().ToList();
        if (stated.Count > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{stated.Count} chains configure the relationship's foreign key.", stated[0].At, stated.Select(x => x.At).ToList());
        }

        // Each modelled attribute as the code holds it: its located member, a shadow property or a backing field included.
        var located = key.Attributes.Select(x => context.MemberOf(x)).ToList();
        string Modelled(string code) => located.FindIndex(x => x?.Name == code) is var index and >= 0 ? key.Columns[index] : code;
        if (stated is [var only])
        {
            return ClaimReading.Stated(string.Join(", ", only.Selected.Select(Modelled)), only.At).Through(Selecting(chains.First(x => x.Contains(only))));
        }

        // The one chain that configures the relationship is read with a key stated elsewhere too.
        var selecting = chains is [var chain] ? Selecting(chain) : [];

        // [ForeignKey("Buyer")] on the key property, or [ForeignKey("BuyerId")] on the navigation.
        var members = context.Code.MembersOf(dependent).ToList();
        string? Named(MemberNode member) => member.Attributes.FirstOrDefault(a => a.Name == "ForeignKey" && a.Arguments.Count == 1)?.Arguments[0];
        if (members.Where(x => Named(x) is { } navigation && key.Navigations.Contains(navigation)).ToList() is { Count: > 0 } annotated)
        {
            return ClaimReading.Stated(string.Join(", ", annotated.Select(x => x.Name)), annotated[0].At).Through(selecting);
        }

        if (members.FirstOrDefault(x => key.Navigations.Contains(x.Name) && Named(x) is not null) is { } navigationProperty)
        {
            return ClaimReading.Stated(string.Join(", ", Named(navigationProperty)!.Split(',', StringSplitOptions.TrimEntries)), navigationProperty.At).Through(selecting);
        }

        // Every modelled column is one EF Core discovers for one of the relationship's navigations. With none modelled (a
        // probe), what EF Core discovers is its convention, which the model needn't state.
        var conventional = located.Count > 0 && located.All(member => member is not null
                                                 && key.Navigations.Any(navigation => ConventionalForeignKey(context, dependent, member, model, navigation) is not null));
        return conventional
            ? ClaimReading.Defaulted(string.Join(", ", key.Columns), located[0]!.At).Through(selecting)
            : ClaimReading.Absent(dependent.At);
    }

    /// <summary>The calls of a chain that pick out its relationship: its <c>HasOne</c> or <c>HasMany</c>, and its <c>WithOne</c> or <c>WithMany</c>.</summary>
    private static IReadOnlyList<Span> Selecting(IReadOnlyList<FluentCall> chain) =>
        chain.Take(1).Concat(chain.Where(x => x.Name is "WithOne" or "WithMany").Take(1)).Select(x => x.At).ToList();

    /// <summary>
    /// How many owners each target has, for a relationship's end that no navigation states: the chain that configures the
    /// navigation (<c>HasMany(b =&gt; b.PaymentMethods).WithOne().IsRequired()</c>), else EF Core's convention for the
    /// navigation alone. An owned type has one owner; <c>WithMany</c>, many; <c>WithOne</c> or a collection alone, one,
    /// required when the chain says <c>IsRequired()</c> or the dependent's foreign key can't be null, else at most one. A
    /// reference alone is many-to-one. Like a key, the Domain designer always states an end's multiplicity, so it can't
    /// leave a conventional one unstated.
    /// </summary>
    private static ClaimReading? Multiplicity(ClaimContext context, EfCoreModel model)
    {
        if (context.ExpectedValue is not Relational.ModelMultiplicity relationship || context.MemberOf(relationship.Navigation) is not { } navigation
            || context.TypeOf(relationship.Owner) is not { } owner)
        {
            return null;
        }

        var target = context.TypeOf(relationship.Target);

        // The relationship's chain from the owner, which starts with its navigation, or from the target, whose With names the
        // navigation and whose own navigation (or type argument) leads back to the owner: several owners' inverses may share
        // a name, as a database-first context's often do.
        bool LeadsToOwner(FluentCall call) => call.Selected is [var named]
            ? target is not null && context.Code.MembersOf(target).Any(x => x.Name == named && x.Type is { } held && (held.Name == owner.Name || held.Arguments.Any(a => a.Name == owner.Name)))
            : call.TypeArguments.Any(x => x.Name == owner.Name);
        var fromOwner = EntityOf(context, model, owner)?.Chains
            .Where(x => x[0].Name is "HasOne" or "HasMany" or "OwnsOne" or "OwnsMany" && x[0].Selected is [var named] && named == navigation.Name) ?? [];
        var fromTarget = target is null ? [] : EntityOf(context, model, target)?.Chains
            .Where(x => x[0].Name is "HasOne" or "HasMany" && LeadsToOwner(x[0]) && x.Any(c => c.Name is "WithOne" or "WithMany" && c.Selected is [var named] && named == navigation.Name)) ?? [];
        var chains = fromOwner.Select(x => (Chain: x, FromOwner: true)).Concat(fromTarget.Select(x => (Chain: x, FromOwner: false))).ToList();
        if (chains.Count > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{chains.Count} chains configure `{owner.Name}.{navigation.Name}`.", chains[0].Chain[0].At, chains.Select(x => x.Chain[0].At).ToList());
        }

        if (chains is [var (chain, isOwners)])
        {
            var first = chain[0];
            var with = chain.FirstOrDefault(x => x.Name is "WithOne" or "WithMany");
            if (first.Name is "OwnsOne" or "OwnsMany")
            {
                return ClaimReading.Stated("1", first.At);
            }

            // From the owner its With is the other end; from the target, its Has is. Read through the calls that pick out the
            // relationship, whatever states the multiplicity.
            var selecting = Selecting(chain);
            var many = isOwners ? with?.Name == "WithMany" || with is null && first.Name == "HasOne" : first.Name == "HasMany";
            if (many)
            {
                return ClaimReading.Stated("*", isOwners ? (with ?? first).At : first.At).Through(selecting);
            }

            if (chain.LastOrDefault(x => x.Name == "IsRequired") is { } required)
            {
                return ClaimReading.Stated(required.Arguments is ["false"] ? "0..1" : "1", required.At).Through(selecting);
            }

            var stated = chain.LastOrDefault(x => x.Name == "HasForeignKey")?.Selected is [var key] ? key : null;
            return (Owners(context, model, owner, target, stated) ?? ClaimReading.Stated("0..1", (with ?? first).At)).Through(selecting);
        }

        // The navigation alone: a collection is one-to-many, a reference many-to-one.
        var collection = navigation.Type is { } type && (type.IsArray || target is not null && type.Name != target.Name && type.Arguments.Any(x => x.Name == target.Name));
        return collection ? Owners(context, model, owner, target, null) ?? ClaimReading.Defaulted("0..1", navigation.At) : ClaimReading.Defaulted("*", navigation.At);
    }

    /// <summary>
    /// A one-to-many's owner end as the dependent's foreign key says: required when it can't be null. The key is the one the
    /// chain names, else EF Core's by convention (<c>BuyerId</c> on a payment method of a buyer): a property of the class, or
    /// a shadow property its configuration declares (<c>Property&lt;int&gt;("BuyerId")</c>), whose <c>IsRequired</c> or type
    /// says. Null when the dependent has none, a shadow property EF Core makes nullable.
    /// </summary>
    private static ClaimReading? Owners(ClaimContext context, EfCoreModel model, TypeNode owner, TypeNode? dependent, string? named) =>
        dependent is not null && KeyRequired(context, model, owner, dependent, named) is (var required, var at) ? ClaimReading.Stated(required ? "1" : "0..1", at) : null;

    /// <summary>Whether the dependent's foreign key to the owner can't be null, and where the code says so (<see cref="Owners"/>).</summary>
    private static (bool Required, Span At)? KeyRequired(ClaimContext context, EfCoreModel model, TypeNode owner, TypeNode dependent, string? named)
    {
        bool IsKey(string name) => named is not null ? name == named : string.Equals(name, owner.Name + "Id", StringComparison.OrdinalIgnoreCase);
        var config = EntityOf(context, model, dependent);
        var configured = config?.Properties.FirstOrDefault(x => IsKey(x.Key)).Value;
        if (configured?.LastOrDefault(x => x.Name == "IsRequired") is { } required)
        {
            return (required.Arguments is not ["false"], required.At);
        }

        // A key's columns are always required: a link table's foreign keys are its primary key.
        if (config?.Calls.FirstOrDefault(x => x.Name == "HasKey" && x.Selected.Any(IsKey)) is { } primaryKey)
        {
            return (true, primaryKey.At);
        }

        if (context.Code.MembersOf(dependent).FirstOrDefault(x => x.Kind == "property" && IsKey(x.Name)) is { Type: { } type } key)
        {
            return (type.Nullable == "false", key.At);
        }

        return configured?.FirstOrDefault(x => x.Name == "Property")?.TypeArguments is [var shadow]
            ? (!shadow.NullableMarked && (shadow.IsPredefinedValueType || shadow.Nullable == "false"), configured[0].At)
            : null;
    }

    /// <summary>
    /// Whether a reference navigation may be null, where its C# type can't say (outside a nullable context), as EF Core
    /// decides it: the relationship's <c>IsRequired</c>, else the navigation's foreign key, required when it can't be null.
    /// The key is the one the chain names (<c>HasForeignKey</c>), else <c>[ForeignKey]</c>, else EF Core's by convention: the
    /// navigation's or the principal's name and <c>Id</c>. A navigation whose own class holds no such key (a principal's to
    /// its dependent in a one-to-one) isn't read.
    /// </summary>
    private static ClaimReading? Navigation(ClaimContext context, EfCoreModel model, IAssociationEnd end)
    {
        if (context.Member is not { } navigation || navigation.Type?.Nullable is not (null or "unknown") || navigation.Type.IsArray
            || context.Code.UngovernedFiles.ContainsKey(navigation.At.File)
            || context.Owner(end) is not { } ownerElement || context.TypeOf(ownerElement) is not { } dependent
            || end.TypeReference?.Element is not IElement principalElement || context.TypeOf(principalElement) is not { } principal
            || navigation.Type.Name != principal.Name)
        {
            return null;
        }

        // The relationship's chain: the dependent's HasOne by this navigation, or the principal's Has... whose WithOne names it
        // and whose own navigation (or type argument) leads to the dependent.
        bool LeadsToDependent(FluentCall call) => call.Selected is [var named]
            ? context.Code.MembersOf(principal).Any(x => x.Name == named && x.Type is { } held && (held.Name == dependent.Name || held.Arguments.Any(a => a.Name == dependent.Name)))
            : call.TypeArguments.Any(x => x.Name == dependent.Name);
        var chains = (EntityOf(context, model, dependent)?.Chains.Where(x => x[0].Name == "HasOne" && x[0].Selected is [var named] && named == navigation.Name) ?? [])
            .Concat(EntityOf(context, model, principal)?.Chains.Where(x => x[0].Name is "HasMany" or "HasOne" && LeadsToDependent(x[0])
                && x.Any(c => c.Name == "WithOne" && c.Selected is [var named] && named == navigation.Name)) ?? [])
            .ToList();
        if (chains.Count > 1)
        {
            // Configured from both ends, as CrmBus's EF6 context restates HasMany(e => e.Containers).WithRequired(e => e.Account)
            // with HasRequired(e => e.Account) from Container: chains that state the same are one statement.
            var states = chains.Select(x => x.LastOrDefault(c => c.Name == "IsRequired")).ToList();
            return states.All(x => x is not null) && states.Select(x => x!.Arguments is ["false"]).Distinct().Count() == 1
                ? ClaimReading.Stated(states[0]!.Arguments is ["false"] ? "true" : "false", states[0]!.At)
                : ClaimReading.CantVerify("ambiguous", $"{chains.Count} chains configure `{dependent.Name}.{navigation.Name}`.", chains[0][0].At, chains.Select(x => x[0].At).ToList());
        }

        var chain = chains.FirstOrDefault();
        if (chain?.LastOrDefault(x => x.Name == "IsRequired") is { } required)
        {
            return ClaimReading.Stated(required.Arguments is ["false"] ? "true" : "false", required.At);
        }

        var members = context.Code.MembersOf(dependent).ToList();
        var key = chain?.LastOrDefault(x => x.Name == "HasForeignKey")?.Selected is [var stated] ? stated
            : navigation.Attributes.FirstOrDefault(x => x.Name == "ForeignKey")?.Arguments is [var annotated] ? annotated.Trim('"')
            : members.FirstOrDefault(x => x.Attributes.Any(a => a.Name == "ForeignKey" && a.Arguments is [var on] && on.Trim('"') == navigation.Name))?.Name
              ?? members.FirstOrDefault(x => x.Kind == "property" && (string.Equals(x.Name, navigation.Name + "Id", StringComparison.OrdinalIgnoreCase)
                                                                     || string.Equals(x.Name, principal.Name + "Id", StringComparison.OrdinalIgnoreCase)))?.Name;
        return key is not null && KeyRequired(context, model, principal, dependent, key) is (var isRequired, var at)
            ? ClaimReading.Stated(isRequired ? "false" : "true", at)
            : null;
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
    /// optional (decision 13: an extension adds a rule to another's kind). A model that says required is then contradicted by
    /// the missing configuration, and the finding says so rather than that the type is nullable, which it can't say
    /// (user decision, 2026-11-24: "The Model says its required, so EF should say its required"). Anywhere else the
    /// engine's own rule reads it, and an association end is its navigation's relationship (<see cref="Navigation"/>).
    /// </summary>
    private static ClaimReading? Required(ClaimContext context, EfCoreModel model)
    {
        if (context.Element is IAssociationEnd end)
        {
            return Navigation(context, model, end);
        }

        var member = context.Member;
        if (member is not null && (member.Type?.Nullable is not (null or "unknown") || context.Code.UngovernedFiles.ContainsKey(member.At.File))
            || context.Owner(context.Element) is not { } ownerElement || context.TypeOf(ownerElement) is not { } type
            || EntityOf(context, model, type) is not { } entity)
        {
            return null;
        }

        // A property the code lacks (its member.exists is that finding) needs its configuration to make it required only where
        // its C# type couldn't: a reference type, in a file outside a nullable context. Anywhere else its type will say it.
        if (member is null && (ReferenceType(context.Element) is not true || context.Code.UngovernedFiles.ContainsKey(type.At.File)
                               || CSharp.CSharpSyntax.Of(context.Code)?.InNullableContext(type.At.File) is not false))
        {
            return null;
        }

        var name = member?.Name ?? context.Element.Name;
        if (entity.Properties.GetValueOrDefault(name)?.LastOrDefault(x => x.Name == "IsRequired") is { } required)
        {
            return ClaimReading.Stated(required.Arguments is ["false"] ? "true" : "false", required.At);
        }

        // A key's columns are always required.
        if (entity.Calls.FirstOrDefault(x => x.Name == "HasKey" && x.Selected.Contains(name)) is { } key)
        {
            return ClaimReading.Stated("false", key.At);
        }

        if (member?.Attributes.FirstOrDefault(x => x.Name == "Required") is { } annotation)
        {
            return ClaimReading.Stated("false", annotation.At);
        }

        // The model is checked against the configuration, which is what would state it, so a finding is against the entity's
        // configuration class (else where it is configured); agreeing, the claim stays linked to the property. A property the
        // code lacks that the model leaves optional needs nothing configured, so it waits on the property.
        var same = context.Expected == "true";
        if (member is null && same)
        {
            return null;
        }

        var configuration = ConfigurationOf(entity);
        return ClaimReading.Compared("true", same ? member!.At : configuration, same,
            $"`{ownerElement.Name}.{context.Element.Name}` is required in the model, but EF Core maps its column as nullable: nothing configures "
            + $"`IsRequired()` or `[Required]` for it, and with nullable reference types off its `{member?.Type?.Text ?? context.Element.TypeReference?.Element?.Name}` type can't make it required. "
            + $"Add `.IsRequired()` to `{type.Name}`'s EF Core configuration, or `[Required]` to the property.");
    }

    /// <summary>
    /// What a property's fluent configuration states for a column kind, or a data annotation on the property. A decimal's
    /// precision and scale may be written in its column type, <c>decimal(18, 2)</c>.
    /// </summary>
    private static (string? Value, Span? At) Column(string kind, IReadOnlyList<FluentCall> calls, MemberNode? member, Arguments arguments)
    {
        FluentCall? Last(params string[] names) => calls.LastOrDefault(x => names.Contains(x.Name));
        AttributeNode? Annotation(params string[] names) => member?.Attributes.FirstOrDefault(x => names.Contains(x.Name));
        switch (kind)
        {
            case "column.name":
                return Last("HasColumnName") is { } name ? (arguments.Of(name, 0), name.At)
                    : Annotation("Column") is { Arguments.Count: > 0 } column ? (arguments.Of(column, 0), column.At) : (null, null);
            case "column.type":
            {
                if (Last("HasColumnType") is { } type)
                {
                    return (arguments.Of(type, 0), type.At);
                }

                if (Annotation("Column") is { } typed && typed.Named.ContainsKey("TypeName"))
                {
                    return (arguments.Of(typed, "TypeName"), typed.At);
                }

                // A text column's store type stated by its facets: HasMaxLength(1).IsUnicode(false).IsFixedLength() is char(1).
                var fixedLength = Last("IsFixedLength") is { Arguments: [] or ["true"] };
                var ansi = Last("IsUnicode") is { Arguments: ["false"] };
                return (fixedLength || ansi) && Last("HasMaxLength") is { Arguments: [_] } maxLength && arguments.Of(maxLength, 0) is { } length
                    ? ($"{(ansi ? "" : "n")}{(fixedLength ? "char" : "varchar")}({length})", maxLength.At)
                    : (null, null);
            }
            case "column.textType":
            {
                // The model's SQL Data Type options: a fixed-length char column is the nearest, VARCHAR or NVARCHAR.
                var stated = Last("HasColumnType") is { } type ? (arguments.Of(type, 0), type.At)
                    : Annotation("Column") is { } typed && typed.Named.ContainsKey("TypeName") ? (arguments.Of(typed, "TypeName"), typed.At)
                    : ((string?)null, (Span?)null);
                if (TextType.Match(stated.Item1 ?? "") is { Success: true } match)
                {
                    return (match.Groups[1].Value.ToLowerInvariant() switch
                    {
                        "varchar" or "char" => "VARCHAR",
                        "nvarchar" or "nchar" => "NVARCHAR",
                        var other => other.ToUpperInvariant()
                    }, stated.Item2);
                }

                return Last("IsUnicode") is { } unicode ? (unicode.Arguments is ["false"] ? "VARCHAR" : "NVARCHAR", unicode.At) : (null, null);
            }
            case "column.maxLength":
                return Last("HasMaxLength") is { } max ? (arguments.Of(max, 0), max.At)
                    : Annotation("MaxLength", "StringLength") is { Arguments.Count: > 0 } annotation ? (arguments.Of(annotation, 0), annotation.At)
                    : Last("HasColumnType") is { } sized && TextType.Match(arguments.Of(sized, 0) ?? "") is { Groups: [_, _, { Success: true } size] } ? (size.Value, sized.At)
                    : (null, null);
            case "column.default":
                // A SQL default is the string's content; a value default is the C# literal as written.
                return Last("HasDefaultValueSql", "HasDefaultValue") is { } value
                    ? (value.Name == "HasDefaultValueSql" ? value.Arguments.FirstOrDefault() : value.Written.FirstOrDefault(), value.At)
                    : (null, null);
            case "column.defaultSql":
                return Last("HasDefaultValueSql", "HasDefaultValue") is { } given ? (given.Name == "HasDefaultValueSql" ? "true" : "false", given.At) : (null, null);
            case "key.generated":
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
                    return (arguments.Of(precision, index), precision.At);
                }

                return Last("HasColumnType") is { } columnType && DecimalType.Match(arguments.Of(columnType, 0) ?? "") is { Success: true } match
                    ? (match.Groups[index + 1].Value, columnType.At)
                    : (null, null);
            }
        }
    }
}
