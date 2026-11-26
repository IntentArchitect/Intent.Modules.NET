using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Relational;

namespace Intent.Modules.Common.DriftEngine.Stacks.Jpa;

/// <summary>
/// How JPA states what the model says about mapping a class's fields and relationships, and how Spring Data states a
/// repository: <c>@Transient</c>, <c>@Embedded</c>, <c>@Version</c>, <c>@Enumerated</c>, <c>mappedBy</c>, <c>@JoinColumn</c>
/// and a <c>JpaRepository&lt;T, ID&gt;</c>.
/// </summary>
internal sealed partial class JpaClaims
{
    private static readonly HashSet<string> RelationshipAnnotations = ["ManyToOne", "OneToOne", "OneToMany", "ManyToMany"];

    /// <summary>JPA maps every field of an entity unless <c>@Transient</c> or <c>transient</c> says it doesn't, so silence is <c>true</c>.</summary>
    private static ClaimReading Mapped(MemberNode member) =>
        Transient(member) is { } transient ? ClaimReading.Stated("false", transient) : ClaimReading.Defaulted("true", member.At);

    private static Span? Transient(MemberNode member) =>
        member.Attributes.FirstOrDefault(x => x.Name == "Transient")?.At ?? (member.Modifiers.Contains("transient") ? member.At : null);

    /// <summary>
    /// Whether a value object field is embedded, JPA's owned type: <c>@Embedded</c>, <c>@ElementCollection</c>, or a field whose
    /// type is <c>@Embeddable</c>, which JPA embeds without saying so. A relationship annotation maps it as an entity of its own.
    /// </summary>
    private static ClaimReading Owned(ClaimContext context, MemberNode member)
    {
        if (member.Attributes.FirstOrDefault(x => x.Name is "Embedded" or "EmbeddedId" or "ElementCollection") is { } embedded)
        {
            return ClaimReading.Stated("true", embedded.At);
        }

        if (member.Attributes.FirstOrDefault(x => RelationshipAnnotations.Contains(x.Name)) is { } related)
        {
            return ClaimReading.Stated("false", related.At);
        }

        var from = context.Owner(context.Element) is { } owner ? context.TypeOf(owner) : null;
        var held = member.Type is { } reference && from is not null ? context.Code.TypeFor(reference.Arguments is [var item] ? item : reference, from) : null;
        return held?.Attributes.Any(x => x.Name == "Embeddable") == true
            ? ClaimReading.Stated("true", member.At)
            : ClaimReading.Absent(member.At, "false");
    }

    /// <summary>A field is the entity's concurrency token with <c>@Version</c>; JPA checks none unless a field says so.</summary>
    private static ClaimReading Version(MemberNode member) =>
        member.Attributes.FirstOrDefault(x => x.Name == "Version") is { } version
            ? ClaimReading.Stated("true", version.At)
            : ClaimReading.Absent(member.At, "false");

    /// <summary>
    /// How an enum field is stored: <c>@Enumerated(EnumType.STRING)</c> is <c>String</c>, <c>@Enumerated</c> or
    /// <c>EnumType.ORDINAL</c> is <c>Number</c>. JPA's default, its ordinal, is never read in place of a modelled storage
    /// (decision 4); an <c>@Convert</c> stores what its converter's code says.
    /// </summary>
    private static ClaimReading EnumStorage(MemberNode member)
    {
        if (member.Attributes.FirstOrDefault(x => x.Name == "Convert") is { } convert)
        {
            return ClaimReading.CantVerify("unreadable", "The code converts it with an `@Convert` converter, which the rule can't read without running the code: what it stores is in the converter's code.", convert.At);
        }

        if (member.Attributes.FirstOrDefault(x => x.Name == "Enumerated") is not { } enumerated)
        {
            return ClaimReading.Absent(member.At);
        }

        var type = enumerated.Values.FirstOrDefault(x => x.Name is null or "value")?.Text ?? "";
        return ClaimReading.Stated(type.EndsWith("STRING", StringComparison.Ordinal) ? "String" : "Number", enumerated.At);
    }

    /// <summary>
    /// Whether an association's source end is navigable. Where the model says so, its field is mapped unless it is transient.
    /// Where it doesn't, the target class has no field back: the navigation's <c>mappedBy</c> names the field that owns the
    /// relationship from the other side, else a field of the target class typed as the source class, which no other
    /// association draws, navigates back.
    /// </summary>
    private static ClaimReading? Navigable(ClaimContext context)
    {
        if (context.ExpectedValue is not ModelNavigability relationship || context.Member is not { } member)
        {
            return null;
        }

        if (context.Expected == "true")
        {
            return Transient(member) is { } transient ? ClaimReading.Stated("false", transient) : ClaimReading.Stated("true", member.At);
        }

        if (context.TypeOf(relationship.SourceClass) is not { } owner || context.TypeOf(relationship.TargetClass) is not { } target)
        {
            return ClaimReading.CantVerify("notFound", $"`{relationship.TargetClass.Name}` isn't located, so whether it navigates back to `{relationship.SourceClass.Name}` can't be read.");
        }

        if (member.Attributes.Where(x => RelationshipAnnotations.Contains(x.Name)).Select(x => (Annotation: x, MappedBy: x.Named.GetValueOrDefault("mappedBy")))
                .FirstOrDefault(x => !string.IsNullOrEmpty(x.MappedBy)) is { Annotation: not null } mapped)
        {
            return ClaimReading.Stated("true", context.Code.MembersOf(target).FirstOrDefault(x => x.Name == mapped.MappedBy)?.At ?? mapped.Annotation.At);
        }

        var drawn = Navigations.DrawnElsewhere(context, relationship);
        var candidates = context.Code.MembersOf(target)
            .Where(x => x.Kind == "field" && x != member && !x.IsStatic && Navigations.Holds(x, owner) && !drawn.Contains(x) && Transient(x) is null)
            .ToList();
        return candidates switch
        {
            [] => ClaimReading.Absent(target.At, "false"),
            [var only] => ClaimReading.Stated("true", only.At),
            _ => ClaimReading.CantVerify("ambiguous", $"{candidates.Count} fields of `{target.Name}` could navigate back to `{owner.Name}`.", candidates[0].At, candidates.Select(x => x.At).ToList())
        };
    }

    /// <summary>
    /// The join column the field that owns a relationship names: <c>@JoinColumn(name = "order_id")</c>, or a constant the code
    /// declares. JPA's <c>&lt;field&gt;_id</c> default is never read in place of a modelled column (decision 4).
    /// </summary>
    private static ClaimReading? JoinColumn(ClaimContext context)
    {
        if (context.ExpectedValue is not ModelJoinColumn join || context.Member is not { } member)
        {
            return null;
        }

        if (member.Attributes.FirstOrDefault(x => x.Name == "JoinColumn") is not { } joinColumn)
        {
            return ClaimReading.Absent(member.At);
        }

        if (joinColumn.Values.FirstOrDefault(x => x.Name == "name") is not { } name)
        {
            return ClaimReading.Absent(joinColumn.At);
        }

        if (name.Literal is { } literal)
        {
            return ClaimReading.Stated(literal, joinColumn.At);
        }

        return context.TypeOf(join.Owner) is { } from && context.Code.LiteralOf(name.Text, from) is { Value: { } constant }
            ? ClaimReading.Stated(constant, joinColumn.At)
            : ClaimReading.CantVerify("unreadable", $"The code states it as `{name.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", joinColumn.At);
    }

    /// <summary>
    /// The foreign key a relationship's join column holds: the dependent's fields mapped to the column the owning side's
    /// <c>@JoinColumn(name = "product_id")</c> names (the dependent's <c>@ManyToOne</c> or <c>@OneToOne</c> navigation, else the
    /// principal's <c>@OneToMany</c> with its own join column). A field maps to the column its <c>@Column(name)</c> names, else
    /// to its own name, which JPA's default naming keeps and Spring Boot's writes in snake case, so underscores and case are
    /// set aside. A navigation with no <c>@JoinColumn</c> leaves the column to JPA's default, which isn't read (decision 4).
    /// </summary>
    private static ClaimReading? ForeignKey(ClaimContext context)
    {
        if (context.ExpectedValue is not ModelForeignKey key || context.TypeOf(key.Dependent) is not { } dependent)
        {
            return null;
        }

        var fields = context.Code.MembersOf(dependent).Where(x => x.Kind == "field" && !x.IsStatic).ToList();
        var joinColumn = Navigation(fields, key.DependentNavigation, ["ManyToOne", "OneToOne"])?.Attributes.FirstOrDefault(x => x.Name == "JoinColumn");
        if (joinColumn is null && key.Principal is not null && context.TypeOf(key.Principal) is { } principal)
        {
            var inverse = Navigation(context.Code.MembersOf(principal).Where(x => x.Kind == "field").ToList(), key.PrincipalNavigation, ["OneToMany", "OneToOne"]);
            joinColumn = inverse?.Attributes.FirstOrDefault(x => x.Name == "JoinColumn");
        }

        if (joinColumn?.Values.FirstOrDefault(x => x.Name == "name") is not { } named)
        {
            return null;
        }

        var column = named.Literal ?? context.Code.LiteralOf(named.Text, dependent)?.Value;
        if (column is null)
        {
            return ClaimReading.CantVerify("unreadable", $"The join column is stated as `{named.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", joinColumn.At);
        }

        var keys = fields.Where(x => !x.Attributes.Any(a => RelationshipAnnotations.Contains(a.Name)) && SameColumn(ColumnName(x) ?? x.Name, column))
            .Select(x => key.Attributes.FirstOrDefault(a => string.Equals(a.Name, x.Name, StringComparison.OrdinalIgnoreCase))?.Name ?? x.Name)
            .ToList();
        if (keys.Count == 0)
        {
            return ClaimReading.Compared("none", joinColumn.At, false,
                $"`{dependent.Name}` foreign key: model `{context.Expected}`, code none. No field of `{dependent.Name}` is mapped to its join column `{column}`. " +
                $"Map the foreign key's field to `{column}` (`@Column(name = \"{column}\")`), or change the model's Foreign Key.");
        }

        return ClaimReading.Stated(string.Join(", ", keys), joinColumn.At);

        static MemberNode? Navigation(IReadOnlyList<MemberNode> fields, string? name, string[] annotations) =>
            name is null ? null : fields.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.Attributes.Any(a => annotations.Contains(a.Name)));

        static bool SameColumn(string a, string b) => string.Equals(a.Replace("_", ""), b.Replace("_", ""), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// The entity a class's own repository is for, where a house stereotype on the class states it: its <c>{Repository}</c>
    /// binding, else <c>{Name}Repository</c>, and the first type argument of the Spring Data repository it extends
    /// (<c>JpaRepository&lt;Order, UUID&gt;</c>). A Domain Repository element is the engine's own rule's.
    /// </summary>
    private static ClaimReading? RepositoryEntity(ClaimContext context)
    {
        if (context.Element.SpecializationType == "Repository" || context.Type is not { } entity)
        {
            return null;
        }

        var name = context.Claim.Bindings?.GetValueOrDefault("Repository") is { Length: > 0 } bound ? bound : $"{context.Element.Name}Repository";
        var repositories = context.Code.Types.Where(x => x.Name == name).ToList();
        if (repositories.Count > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{repositories.Count} types are named `{name}`.", repositories[0].At, repositories.Select(x => x.At).ToList());
        }

        if (repositories is not [var repository])
        {
            return ClaimReading.Absent(entity.At);
        }

        foreach (var declaring in new[] { repository }.Concat(context.Code.Bases(repository)))
        {
            if (declaring.BaseTypes.FirstOrDefault(x => x.Arguments.Count > 0 && x.Name.EndsWith("Repository", StringComparison.Ordinal)) is { } based)
            {
                var argument = based.Arguments[0];
                var same = context.Code.TypeFor(argument, declaring) is { } resolved ? resolved == entity : argument.Name == context.Element.Name;
                return ClaimReading.Compared(argument.Text, repository.At, same);
            }
        }

        return ClaimReading.CantVerify("notFound", $"`{name}` extends no generic Spring Data repository, such as `JpaRepository<T, ID>`, whose type argument names its entity.", repository.At);
    }

    /// <summary>
    /// A repository interface that extends a Spring Data repository has its implementation from Spring Data at run time, so it
    /// is implemented. Anything else is the engine's own rule's: a class in scope that implements it.
    /// </summary>
    private static ClaimReading? RepositoryImplementation(ClaimContext context)
    {
        if (context.Type is not { Kind: "interface" } type)
        {
            return null;
        }

        var springData = context.Code.Files.Where(x => type.Files.Contains(x.Path)).SelectMany(x => x.Imports)
            .Any(x => x.Target.StartsWith("org.springframework.data", StringComparison.Ordinal));
        var extendsRepository = new[] { type }.Concat(context.Code.Bases(type))
            .SelectMany(x => x.BaseTypes.Where(b => context.Code.TypeFor(b, x) is null))
            .Any(x => x.Name.EndsWith("Repository", StringComparison.Ordinal));
        return springData && extendsRepository ? ClaimReading.Stated("present", type.At) : null;
    }
}
