using System.Text.Json.Nodes;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>
/// The <c>type</c> comparer (rules.md, Language profiles): maps the model's type through the profile's types and
/// collections and compares it with the code's type ref. Nullability isn't part of it.
/// </summary>
internal static class TypeComparer
{
    public static bool Matches(ITypeReference model, TypeRefNode code, JsonNode profile)
    {
        if (!model.IsCollection && IsWrittenAs(model, code, profile))
        {
            return true;
        }

        code = WithoutNullable(code, profile);

        // A primitive the profile writes in a collection's form (binary is byte[]) matches as written, before collections.
        if (!model.IsCollection && Written(model, profile) is { } whole && (whole.Contains(code.Text) || whole.Contains(code.QualifiedName)))
        {
            return true;
        }

        if (model.IsCollection)
        {
            if (CollectionItem(code, profile) is not { } item)
            {
                return false;
            }

            // A collection of a type definition named as its items are written: int? for int?[].
            if (IsWrittenAs(model, item, profile))
            {
                return true;
            }

            code = WithoutNullable(item, profile);
        }
        else if (CollectionItem(code, profile) is not null)
        {
            return false;
        }

        if (model.Element is not { } element)
        {
            return false;
        }

        // A primitive is a type definition the profile maps; a modelled element compares by name, unresolved.
        if (Written(model, profile) is { } written)
        {
            return written.Contains(code.QualifiedName) || written.Contains(code.Text);
        }

        if (IsWrittenAs(model, code, profile))
        {
            return true;
        }

        // Type arguments count only when the model states them: Paged<Order> isn't Paged<Product>.
        var arguments = model.GenericTypeParameters?.ToList() ?? [];
        return code.Name == element.Name && (arguments.Count == 0 ||
            arguments.Count == code.Arguments.Count && arguments.Zip(code.Arguments).All(x => Matches(x.First, x.Second, profile)));
    }

    /// <summary>
    /// A type definition the profile doesn't map, named as the code writes the type (<c>Func&lt;Task&gt;</c>, <c>int?</c>), is
    /// that type, nullability included.
    /// </summary>
    private static bool IsWrittenAs(ITypeReference model, TypeRefNode code, JsonNode profile) =>
        model.Element is { SpecializationType: "Type-Definition" } element && element.Name.IndexOfAny(['<', '[', '?']) >= 0 && Written(model, profile) is null
        && element.Name.Replace(" ", "") == code.Text.Replace(" ", "");

    /// <summary>How the profile writes the model's type, when it's a primitive (a type definition the profile maps).</summary>
    private static IReadOnlyList<string>? Written(ITypeReference model, JsonNode profile) =>
        model.Element is { SpecializationType: "Type-Definition" } element && profile["types"]?[element.Name] is JsonArray names
            ? names.Select(x => x!.GetValue<string>()).ToList()
            : null;

    private static TypeRefNode WithoutNullable(TypeRefNode code, JsonNode profile)
    {
        // A union with what only makes it nullable (TypeScript's string | null) is its one other member.
        if (code.Union.Count > 0)
        {
            var nulls = (profile["nullable"]?["unionMembers"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToHashSet() ?? [];
            return code.Union.Where(x => !nulls.Contains(x.Text)).ToList() is [var only] ? WithoutNullable(only, profile) : code;
        }

        if (code.NullableMarked)
        {
            return new TypeRefNode { Text = code.Text.TrimEnd('?'), Name = code.Name, QualifiedName = code.QualifiedName, Arguments = code.Arguments, IsArray = code.IsArray, IsPredefinedValueType = code.IsPredefinedValueType, Language = code.Language };
        }

        var wrappers = (profile["nullable"]?["wrappers"] as JsonArray)?.Select(x => x!.GetValue<string>().Split('<')[0]) ?? [];
        return wrappers.Contains(code.Name) && code.Arguments.Count == 1 ? code.Arguments[0] : code;
    }

    /// <summary>The item type, when the code type is one of the profile's collection forms.</summary>
    private static TypeRefNode? CollectionItem(TypeRefNode code, JsonNode profile)
    {
        if (code.IsArray)
        {
            return code.Arguments[0];
        }

        var forms = (profile["collections"] as JsonArray)?.Select(x => x!.GetValue<string>()).Where(x => x.EndsWith("<T>", StringComparison.Ordinal)).Select(x => x[..^3]);
        return code.Arguments.Count == 1 && forms?.Contains(code.Name) == true ? code.Arguments[0] : null;
    }
}
