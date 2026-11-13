using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>One call in a fluent chain: <c>HasMaxLength(50)</c>.</summary>
public sealed record FluentCall
{
    public required string Name { get; init; }

    /// <summary>The arguments: a string literal's value, otherwise the argument as written.</summary>
    public IReadOnlyList<string> Arguments { get; init; } = [];

    /// <summary>The arguments as written: <c>"Deal"</c> with its quotes.</summary>
    public IReadOnlyList<string> Written { get; init; } = [];

    /// <summary>
    /// The members it names: its selector lambdas' (<c>e =&gt; e.Id</c> is <c>Id</c>), or the strings of a call that names
    /// members by string (<c>Property("_alias")</c>, <c>HasForeignKey("BuyerId")</c>).
    /// </summary>
    public IReadOnlyList<string> Selected { get; init; } = [];

    /// <summary>Its generic type arguments: <c>Property&lt;int&gt;("BuyerId")</c>'s <c>int</c>.</summary>
    public IReadOnlyList<TypeRefNode> TypeArguments { get; init; } = [];

    /// <summary>The variable a chain's first call is made on.</summary>
    public string? Receiver { get; init; }

    public required Span At { get; init; }
}

/// <summary>One entity's fluent configuration.</summary>
public sealed class EntityConfig
{
    public required string Entity { get; init; }

    public required Span At { get; init; }

    /// <summary>Calls on the entity's builder: <c>ToTable</c>, <c>HasKey</c>, <c>HasOne</c> chains.</summary>
    public List<FluentCall> Calls { get; } = [];

    /// <summary>The same calls as their chains, first call first: <c>HasOne(d =&gt; d.Region).WithMany().HasForeignKey(d =&gt; d.RegionID)</c>.</summary>
    public List<IReadOnlyList<FluentCall>> Chains { get; } = [];

    /// <summary>
    /// Each configured property's calls, from its <c>Property(e =&gt; e.X)</c> or <c>Property("_x")</c> on, by the name it
    /// selects.
    /// </summary>
    public Dictionary<string, List<FluentCall>> Properties { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// The properties declared by name with their type, <c>Property&lt;int&gt;("BuyerId")</c>: a shadow property when the class
    /// has no member by that name.
    /// </summary>
    public Dictionary<string, (TypeRefNode Type, Span At)> Declared { get; } = new(StringComparer.Ordinal);
}

/// <summary>
/// EF Core's fluent configuration in the in-scope code, by entity type name (<see cref="EfCoreReader"/>). A house extension
/// reads it from <see cref="CodeGraph.Get{T}"/> to check what its own stereotypes state about the configuration.
/// </summary>
public sealed class EfCoreModel
{
    public Dictionary<string, EntityConfig> Entities { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// A type's configuration. A configuration names its entity unqualified, so it is the type's only when the configuring
    /// file reaches the type's namespace: a Services view model named <c>Order</c> isn't the <c>Order</c> entity.
    /// </summary>
    public EntityConfig? Of(TypeNode type, CodeGraph code) =>
        Entities.GetValueOrDefault(type.Name) is { } entity
        && (code.Files.FirstOrDefault(x => x.Path == entity.At.File) is not { } file || code.Reaches(file, type.Namespace))
            ? entity
            : null;
}
