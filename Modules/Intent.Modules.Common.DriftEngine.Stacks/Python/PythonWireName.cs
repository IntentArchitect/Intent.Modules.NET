using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python;

/// <summary>
/// The name Python code serializes a member as (<c>member.wireName</c>): Pydantic's <c>Field(serialization_alias="buyer")</c>,
/// else its <c>Field(alias="buyer")</c>, as the member's value or in its <c>Annotated</c> metadata; marshmallow's
/// <c>fields.Str(data_key="buyer")</c>; msgspec's <c>field(name="buyer")</c>. Only what the code states counts: a model's
/// <c>alias_generator</c> says nothing about one member (decision 4). A name given as a constant the code declares is that
/// constant's; one the code computes is unreadable; several that disagree are ambiguous.
/// </summary>
internal sealed class PythonWireName : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.wireName"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A member another language reads is that language's to read.
        if (context.Member is not { } member || !new PythonLanguage().Reads(member.At.File))
        {
            return null;
        }

        var type = context.Located<TypeNode>();
        var stated = new List<(string Value, Span At)>();

        // The member's value: a creation (Field, a marshmallow field) or a call (msgspec's field).
        var given = member.Value is null ? [] : member.Value.Creations.Select(x => (x.Type.Name, x.Arguments, x.At))
            .Concat(member.Value.Call is { } call ? [(call.Name, call.Arguments, call.At)] : []);
        foreach (var (name, arguments, at) in given)
        {
            if (Named(name, arguments.Where(x => x.Name is not null).Select(x => x.Name!).ToList()) is not { } keyword)
            {
                continue;
            }

            var value = arguments.First(x => x.Name == keyword);
            if (context.Code.StringOf(value) is not { Kind: "string", Value: { } text })
            {
                return ClaimReading.CantVerify("unreadable", $"The code names it `{value.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", at);
            }

            stated.Add((text, at));
        }

        // Annotated[str, Field(alias="buyer")].
        foreach (var attribute in member.Attributes)
        {
            if (Named(attribute.Name, attribute.Named.Keys.ToList()) is not { } keyword)
            {
                continue;
            }

            var value = attribute.Values.First(x => x.Name == keyword);
            if ((value.Literal ?? (type is null ? null : context.Code.LiteralOf(value.Text, type)?.Value)) is not { } text)
            {
                return ClaimReading.CantVerify("unreadable", $"The code names it `{value.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", attribute.At);
            }

            stated.Add((text, attribute.At));
        }

        return stated.Select(x => x.Value).Distinct(StringComparer.Ordinal).Count() switch
        {
            0 => ClaimReading.Absent(member.At),
            1 => ClaimReading.Stated(stated[0].Value, stated[0].At),
            _ => ClaimReading.CantVerify("ambiguous", $"`{member.Name}` is given several serialized names: {string.Join(", ", stated.Select(x => $"`{x.Value}`"))}.", member.At, stated.Select(x => x.At).ToList())
        };
    }

    /// <summary>The keyword that names the member in what it is given: Pydantic's serialization alias over its alias, which names both ways.</summary>
    private static string? Named(string callee, IReadOnlyList<string> keywords) => callee switch
    {
        "Field" when keywords.Contains("serialization_alias") => "serialization_alias",
        "Field" when keywords.Contains("alias") => "alias",
        "field" when keywords.Contains("name") => "name",
        _ when keywords.Contains("data_key") => "data_key",
        _ => null
    };
}
