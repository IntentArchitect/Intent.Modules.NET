using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Jpa;

/// <summary>
/// How JPA code states the relational claims (<see cref="Relational.RelationalExtension"/>): its mapping annotations,
/// <c>@Table</c>, <c>@Id</c>, <c>@Column</c> and the rest. It adds no claims of its own, as EF Core's extension doesn't.
/// </summary>
public sealed class JpaExtension : IDriftExtension
{
    public string Id => "intent.jpa";

    public IEnumerable<ICodeReader> Readers { get; } = [new JpaDetector()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new JpaClaims()];
}

/// <summary>JPA is in the code: the types mapped with <c>@Entity</c>, <c>@Embeddable</c> or <c>@MappedSuperclass</c>.</summary>
internal sealed class JpaCode(IReadOnlySet<TypeNode> mapped)
{
    public IReadOnlySet<TypeNode> Mapped { get; } = mapped;
}

internal sealed class JpaDetector : ICodeReader
{
    private static readonly HashSet<string> Mappings = ["Entity", "Embeddable", "MappedSuperclass"];

    public void Read(CodeGraph graph)
    {
        // A mapping annotation in a file that imports JPA (a type of it, or all of it with jakarta.persistence.*): another
        // library's @Entity isn't JPA's.
        var files = graph.Files
            .Where(x => x.Imports.Any(i => $"{i.Target}.".StartsWith("jakarta.persistence.", StringComparison.Ordinal) || $"{i.Target}.".StartsWith("javax.persistence.", StringComparison.Ordinal)))
            .Select(x => x.Path)
            .ToHashSet(StringComparer.Ordinal);
        var mapped = graph.Types.Where(x => x.Attributes.Any(a => Mappings.Contains(a.Name)) && x.Files.Any(files.Contains)).ToHashSet();
        if (mapped.Count > 0)
        {
            graph.Set(new JpaCode(mapped));
        }
    }
}

internal sealed partial class JpaClaims : IClaimReader
{
    private static readonly HashSet<string> Relationships = ["ManyToOne", "OneToOne"];

    /// <summary>A text store type a <c>columnDefinition</c> names: <c>varchar(50)</c>, <c>nchar(1)</c>, <c>text</c>.</summary>
    private static readonly Regex TextType = new(@"^\s*(n?varchar|n?char|n?text)\b", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "entity.persisted", "table.name", "table.schema", "table.kind", "key", "index.columns", "index.unique", "index.filter", "index.name", "column.name", "column.type", "column.maxLength",
        "column.precision", "column.scale", "column.textType", "key.generated", "member.nullable", "relationship.multiplicity",
        "column.mapped", "column.owned", "column.version", "column.enumStorage", "relationship.navigable", "relationship.joinColumn", "repository.entity",
        "repository.implementation"
    };

    public ClaimReading? Read(ClaimContext context)
    {
        // No JPA in the code: the claim is another framework's to read, or can't be verified.
        if (context.Code.Get<JpaCode>() is not { } jpa)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "entity.persisted":
            {
                // @Entity maps a table, @Embeddable columns of the tables that embed it; JPA maps nothing else.
                var type = context.Type!;
                return jpa.Mapped.Contains(type) && type.Attributes.FirstOrDefault(x => x.Name is "Entity" or "Embeddable") is { } mapping
                    ? ClaimReading.Stated("true", mapping.At)
                    : ClaimReading.Absent(type.At, "false");
            }
            case "table.name" or "table.schema":
            {
                var type = context.Type!;
                var table = type.Attributes.FirstOrDefault(x => x.Name == "Table");
                return Value(table?.Named.GetValueOrDefault(context.Kind == "table.name" ? "name" : "schema"), table?.At ?? type.At);
            }
            case "table.kind":
            {
                // Hibernate maps a query (@Subselect) or a view-backed entity (@View) rather than a table; anything else is a table.
                var type = context.Type!;
                return type.Attributes.FirstOrDefault(x => x.Name is "Subselect" or "View") is { } view
                    ? ClaimReading.Stated("View", view.At)
                    : type.Attributes.FirstOrDefault(x => x.Name == "Table") is { } table
                        ? ClaimReading.Stated("Table", table.At)
                        : ClaimReading.Defaulted("Table", type.At);
            }
            case "key":
                return Key(context);
            case "index.columns" or "index.unique" or "index.filter" or "index.name":
                return Index(context);
            case "member.nullable":
                return Nullable(context, jpa);
            case "relationship.multiplicity":
                return Multiplicity(context, jpa);
            case "column.mapped":
                return context.Member is { } mapped ? Mapped(mapped) : null;
            case "column.owned":
                return context.Member is { } owned ? Owned(context, owned) : null;
            case "column.version":
                return context.Member is { } version ? Version(version) : null;
            case "column.enumStorage":
                return context.Member is { } stored ? EnumStorage(stored) : null;
            case "relationship.navigable":
                return Navigable(context);
            case "relationship.joinColumn":
                return JoinColumn(context);
            case "repository.entity":
                return RepositoryEntity(context);
            case "repository.implementation":
                return RepositoryImplementation(context);
            default:
                return Column(context.Kind, context.Member!);
        }
    }

    private static ClaimReading Value(string? value, Span at) => value is null ? ClaimReading.Absent(at) : ClaimReading.Stated(value, at);

    /// <summary>
    /// The <c>@Id</c> or <c>@EmbeddedId</c> fields, by the names of the attributes they are; a key field no attribute is
    /// keeps its own name, so it can't match.
    /// </summary>
    private static ClaimReading Key(ClaimContext context)
    {
        var type = context.Type!;
        var keys = context.Code.MembersOf(type).Where(x => x.Attributes.Any(a => a.Name is "Id" or "EmbeddedId")).ToList();
        if (keys.Count == 0)
        {
            return ClaimReading.Absent(type.At);
        }

        var names = Attributes(context.Element).Select(x => (Element: x, Member: context.MemberOf(x))).Where(x => x.Member is not null).ToList();
        return ClaimReading.Stated(string.Join(", ", keys.Select(k => names.FirstOrDefault(x => x.Member == k).Element?.Name ?? k.Name)), keys[0].At);
    }

    /// <summary>
    /// An index the model states on an attribute (its Index stereotype) or as an Index element, against the code's: a
    /// field's <c>@Column(unique = true)</c>, an <c>@Index</c> in <c>@Table(indexes)</c> or a <c>@UniqueConstraint</c> in
    /// <c>@Table(uniqueConstraints)</c>. Those list columns, mapped back to attributes by each field's
    /// <c>@Column(name)</c> or its name. The index is found by its name, else by its columns.
    /// </summary>
    private static ClaimReading Index(ClaimContext context)
    {
        var element = context.Element;
        // The Index stereotypes' index is a claim on the class itself; an Index element is in its class.
        var stated = context.ExpectedValue as Relational.ModelIndex;
        var owner = stated is null ? context.Owner(element)! : element;
        var type = context.TypeOf(owner) ?? context.Type!;
        var attributes = Attributes(owner).Select(x => (x.Name, Member: context.MemberOf(x))).Where(x => x.Member is not null).ToList();
        string AttributeOf(string column) =>
            attributes.FirstOrDefault(x => ColumnName(x.Member!) == column || string.Equals(x.Member!.Name, column, StringComparison.OrdinalIgnoreCase)).Name ?? column;

        var table = type.Attributes.FirstOrDefault(x => x.Name == "Table");
        var declared = new List<(string? Name, IReadOnlyList<string> Columns, bool Unique, Span At)>();
        foreach (var index in Nested(table, "indexes"))
        {
            var columns = (index.Named.GetValueOrDefault("columnList") ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => AttributeOf(x.Split(' ')[0])).ToList();
            declared.Add((index.Named.GetValueOrDefault("name"), columns, index.Named.GetValueOrDefault("unique") == "true", index.At));
        }

        foreach (var constraint in Nested(table, "uniqueConstraints"))
        {
            var names = constraint.Values.FirstOrDefault(x => x.Name == "columnNames") is { } list
                ? list.Items.Count > 0 ? list.Items.Select(x => x.Literal ?? x.Text) : [list.Literal ?? list.Text]
                : [];
            declared.Add((constraint.Named.GetValueOrDefault("name"), names.Select(AttributeOf).ToList(), true, constraint.At));
        }

        declared.AddRange(attributes
            .Select(x => (x.Name, Column: x.Member!.Attributes.FirstOrDefault(a => a.Name == "Column" && a.Named.GetValueOrDefault("unique") == "true")))
            .Where(x => x.Column is not null)
            .Select(x => ((string?)null, (IReadOnlyList<string>)[x.Name], true, x.Column!.At)));

        var expected = stated is null ? context.ClaimOn(element, "index.columns")?.Expected : string.Join(", ", stated.Columns);
        var name = stated is null ? element.SpecializationType == "Index" ? element.Name : null : stated.Name;
        var found = declared.FirstOrDefault(x => name is not null && x.Name == name);
        if (found.Columns is null)
        {
            found = declared.FirstOrDefault(x => expected is not null && string.Join(", ", x.Columns) == expected);
        }

        if (found.Columns is null)
        {
            return ClaimReading.Absent(type.At);
        }

        return context.Kind switch
        {
            "index.columns" => ClaimReading.Stated(string.Join(", ", found.Columns), found.At),
            "index.unique" => ClaimReading.Stated(found.Unique ? "true" : "false", found.At),
            "index.name" => found.Name is { } indexName ? ClaimReading.Stated(indexName, found.At) : ClaimReading.Absent(found.At),
            // JPA has no filtered index.
            _ => ClaimReading.Absent(found.At)
        };
    }

    /// <summary>
    /// JPA's rule for nullability where the Java type doesn't state it (an unannotated reference): <c>@Id</c>,
    /// <c>@Column(nullable = false)</c>, <c>@JoinColumn(nullable = false)</c> or a relationship's <c>optional = false</c>
    /// makes a mapped field's column required, and <c>nullable = true</c> optional; without one, JPA maps it nullable
    /// (decision 13: an extension adds a rule to another's kind). A model that says required is then contradicted by the
    /// missing mapping, and the finding says so rather than that the type is nullable, which it can't say (user decision,
    /// 2026-11-24, for EF Core's equivalent). It reads a Domain attribute or association end, which JPA
    /// maps; a field of a type JPA doesn't map, or another designer's element on a mapped type (a Services DTO that is an
    /// @Embeddable record), is left to the engine.
    /// </summary>
    private static ClaimReading? Nullable(ClaimContext context, JpaCode jpa)
    {
        if (context.Element is not IAssociationEnd && context.Element.SpecializationType != "Attribute"
            || context.Member is not { } member || member.Type?.Nullable is not (null or "unknown")
            || context.Owner(context.Element) is not { } owner || context.TypeOf(owner) is not { } type
            || !jpa.Mapped.Contains(type) && !context.Code.Supertypes(type).Any(x => jpa.Mapped.Any(m => m.Name == x)))
        {
            return null;
        }

        if (member.Attributes.FirstOrDefault(x => x.Name is "Id" or "EmbeddedId") is { } id)
        {
            return ClaimReading.Stated("false", id.At);
        }

        foreach (var annotation in member.Attributes)
        {
            var stated = annotation.Name is "Column" or "JoinColumn" ? annotation.Named.GetValueOrDefault("nullable")
                : Relationships.Contains(annotation.Name) ? annotation.Named.GetValueOrDefault("optional")
                : null;
            if (stated is "true" or "false")
            {
                return ClaimReading.Stated(stated, annotation.At);
            }
        }

        return ClaimReading.Compared("true", member.At, context.Expected == "true",
            $"`{owner.Name}.{context.Element.Name}` is required in the model, but JPA maps it as nullable: nothing sets `@Column(nullable = false)`, "
            + "`@JoinColumn(nullable = false)` or a relationship's `optional = false` on it, and Java's type can't make it required. Add the one that fits.");
    }

    /// <summary>
    /// How many owners each target has, for a relationship's end that no navigation states, from the owning field's
    /// relationship annotation: <c>@ManyToOne</c> and <c>@ManyToMany</c>, many; <c>@Embedded</c> and
    /// <c>@ElementCollection</c>, one; <c>@OneToMany</c> and <c>@OneToOne</c>, one when the owner owns the target (a cascade
    /// that removes it, <c>orphanRemoval = true</c>) or the join column can't be null, else at most one.
    /// </summary>
    private static ClaimReading? Multiplicity(ClaimContext context, JpaCode jpa)
    {
        if (context.ExpectedValue is not Relational.ModelMultiplicity relationship || context.MemberOf(relationship.Navigation) is not { } field
            || context.TypeOf(relationship.Owner) is not { } owner || !jpa.Mapped.Contains(owner))
        {
            return null;
        }

        var annotations = field.Attributes;
        if (annotations.FirstOrDefault(x => x.Name is "ManyToOne" or "ManyToMany") is { } many)
        {
            return ClaimReading.Stated("*", many.At);
        }

        if (annotations.FirstOrDefault(x => x.Name is "Embedded" or "ElementCollection") is { } embedded)
        {
            return ClaimReading.Stated("1", embedded.At);
        }

        if (annotations.FirstOrDefault(x => x.Name is "OneToMany" or "OneToOne") is { } one)
        {
            var owned = one.Named.GetValueOrDefault("orphanRemoval") == "true"
                        || one.Named.GetValueOrDefault("cascade") is { } cascade && (cascade.Contains("ALL", StringComparison.Ordinal) || cascade.Contains("REMOVE", StringComparison.Ordinal));
            var required = annotations.FirstOrDefault(x => x.Name == "JoinColumn")?.Named.GetValueOrDefault("nullable") == "false";
            return ClaimReading.Stated(owned || required ? "1" : "0..1", one.At);
        }

        return ClaimReading.CantVerify("notFound", $"`{owner.Name}.{field.Name}` has no JPA relationship annotation (`@ManyToOne`, `@OneToMany`, `@OneToOne`, `@ManyToMany`, `@Embedded`), so how many `{owner.Name}`s each has can't be read.", field.At);
    }

    /// <summary>What a field's <c>@Column</c> or <c>@GeneratedValue</c> states for a column kind.</summary>
    private static ClaimReading Column(string kind, MemberNode member)
    {
        var column = member.Attributes.FirstOrDefault(x => x.Name == "Column");
        string? Named(string name) => column?.Named.GetValueOrDefault(name);
        return kind switch
        {
            "column.name" => Value(Named("name"), column?.At ?? member.At),
            "column.type" => Value(Named("columnDefinition"), column?.At ?? member.At),
            "column.maxLength" => Value(Named("length"), column?.At ?? member.At),
            "column.precision" => Value(Named("precision"), column?.At ?? member.At),
            "column.scale" => Value(Named("scale"), column?.At ?? member.At),
            // Hibernate's @Nationalized is NVARCHAR; a columnDefinition names the type; without either it's the provider's.
            "column.textType" => member.Attributes.FirstOrDefault(x => x.Name == "Nationalized") is { } nationalized
                ? ClaimReading.Stated("NVARCHAR", nationalized.At)
                : TextType.Match(Named("columnDefinition") ?? "") is { Success: true } text
                    ? ClaimReading.Stated(text.Groups[1].Value.ToLowerInvariant() switch { "varchar" or "char" => "VARCHAR", "nvarchar" or "nchar" => "NVARCHAR", var other => other.ToUpperInvariant() }, column!.At)
                    : ClaimReading.Absent(column?.At ?? member.At, "DEFAULT"),
            // The Primary Key's Data source: @GeneratedValue has JPA generate it; without it the code doesn't state one, as JPA
            // leaves an unannotated key to the code (decision 4).
            _ => member.Attributes.FirstOrDefault(x => x.Name == "GeneratedValue") is { } generated
                ? ClaimReading.Stated("Auto-generated", generated.At)
                : ClaimReading.Absent(member.At)
        };
    }

    private static string? ColumnName(MemberNode member) =>
        member.Attributes.FirstOrDefault(x => x.Name == "Column")?.Named.GetValueOrDefault("name");

    /// <summary>The annotations an annotation's argument holds: one, or a <c>{ ... }</c> list of them.</summary>
    private static IEnumerable<AttributeNode> Nested(AttributeNode? annotation, string argument) =>
        annotation?.Values.FirstOrDefault(x => x.Name == argument) is { } value
            ? value.Annotation is { } single ? [single] : value.Items.Select(x => x.Annotation).OfType<AttributeNode>()
            : [];

    private static IEnumerable<IElement> Attributes(IElement owner) =>
        owner.ChildElements.Where(x => x.SpecializationType == "Attribute");
}
