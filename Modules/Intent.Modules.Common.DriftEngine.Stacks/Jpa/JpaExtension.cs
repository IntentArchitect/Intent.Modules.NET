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

internal sealed class JpaClaims : IClaimReader
{
    private static readonly HashSet<string> Relationships = ["ManyToOne", "OneToOne"];

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "table.name", "table.schema", "table.key", "index.columns", "index.unique", "index.filter", "column.name", "column.type", "column.maxLength",
        "column.precision", "column.scale", "column.valueGeneration", "member.nullable"
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
            case "table.name" or "table.schema":
            {
                var type = context.Type!;
                var table = type.Attributes.FirstOrDefault(x => x.Name == "Table");
                return Value(table?.Named.GetValueOrDefault(context.Kind == "table.name" ? "name" : "schema"), table?.At ?? type.At);
            }
            case "table.key":
                return Key(context);
            case "index.columns" or "index.unique" or "index.filter":
                return Index(context);
            case "member.nullable":
                return Nullable(context, jpa);
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
            // JPA has no filtered index.
            _ => ClaimReading.Absent(found.At)
        };
    }

    /// <summary>
    /// JPA's rule for nullability where the Java type doesn't state it (an unannotated reference): <c>@Id</c>,
    /// <c>@Column(nullable = false)</c>, <c>@JoinColumn(nullable = false)</c> or a relationship's <c>optional = false</c>
    /// makes a mapped field's column required, and <c>nullable = true</c> optional; without one, JPA maps it nullable
    /// (decision 13: an extension adds a rule to another's kind). It reads a Domain attribute or association end, which JPA
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

        return ClaimReading.Defaulted("true", member.At);
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
            // The Primary Key's Data source: JPA generates a key only with @GeneratedValue, so without it the key is user
            // supplied.
            _ => member.Attributes.FirstOrDefault(x => x.Name == "GeneratedValue") is { } generated
                ? ClaimReading.Stated("Auto-generated", generated.At)
                : ClaimReading.Defaulted("User supplied", member.At)
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
