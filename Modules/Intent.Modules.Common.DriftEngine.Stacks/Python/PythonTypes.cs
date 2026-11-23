using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python;

/// <summary>
/// Annotations as the graph's types (code-reader.md, Type references): <c>list[OrderLine]</c>, <c>typing.Optional[str]</c>,
/// <c>str | None</c>, and a string annotation's forward reference (<c>"Order"</c>) as the type it names. <c>Annotated[T, ...]</c>
/// is <c>T</c>; its metadata is what the member says about it (<see cref="Metadata"/>).
/// </summary>
internal static class PythonTypes
{
    // What makes a type nullable, from the Python profile: the Optional wrapper, and None in a union.
    private static readonly JsonNode Nullable = StockExtensions.LoadPack(PythonLanguage.LanguageName).Profile(PythonLanguage.LanguageName)!["nullable"]!;
    private static readonly HashSet<string> Wrappers = Names(Nullable["wrappers"]).Select(x => x.Split('<')[0]).ToHashSet(StringComparer.Ordinal);
    private static readonly HashSet<string> NullMembers = Names(Nullable["unionMembers"]);

    /// <summary>A type that can hold anything, None included, so it says nothing either way.</summary>
    private static readonly HashSet<string> Anything = ["Any", "object"];

    public static TypeRefNode TypeRef(PythonExpression annotation)
    {
        switch (annotation)
        {
            case PythonString { Value: { } text, IsBytes: false } when PythonParser.Expression(text) is { } named:
                return TypeRef(named);
            case PythonSubscript subscript:
            {
                var generic = TypeRef(subscript.Value);
                var arguments = subscript.Index is PythonSequence { Kind: "tuple" } tuple ? tuple.Items : [subscript.Index];
                if (generic.Name == "Annotated" && arguments.Count > 0)
                {
                    return TypeRef(arguments[0]);
                }

                var types = arguments.Select(TypeRef).ToList();
                if (generic.Name == "Union")
                {
                    return Union(types);
                }

                return new TypeRefNode
                {
                    Text = $"{generic.Text}[{string.Join(", ", types.Select(x => x.Text))}]",
                    Name = generic.Name,
                    QualifiedName = generic.QualifiedName,
                    Arguments = types,
                    Language = PythonLanguage.LanguageName
                };
            }
            case PythonBinary { Operator: "|" } union:
                return Union(Members(union).Select(TypeRef).ToList());
            case PythonSequence { Kind: "list" } list:
            {
                // Callable[[int, str], bool]'s parameters.
                var items = list.Items.Select(TypeRef).ToList();
                var text = $"[{string.Join(", ", items.Select(x => x.Text))}]";
                return new TypeRefNode { Text = text, Name = text, QualifiedName = text, Arguments = items, Language = PythonLanguage.LanguageName };
            }
            default:
            {
                var text = Print(annotation);
                var literal = PythonReader.LiteralOf(annotation, null, "");
                return new TypeRefNode { Text = text, Name = text.Split('.')[^1], QualifiedName = text, Literal = literal, Language = PythonLanguage.LanguageName };
            }
        }
    }

    /// <summary>A union's members, in order: <c>Order | None</c>, <c>Union[Order, None]</c>.</summary>
    private static TypeRefNode Union(IReadOnlyList<TypeRefNode> members)
    {
        var flat = members.SelectMany(x => x.Union.Count > 0 ? x.Union : [x]).ToList();
        var text = string.Join(" | ", flat.Select(x => x.Text));
        return new TypeRefNode { Text = text, Name = text, QualifiedName = text, Union = flat, Language = PythonLanguage.LanguageName };
    }

    private static IEnumerable<PythonExpression> Members(PythonExpression expression) =>
        expression is PythonBinary { Operator: "|" } union ? Members(union.Left).Concat(Members(union.Right)) : [expression];

    /// <summary>
    /// An annotated type's nullability: <c>Optional[T]</c>, or a union with <c>None</c>, is nullable; <c>Any</c> and
    /// <c>object</c> don't say; any other type isn't, as a type checker reads it.
    /// </summary>
    public static string Nullability(TypeRefNode type) =>
        type.Union.Count > 0 ? type.Union.Any(x => NullMembers.Contains(x.Text)) ? "true" : "false"
        : NullMembers.Contains(type.Text) || Wrappers.Contains(type.Name) && type.Arguments.Count == 1 ? "true"
        : Anything.Contains(type.Name) ? "unknown"
        : "false";

    /// <summary><c>Annotated[T, ...]</c>'s metadata, in order: <c>Field(alias="buyer")</c>, <c>Query(max_length=50)</c>; none for any other type.</summary>
    public static IReadOnlyList<PythonExpression> Metadata(PythonExpression? annotation) =>
        annotation is PythonSubscript { Index: PythonSequence { Kind: "tuple" } tuple } subscript && TypeRef(subscript.Value).Name == "Annotated"
            ? tuple.Items.Skip(1).ToList()
            : annotation is PythonString { Value: { } text } && PythonParser.Expression(text) is { } named ? Metadata(named) : [];

    /// <summary>An expression as Python writes it, with Python's spacing, for a type's text.</summary>
    public static string Print(PythonExpression expression) => expression switch
    {
        PythonName name => name.Id,
        PythonAttribute attribute => $"{Print(attribute.Value)}.{attribute.Name}",
        PythonSubscript subscript => $"{Print(subscript.Value)}[{(subscript.Index is PythonSequence { Kind: "tuple" } tuple ? string.Join(", ", tuple.Items.Select(Print)) : Print(subscript.Index))}]",
        PythonConstant constant => constant.Value,
        PythonNumber number => number.Text,
        PythonString { Value: { } text } => $"'{text}'",
        PythonSequence { Kind: "list" } list => $"[{string.Join(", ", list.Items.Select(Print))}]",
        PythonSequence { Kind: "tuple" } tuple => $"({string.Join(", ", tuple.Items.Select(Print))})",
        PythonBinary binary => $"{Print(binary.Left)} {binary.Operator} {Print(binary.Right)}",
        PythonUnary unary => unary.Operator == "not" ? $"not {Print(unary.Operand)}" : $"{unary.Operator}{Print(unary.Operand)}",
        PythonCall call => $"{Print(call.Function)}({string.Join(", ", call.Arguments.Select(x => new string('*', x.Stars) + (x.Name is null ? "" : $"{x.Name}=") + Print(x.Value)))})",
        PythonStarred starred => $"*{Print(starred.Value)}",
        _ => "..."
    };

    private static HashSet<string> Names(JsonNode? list) => (list as JsonArray ?? []).Select(x => x!.GetValue<string>()).ToHashSet(StringComparer.Ordinal);
}
