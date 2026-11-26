namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>
/// A member typed by a type parameter of the generic type of the code that declares it (<c>T Value</c> in
/// <c>ValidatableObject&lt;T&gt;</c>), as the type a model's element is located as sees it: a type parameter that type's bases
/// bind is the argument they give it (<c>class UserName : ValidatableObject&lt;string&gt;</c> has a <c>string</c>
/// <c>Value</c>), and one left unbound is at least an <c>object</c>. A designer with generics states the parameter itself, which
/// the type comparer matches by name; one without them (the User Interface designer's View Model, Store or Contract) can only
/// state <c>object</c>, which is less precise but true, so it isn't drift.
/// </summary>
internal static class TypeParameters
{
    /// <param name="owner">The type the member's owner element is located as; null when nothing located it.</param>
    /// <param name="objectName">How the member's language writes <c>object</c>, by its profile.</param>
    /// <returns>
    /// The member's type with the bases' arguments bound, and with every type parameter left unbound read as <c>object</c>;
    /// null when its type names no type parameter of the types it is reached through.
    /// </returns>
    public static (TypeRefNode Bound, TypeRefNode Erased)? Bound(CodeGraph graph, TypeNode? owner, MemberNode member, string objectName = "object")
    {
        if (member.Type is not { } type)
        {
            return null;
        }

        var parameters = new HashSet<string>(StringComparer.Ordinal);
        var bindings = Bindings(graph, owner, member, parameters);
        if (!Names(type, parameters))
        {
            return null;
        }

        var bound = Substitute(type, bindings);
        var objectType = new TypeRefNode { Text = objectName, Name = objectName, QualifiedName = objectName, Language = type.Language };
        return (bound, Substitute(bound, parameters.ToDictionary(x => x, _ => objectType, StringComparer.Ordinal)));
    }

    /// <summary>
    /// What the owner's bases bind the type parameters of the type declaring the member to, walking from the owner to that
    /// type; the type parameters of every type on the way are added to <paramref name="parameters"/>. With no owner, or a member
    /// the owner doesn't reach, the declaring type's own parameters, unbound.
    /// </summary>
    private static Dictionary<string, TypeRefNode> Bindings(CodeGraph graph, TypeNode? owner, MemberNode member, HashSet<string> parameters)
    {
        if (owner is not null)
        {
            parameters.UnionWith(owner.TypeParameters);
            if (owner.Members.Contains(member))
            {
                return [];
            }

            var seen = new HashSet<TypeNode> { owner };
            var queue = new Queue<(TypeNode Type, Dictionary<string, TypeRefNode> Bindings)>([(owner, [])]);
            while (queue.TryDequeue(out var current))
            {
                foreach (var reference in current.Type.BaseTypes)
                {
                    if (graph.TypeFor(reference, current.Type) is not { } @base || !seen.Add(@base))
                    {
                        continue;
                    }

                    parameters.UnionWith(@base.TypeParameters);
                    var bindings = new Dictionary<string, TypeRefNode>(StringComparer.Ordinal);
                    for (var i = 0; i < @base.TypeParameters.Count && i < reference.Arguments.Count; i++)
                    {
                        bindings[@base.TypeParameters[i]] = Substitute(reference.Arguments[i], current.Bindings);
                    }

                    if (@base.Members.Contains(member))
                    {
                        return bindings;
                    }

                    queue.Enqueue((@base, bindings));
                }
            }
        }

        if (graph.Types.FirstOrDefault(x => x.Members.Contains(member)) is { } declaring)
        {
            parameters.UnionWith(declaring.TypeParameters);
        }

        return [];
    }

    /// <summary>The type names one of the parameters, itself, as an array's item or as a type argument.</summary>
    private static bool Names(TypeRefNode type, IReadOnlySet<string> parameters) =>
        IsParameter(type) && parameters.Contains(type.Name) || type.Arguments.Any(x => Names(x, parameters));

    private static bool IsParameter(TypeRefNode type) => type is { IsArray: false, Arguments.Count: 0 } && type.QualifiedName == type.Name;

    /// <summary>The type with each type parameter the bindings give replaced by its argument, its nullability kept.</summary>
    private static TypeRefNode Substitute(TypeRefNode type, IReadOnlyDictionary<string, TypeRefNode> bindings)
    {
        if (bindings.Count == 0)
        {
            return type;
        }

        if (IsParameter(type) && bindings.TryGetValue(type.Name, out var argument))
        {
            var nullable = type.NullableMarked && !argument.NullableMarked;
            return Copy(argument, argument.Text + (nullable ? "?" : ""), argument.Arguments, argument.NullableMarked || type.NullableMarked);
        }

        if (type.Arguments.Count == 0)
        {
            return type;
        }

        var arguments = type.Arguments.Select(x => Substitute(x, bindings)).ToList();
        if (arguments.SequenceEqual(type.Arguments))
        {
            return type;
        }

        var marker = type.NullableMarked ? "?" : "";
        var text = type.IsArray
            ? $"{arguments[0].Text}[]{marker}"
            : $"{(type.Text.IndexOf('<') is var open and > 0 ? type.Text[..open] : type.Name)}<{string.Join(", ", arguments.Select(x => x.Text))}>{marker}";
        return Copy(type, text, arguments, type.NullableMarked);
    }

    private static TypeRefNode Copy(TypeRefNode type, string text, IReadOnlyList<TypeRefNode> arguments, bool nullableMarked) => new()
    {
        Text = text,
        Name = type.Name,
        QualifiedName = type.QualifiedName,
        Arguments = arguments,
        IsArray = type.IsArray,
        NullableMarked = nullableMarked,
        Nullable = nullableMarked ? "true" : type.Nullable,
        IsPredefinedValueType = type.IsPredefinedValueType,
        Union = type.Union,
        Literal = type.Literal,
        IsAnonymous = type.IsAnonymous,
        Members = type.Members,
        Language = type.Language
    };
}
