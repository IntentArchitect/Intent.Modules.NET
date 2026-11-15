using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// The name C# code serializes a member as (<c>member.wireName</c>): System.Text.Json's <c>[JsonPropertyName("buyer")]</c>,
/// Newtonsoft's <c>[JsonProperty("buyer")]</c> or <c>[JsonProperty(PropertyName = "buyer")]</c>, or a data contract's
/// <c>[DataMember(Name = "buyer")]</c>. Only what the code states counts: a serializer's naming policy says nothing about one
/// member (decision 4), so a member with none of them doesn't state its name. A name given as a constant the code declares is
/// that constant's; one the code computes is unreadable; several that disagree are ambiguous.
/// </summary>
internal sealed class CSharpWireName : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.wireName"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A member another language reads is that language's to read.
        if (context.Member is not { } member || !new CSharpLanguage().Reads(member.At.File))
        {
            return null;
        }

        var stated = new List<(string Value, Span At)>();
        foreach (var attribute in member.Attributes)
        {
            var value = attribute.Name switch
            {
                "JsonPropertyName" => Positional(attribute),
                "JsonProperty" => Named(attribute, "PropertyName") ?? Positional(attribute),
                "DataMember" => Named(attribute, "Name"),
                _ => null
            };
            if (value is null)
            {
                continue;
            }

            if ((value.Literal ?? (context.Located<TypeNode>() is { } type ? context.Code.LiteralOf(value.Text, type)?.Value : null)) is not { } name)
            {
                return ClaimReading.CantVerify("unreadable", $"The code names it `{value.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", attribute.At);
            }

            stated.Add((name, attribute.At));
        }

        return stated.Select(x => x.Value).Distinct(StringComparer.Ordinal).Count() switch
        {
            0 => ClaimReading.Absent(member.At),
            1 => ClaimReading.Stated(stated[0].Value, stated[0].At),
            _ => ClaimReading.CantVerify("ambiguous", $"`{member.Name}` is given several serialized names: {string.Join(", ", stated.Select(x => $"`{x.Value}`"))}.", member.At, stated.Select(x => x.At).ToList())
        };
    }

    private static AttributeValueNode? Positional(AttributeNode attribute) => attribute.Values.FirstOrDefault(x => x.Name is null);

    private static AttributeValueNode? Named(AttributeNode attribute, string name) => attribute.Values.FirstOrDefault(x => x.Name == name);
}
