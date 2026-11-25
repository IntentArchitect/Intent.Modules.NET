using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// What a LINQ or EF Core query gives, read from its chain of calls without a compiler:
/// <c>context.CatalogTypes.OrderBy(x =&gt; x.Type).ToListAsync()</c> is a <c>List&lt;CatalogType&gt;</c>, because the chain
/// starts on a <c>DbSet&lt;CatalogType&gt;</c> property, keeps its element through <c>OrderBy</c>, and <c>ToListAsync</c> lists
/// it. An operator that changes the element (<c>Select</c>) or isn't known leaves it unknown, never guessed.
/// </summary>
internal static class Sequences
{
    /// <summary>
    /// The sequences a chain can start on, of one element type; EF6's too, such as the <c>ObjectResult&lt;T&gt;</c> a database-first
    /// context's stored procedure method gives (<c>context.spGetOrder(id).FirstOrDefault()</c>) and <c>Database.SqlQuery&lt;T&gt;</c>'s.
    /// </summary>
    private static readonly HashSet<string> Sources = new(StringComparer.Ordinal)
    {
        "DbSet", "IQueryable", "IOrderedQueryable", "IEnumerable", "IOrderedEnumerable", "List", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection",
        "HashSet", "ISet", "IIncludableQueryable", "ObjectResult", "DbRawSqlQuery", "DbSqlQuery"
    };

    /// <summary>The operators that keep the element type.</summary>
    private static readonly HashSet<string> Keep = new(StringComparer.Ordinal)
    {
        "Where", "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending", "Skip", "Take", "Include", "ThenInclude", "AsNoTracking",
        "AsNoTrackingWithIdentityResolution", "AsTracking", "AsQueryable", "AsEnumerable", "AsSplitQuery", "AsSingleQuery", "IgnoreQueryFilters", "Distinct", "Reverse",
        "TagWith"
    };

    private static readonly HashSet<string> One = new(StringComparer.Ordinal)
    {
        "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault", "Find", "ElementAt", "ElementAtOrDefault"
    };

    /// <summary>What the call gives when it ends a query chain, awaited: null when the chain or the call isn't one it knows.</summary>
    public static TypeRefNode? Result(CallNode call)
    {
        if (Element(call) is not { } element)
        {
            return null;
        }

        return Awaited(call.Name, element);
    }

    /// <summary>A query operator this reads, async or not: one that keeps, picks, counts, lists or projects the elements, or groups them.</summary>
    public static bool IsOperator(string name)
    {
        var bare = Bare(name);
        return Keep.Contains(bare) || One.Contains(bare) || bare is "ToList" or "ToArray" or "Count" or "LongCount" or "Any" or "All" or "Contains" or "Select" or "GroupBy";
    }

    /// <summary>A sequence's element type: an array's, or one of the sequences a chain can start on; null for anything else.</summary>
    public static TypeRefNode? ElementOf(TypeRefNode? sequence) => sequence switch
    {
        { IsArray: true, Arguments: [var element] } => element,
        { Arguments: [var element] } when Sources.Contains(sequence.Name) => element,
        _ => null
    };

    /// <summary>
    /// What an operator gives as written, on a sequence of <paramref name="element"/> (for a <c>Select</c>, of what it projects
    /// to): an async one's (<c>ToListAsync</c>) is a <c>Task</c> of what it gives awaited.
    /// </summary>
    public static TypeRefNode? Given(string name, TypeRefNode element) =>
        Awaited(name, element) is not { } result ? null
        : name.EndsWith("Async", StringComparison.Ordinal) ? Generic("Task", "System.Threading.Tasks.Task", result)
        : result;

    /// <summary>What an operator gives once awaited, on a sequence of <paramref name="element"/>.</summary>
    private static TypeRefNode? Awaited(string name, TypeRefNode element) =>
        Bare(name) switch
        {
            "ToList" => Generic("List", "System.Collections.Generic.List", element),
            // An array's element is its one type argument.
            "ToArray" => new TypeRefNode { Text = $"{element.Text}[]", Name = element.Name, QualifiedName = element.QualifiedName, Arguments = [element], IsArray = true },
            "Count" => Named("int"),
            "LongCount" => Named("long"),
            "Any" or "All" or "Contains" => Named("bool"),
            var bare when One.Contains(bare) => element,
            // An unfinished query is serialized as the sequence it is.
            var bare when Keep.Contains(bare) || bare == "Select" => Generic("IEnumerable", "System.Collections.Generic.IEnumerable", element),
            _ => null
        };

    private static string Bare(string name) => name.EndsWith("Async", StringComparison.Ordinal) ? name[..^"Async".Length] : name;

    /// <summary>
    /// The element type of the sequence a call is made on: what an operator before it keeps, what a <c>Select</c> before it
    /// projects to, or what it starts on. A call that is a <c>Select</c> gives its projection.
    /// </summary>
    private static TypeRefNode? Element(CallNode call)
    {
        if (call.Name == "Select")
        {
            return Projected(call);
        }

        // Made on another operator, the sequence is what that one gives; made on any other call, what the call is typed as.
        if (call.On is { } on && (Keep.Contains(on.Name) || on.Name == "Select"))
        {
            return Element(on);
        }

        return ElementOf(call.ReceiverType);
    }

    /// <summary>
    /// What a <c>Select</c>'s lambda returns, when the code says and every return agrees: <c>x =&gt; new OrderDto { ... }</c> is
    /// <c>OrderDto</c>. An anonymous type, or a projection the code doesn't type, leaves it unknown.
    /// </summary>
    private static TypeRefNode? Projected(CallNode select) =>
        select.Arguments is [{ Function.Returns: [_, ..] returns }]
        && returns.Select(x => x is { IsKnown: true, Types: [var only] } ? only : null).ToList() is [{ } first, ..] types
        && types.All(x => x?.Text == first.Text)
            ? first
            : null;

    private static TypeRefNode Generic(string name, string qualified, TypeRefNode element) =>
        new() { Text = $"{name}<{element.Text}>", Name = name, QualifiedName = qualified, Arguments = [element] };

    private static TypeRefNode Named(string name) => new() { Text = name, Name = name, QualifiedName = name, IsPredefinedValueType = true };
}
