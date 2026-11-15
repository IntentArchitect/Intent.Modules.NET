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
    /// <summary>The sequences a chain can start on, of one element type.</summary>
    private static readonly HashSet<string> Sources = new(StringComparer.Ordinal)
    {
        "DbSet", "IQueryable", "IOrderedQueryable", "IEnumerable", "IOrderedEnumerable", "List", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection",
        "HashSet", "ISet", "IIncludableQueryable"
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

        var name = call.Name.EndsWith("Async", StringComparison.Ordinal) ? call.Name[..^"Async".Length] : call.Name;
        return name switch
        {
            "ToList" => Generic("List", "System.Collections.Generic.List", element),
            // An array's element is its one type argument.
            "ToArray" => new TypeRefNode { Text = $"{element.Text}[]", Name = element.Name, QualifiedName = element.QualifiedName, Arguments = [element], IsArray = true },
            "Count" => Named("int"),
            "LongCount" => Named("long"),
            "Any" or "All" or "Contains" => Named("bool"),
            _ when One.Contains(name) => element,
            // An unfinished query is serialized as the sequence it is.
            _ when Keep.Contains(name) => Generic("IEnumerable", "System.Collections.Generic.IEnumerable", element),
            _ => null
        };
    }

    /// <summary>The element type of the sequence a call is made on: what an operator before it keeps, or what it starts on.</summary>
    private static TypeRefNode? Element(CallNode call)
    {
        if (call.On is { } on)
        {
            return Keep.Contains(on.Name) ? Element(on) : null;
        }

        return call.ReceiverType switch
        {
            { IsArray: true, Arguments: [var element] } => element,
            { Arguments: [var element] } sequence when Sources.Contains(sequence.Name) => element,
            _ => null
        };
    }

    private static TypeRefNode Generic(string name, string qualified, TypeRefNode element) =>
        new() { Text = $"{name}<{element.Text}>", Name = name, QualifiedName = qualified, Arguments = [element] };

    private static TypeRefNode Named(string name) => new() { Text = name, Name = name, QualifiedName = name, IsPredefinedValueType = true };
}
