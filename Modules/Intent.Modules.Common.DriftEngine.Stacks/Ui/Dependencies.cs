using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.Ui;

/// <summary>
/// Where one type depends on another: the code side of the User Interface designer's Uses from a plain class (a View Model,
/// a Store, a UI Service), which the designer describes as "an import, hook call, injection or instance it creates". Read
/// from the graph alone, so every C# front end shares it: Blazor's classes now, a MAUI view model's later.
/// </summary>
public static class Dependencies
{
    /// <summary>
    /// Where <paramref name="user"/> depends on <paramref name="used"/>, the nearest first: a field, property or constructor
    /// parameter (a primary constructor's too) typed by it, by an interface of the code it implements (<c>IBasketState</c>
    /// for <c>BasketState</c>) or by a generic type over either (<c>IReadOnlyList&lt;CatalogBrandSelectionViewModel&gt;</c>),
    /// its bases' fields and properties included; else an object of it the type creates; else a call made through a property
    /// of one of its dependencies that is typed by it (<c>_appEnvironment.IdentityService.GetUserInfoAsync()</c>). Types are
    /// resolved as the code names them, so a type of the same name elsewhere doesn't count, and a library interface it also
    /// implements (<c>IDisposable</c>) doesn't stand for it. Null when the code shows none.
    /// </summary>
    public static Dependency? Find(CodeGraph graph, TypeNode user, TypeNode used)
    {
        var accepted = new HashSet<TypeNode> { used };
        accepted.UnionWith(graph.Bases(used).Where(x => x.Kind == "interface"));
        bool Names(TypeRefNode? type) => type is not null && (graph.TypeFor(type, user) is { } resolved && accepted.Contains(resolved) || type.Arguments.Any(Names));

        var members = graph.MembersOf(user).ToList();
        foreach (var member in members)
        {
            if (member.Kind is "field" or "property" && Names(member.Type))
            {
                return new Dependency(member, member.At);
            }

            if (member.Kind == "constructor" && member.Parameters.FirstOrDefault(x => Names(x.Type)) is { } parameter)
            {
                return new Dependency(parameter, parameter.At);
            }
        }

        if (user.Members.SelectMany(x => x.Creations).FirstOrDefault(x => Names(x.Type)) is { } creation)
        {
            return new Dependency(creation, creation.At);
        }

        // A call through a dependency's property typed by the used type: `_holder.Used.Call()`.
        var holders = members.Where(x => x.Kind is "field" or "property").Select(x => x.Type)
            .Concat(members.Where(x => x.Kind == "constructor").SelectMany(x => x.Parameters).Select(x => x.Type))
            .OfType<TypeRefNode>()
            .Select(x => graph.TypeFor(x, user))
            .OfType<TypeNode>()
            .Where(x => !accepted.Contains(x))
            .Distinct()
            .ToList();
        var through = holders.SelectMany(x => graph.ImplementationsOf(x).Prepend(x))
            .SelectMany(owner => graph.MembersOf(owner).Select(member => (Owner: owner, Member: member)))
            .Where(x => x.Member.Kind == "property" && x.Member.Type is { } type && graph.TypeFor(type, x.Owner) is { } resolved && accepted.Contains(resolved))
            .Select(x => x.Member.Name)
            .ToHashSet(StringComparer.Ordinal);
        if (through.Count > 0 && user.Members.SelectMany(x => x.Calls).FirstOrDefault(x => x.Receiver is { } receiver && through.Contains(receiver.Split('.')[^1])) is { } call)
        {
            return new Dependency(call, call.At);
        }

        return null;
    }
}

/// <summary>Where a type depends on another: the field, property, parameter, creation or call that shows it, and its place.</summary>
public sealed record Dependency(object Node, Span At);
