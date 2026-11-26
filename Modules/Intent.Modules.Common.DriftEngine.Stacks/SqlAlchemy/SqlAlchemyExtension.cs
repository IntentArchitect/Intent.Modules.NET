using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Interactions;
using Intent.Modules.Common.DriftEngine.Stacks.Python;

namespace Intent.Modules.Common.DriftEngine.Stacks.SqlAlchemy;

/// <summary>
/// How SQLAlchemy's declarative mapping states the relational claims (<see cref="Relational.RelationalExtension"/>), as
/// <c>intent.efcore</c> and <c>intent.jpa</c> read theirs: a class with a <c>__tablename__</c> or a <c>DeclarativeBase</c> is
/// persisted, its <c>mapped_column(...)</c>s its columns, its <c>relationship(...)</c>s its navigations, a <c>ForeignKey</c> its
/// relationship's key. An attribute's type is the <c>T</c> of its <c>Mapped[T]</c>, nullable when <c>T</c> is a union with
/// <c>None</c>. A service's data access through a <c>Session</c> (<c>select(Order)</c>, <c>session.get(Order, id)</c>) queries the
/// entity. It answers only for a declarative class.
/// </summary>
public sealed class SqlAlchemyExtension : IDriftExtension
{
    public string Id => "intent.sqlalchemy";

    public IEnumerable<ICodeReader> Readers { get; } = [new SqlAlchemyReader()];

    public IEnumerable<IMemberRefiner> Refiners { get; } = [new MappedTypes()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new SqlAlchemyClaims(), new SessionAccess()];
}

/// <summary>A <c>mapped_column(...)</c>, a classic <c>Column(...)</c>, or a bare <c>Mapped[T]</c> annotation, of a declarative class.</summary>
internal sealed class SqlColumn(MemberNode member, CallNode? call)
{
    public MemberNode Member { get; } = member;

    public CallNode? Call { get; } = call;

    public string Name => Member.Name;

    public Span At => Call?.At ?? Member.At;

    private IEnumerable<ValueNode> Positional => Call?.Arguments.Where(x => x.Name is null) ?? [];

    public string? Flag(string name) => Call?.Arguments.FirstOrDefault(x => x.Name == name)?.Literal?.Value;

    public bool PrimaryKey => Flag("primary_key") == "true";

    public bool Unique => Flag("unique") == "true";

    public bool Index => Flag("index") == "true";

    /// <summary>The <c>ForeignKey("table.column")</c> it is given, if any.</summary>
    public string? ForeignKey => Positional.SelectMany(x => x.Creations).FirstOrDefault(x => x.Type.Name == "ForeignKey")?.Arguments.FirstOrDefault()?.Literal?.Value;

    /// <summary>
    /// The column type it is given: <c>String(100)</c>, <c>Text</c>, <c>UTCDateTime()</c>, or what a helper it calls returns
    /// (<c>_enum_column(Status)</c> returning <c>Enum</c>). Null when the annotation decides it.
    /// </summary>
    public (string Name, ValueNode Value, CreationNode? Creation)? ColumnType
    {
        get
        {
            foreach (var argument in Positional)
            {
                if (argument.Creations.FirstOrDefault() is { } creation)
                {
                    if (creation.Type.Name == "ForeignKey")
                    {
                        continue;
                    }

                    return (creation.Type.Name, argument, creation);
                }

                if (argument.Call is { } helper)
                {
                    return (helper.Target?.Member.Type?.Name ?? helper.Name, argument, null);
                }

                if (argument.Literal is null && argument.Text.Length > 0 && char.IsUpper(argument.Text.Split('.')[^1][0]) && !argument.Text.Contains('('))
                {
                    return (argument.Text.Split('.')[^1], argument, null);
                }
            }

            return null;
        }
    }
}

/// <summary>A declarative class: its table, columns and relationships.</summary>
internal sealed record SqlEntity(TypeNode Type, MemberNode? TableName, List<SqlColumn> Columns, List<(MemberNode Member, CallNode Call)> Relationships)
{
    public string? Table => TableName?.Value?.Literal?.Value;

    public SqlColumn? Column(string name) => Columns.FirstOrDefault(x => x.Name == name);
}

/// <summary>The SQLAlchemy declarative mapping the code states, read once per run.</summary>
internal sealed class SqlAlchemySite
{
    public Dictionary<TypeNode, SqlEntity> Entities { get; } = [];

    public static SqlAlchemySite? Of(CodeGraph graph) => graph.Get<SqlAlchemySite>();

    public SqlEntity? EntityOf(MemberNode? member) => member is null ? null : Entities.Values.FirstOrDefault(x => x.Type.Members.Contains(member));

    internal static SqlAlchemySite? Read(CodeGraph graph)
    {
        // A file that imports sqlalchemy: another library's mapped_column isn't SQLAlchemy's.
        var files = graph.Files.Where(x => x.Imports.Any(i => i.Target == "sqlalchemy" || i.Target.StartsWith("sqlalchemy.", StringComparison.Ordinal))).Select(x => x.Path).ToHashSet(StringComparer.Ordinal);
        if (files.Count == 0)
        {
            return null;
        }

        var site = new SqlAlchemySite();
        foreach (var type in graph.Types.Where(x => x.Kind == "class" && x.Files.Any(files.Contains)))
        {
            var tableName = type.Members.FirstOrDefault(x => x.Name == "__tablename__");
            var supertypes = graph.Supertypes(type);
            var declarative = supertypes.Contains("DeclarativeBase") || supertypes.Contains("DeclarativeBaseNoMeta");
            // The declarative base itself, and an abstract or mixin class, has no table of its own.
            if (tableName is null && (!declarative || type.IsAbstract || type.BaseTypes.Any(x => x.Name is "DeclarativeBase" or "DeclarativeBaseNoMeta")))
            {
                continue;
            }

            var columns = new List<SqlColumn>();
            var relationships = new List<(MemberNode, CallNode)>();
            foreach (var member in type.Members.Where(x => x.Kind == "field" && !(x.Name.StartsWith("__", StringComparison.Ordinal) && x.Name.EndsWith("__", StringComparison.Ordinal))))
            {
                var call = member.Value?.Call;
                if (call?.Name == "relationship")
                {
                    relationships.Add((member, call));
                }
                else if (call?.Name is "mapped_column" or "Column")
                {
                    columns.Add(new SqlColumn(member, call));
                }
                else if (member.Type?.Name == "Mapped" && member.Value is null)
                {
                    columns.Add(new SqlColumn(member, null));
                }
            }

            site.Entities[type] = new SqlEntity(type, tableName, columns, relationships);
        }

        return site.Entities.Count == 0 ? null : site;
    }
}

internal sealed class SqlAlchemyReader : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        if (SqlAlchemySite.Read(graph) is { } site)
        {
            graph.Set(site);
        }
    }
}

/// <summary>
/// A declarative attribute is the <c>T</c> of its <c>Mapped[T]</c>, as SQLAlchemy reads it: nullable when <c>T</c> is a union
/// with <c>None</c> or <c>Optional</c>, unless its <c>mapped_column(nullable=...)</c> says otherwise.
/// </summary>
internal sealed class MappedTypes : IMemberRefiner
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.type", "member.nullable"];

    public MemberNode? Refine(ClaimContext context)
    {
        var member = context.Member;
        if (member?.Type is not { Name: "Mapped", Arguments: [var inner] } || SqlAlchemySite.Of(context.Code)?.EntityOf(member) is not { } entity)
        {
            return null;
        }

        var stated = entity.Columns.FirstOrDefault(x => x.Member == member)?.Flag("nullable");
        var type = new TypeRefNode
        {
            Text = inner.Text,
            Name = inner.Name,
            QualifiedName = inner.QualifiedName,
            Arguments = inner.Arguments,
            Union = inner.Union,
            IsArray = inner.IsArray,
            Literal = inner.Literal,
            Language = inner.Language,
            Nullable = stated is "true" or "false" ? stated : PythonTypes.Nullability(inner)
        };
        return member with { Type = type };
    }
}

/// <summary>
/// The Domain's persistence as a declarative class states it: that it is mapped, its table and key, each attribute's column,
/// SQL type, text type and length, its single-column indexes and unique constraints (<c>index=True</c>, <c>unique=True</c>),
/// and each relationship's foreign key and navigability.
/// </summary>
internal sealed class SqlAlchemyClaims : IClaimReader
{
    /// <summary>Column types SQLAlchemy maps an annotation to anyway: given without a length, they restate the default.</summary>
    private static readonly HashSet<string> Defaults =
        ["String", "Unicode", "Text", "UnicodeText", "Enum", "Integer", "BigInteger", "SmallInteger", "Boolean", "Float", "Numeric", "DateTime", "Date", "Time", "Uuid", "LargeBinary"];

    public IReadOnlyCollection<string> Kinds { get; } =
    [
        "entity.persisted", "table.name", "table.kind", "key", "index.columns", "index.unique",
        "column.mapped", "column.type", "column.textType", "column.maxLength", "relationship.foreignKey", "relationship.navigable"
    ];

    public ClaimReading? Read(ClaimContext context)
    {
        if (SqlAlchemySite.Of(context.Code) is not { } site)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "relationship.foreignKey" or "relationship.navigable":
                return Relationship(context, site);
            case "column.mapped" or "column.type" or "column.textType" or "column.maxLength":
                return Column(context, site);
        }

        if (context.Type is not { } type || !site.Entities.TryGetValue(type, out var entity))
        {
            return null;
        }

        switch (context.Kind)
        {
            case "entity.persisted":
                return ClaimReading.Stated("true", entity.TableName?.At ?? type.At);
            case "table.name":
                return entity.Table is { } table ? ClaimReading.Stated(table, entity.TableName!.At) : ClaimReading.Absent(type.At);
            case "table.kind":
                return entity.Table is null ? ClaimReading.Absent(type.At) : ClaimReading.Stated("Table", entity.TableName!.At);
            case "key":
            {
                var keys = entity.Columns.Where(x => x.PrimaryKey).ToList();
                return keys.Count == 0 ? ClaimReading.Absent(type.At) : ClaimReading.Stated(string.Join(", ", keys.Select(x => x.Name)), keys[0].At);
            }
            default:
            {
                // mapped_column(index=True) is a single-column index, unique=True a single-column unique constraint.
                var column = entity.Column(context.Claim.Discriminator ?? context.Expected ?? "");
                if (column is null || !(column.Index || column.Unique))
                {
                    // No such index, which index.columns reports; without one nothing is unique.
                    return context.Kind == "index.columns"
                        ? ClaimReading.Absent(column?.At ?? type.At)
                        : ClaimReading.CantVerify("unresolved", $"`{type.Name}` has no index on `{context.Claim.Discriminator ?? context.Expected}` to read its uniqueness from.", column?.At ?? type.At);
                }

                return context.Kind == "index.columns" ? ClaimReading.Stated(column.Name, column.At) : ClaimReading.Stated(column.Unique ? "true" : "false", column.At);
            }
        }
    }

    private static ClaimReading? Column(ClaimContext context, SqlAlchemySite site)
    {
        if (context.Member is not { } member || site.EntityOf(member) is not { } entity)
        {
            return null;
        }

        var column = entity.Columns.FirstOrDefault(x => x.Member == member);
        if (context.Kind == "column.mapped")
        {
            return column is null ? ClaimReading.Absent(member.At, "false") : ClaimReading.Stated("true", column.At);
        }

        if (column is null)
        {
            return null;
        }

        var type = column.ColumnType;
        switch (context.Kind)
        {
            case "column.type":
                if (type is not { } given)
                {
                    return string.IsNullOrEmpty(context.Expected) ? null : ClaimReading.Absent(column.At);
                }

                // Unset in the model: a type that only restates the annotation's mapping (a String the Text Constraints describe, an
                // Enum column for an enum attribute) isn't a column type of its own.
                return string.IsNullOrEmpty(context.Expected) && Defaults.Contains(given.Name)
                    ? ClaimReading.Defaulted(given.Name, given.Value.At)
                    : ClaimReading.Stated(given.Name, given.Value.At);
            case "column.textType":
                return type is { Name: "Text" or "UnicodeText" } text
                    ? ClaimReading.Stated("TEXT", text.Value.At)
                    : ClaimReading.Defaulted("DEFAULT", type?.Value.At ?? column.At);
            default:
            {
                var length = type is { Name: "String" or "Unicode" or "VARCHAR" or "NVARCHAR", Creation: { } creation }
                    ? creation.Arguments.FirstOrDefault(x => x.Name is null or "length")?.Literal?.Value
                    : null;
                return length is null ? ClaimReading.Absent(type?.Value.At ?? column.At) : ClaimReading.Stated(length, type!.Value.Value.At);
            }
        }
    }

    private static ClaimReading? Relationship(ClaimContext context, SqlAlchemySite site)
    {
        if (context.Element is not IAssociationEnd end || context.Owner(end) is not { } owner || end.TypeReference?.Element is not IElement other
            || context.TypeOf(owner) is not { } ownerType || context.TypeOf(other) is not { } otherType
            || !site.Entities.TryGetValue(ownerType, out var ownerEntity) || !site.Entities.TryGetValue(otherType, out var otherEntity))
        {
            return null;
        }

        if (context.Kind == "relationship.navigable")
        {
            return ownerEntity.Relationships.FirstOrDefault(x => x.Member.Name == end.Name) is { Call: { } call }
                ? ClaimReading.Stated("true", call.At)
                : ClaimReading.Absent(ownerType.At, "false");
        }

        // The foreign key is on whichever side references the other's table.
        foreach (var (dependent, principal) in new[] { (otherEntity, ownerEntity), (ownerEntity, otherEntity) })
        {
            var keys = dependent.Columns.Where(x => x.ForeignKey is { } key && principal.Table is { } table && key.Split('.')[0] == table).ToList();
            if (keys.Count > 0)
            {
                return ClaimReading.Stated(string.Join(", ", keys.Select(x => x.Name)), keys[0].At);
            }
        }

        return ClaimReading.Absent(otherType.At);
    }
}

/// <summary>
/// A service's data access through SQLAlchemy's <c>Session</c>, which names the entity as a value rather than through a
/// repository of it: <c>select(Order)</c>, <c>select(Order.total)</c>, <c>session.get(Order, id)</c>, <c>session.query(Order)</c>
/// query it, and an entity it loads that is then changed (<c>order.status = ...</c>, <c>setattr(order, ...)</c>) or merged is
/// updated. Read after the stock reading: only what that doesn't find is looked for.
/// </summary>
internal sealed class SessionAccess : IClaimReader
{
    private static readonly HashSet<string> Selects = ["select", "select_from", "query", "get", "get_one"];

    public IReadOnlyCollection<string> Kinds { get; } = ["intent.queries", "intent.updates"];

    public ClaimReading? Read(ClaimContext context)
    {
        var stock = context.ReadNext();
        if (stock is { From: "compared:same" or "found" } || SqlAlchemySite.Of(context.Code) is not { } site
            || context.Element is not IAssociationEnd { TypeReference.Element: IElement target } end || Reach.Source(context, end) is not { } source)
        {
            return stock;
        }

        var entity = target.SpecializationType == "Class" ? target : context.Owner(target) ?? target;
        if (!site.Entities.Keys.Any(x => x.Name == entity.Name))
        {
            return stock;
        }

        var reached = Reach.From(context, source, out var own);
        if (own == 0)
        {
            return stock;
        }

        Span? queried = null, changed = null;
        foreach (var (_, member) in reached)
        {
            queried ??= member.Calls.FirstOrDefault(x => Selects.Contains(x.Name) && x.Arguments.FirstOrDefault() is { } first && Names(first, entity.Name))?.At;
            changed ??= member.Calls.FirstOrDefault(x => x.Name == "merge" && x.Arguments.Any(a => a.Types.Any(t => t.Name == entity.Name)))?.At
                        ?? member.Body?.Assignments.FirstOrDefault(x => x.Receiver?.Types.Any(t => t.Name == entity.Name) == true)?.At;
        }

        var at = context.Kind == "intent.queries" ? queried : queried is not null ? changed : null;
        return at is { } found ? ClaimReading.Compared("present", found, true) : stock;
    }

    /// <summary>The value is the entity's class, or one of its columns (<c>Order.total</c>).</summary>
    private static bool Names(ValueNode value, string entity) =>
        value.Text == entity || value.Text.StartsWith(entity + ".", StringComparison.Ordinal) || value.Field?.Type.Name == entity;
}
