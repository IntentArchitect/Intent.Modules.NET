using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Java;

/// <summary>
/// The name Java code serializes a member as (<c>member.wireName</c>): Jackson's <c>@JsonProperty("buyer")</c> or
/// <c>@JsonProperty(value = "buyer")</c>, or Gson's <c>@SerializedName("buyer")</c>, on the field or on its accessors. Only
/// what the code states counts: a mapper's naming strategy says nothing about one member (decision 4). A name given as a
/// constant the code declares is that constant's; one the code computes is unreadable; several that disagree are ambiguous.
/// </summary>
internal sealed class JavaWireName : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.wireName"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A member another language reads is that language's to read.
        if (context.Member is not { } member || !new JavaLanguage().Reads(member.At.File))
        {
            return null;
        }

        // Jackson reads a property's annotations from its field and its accessors alike.
        var type = context.Located<TypeNode>();
        var property = member.Name.Length == 0 ? member.Name : char.ToUpperInvariant(member.Name[0]) + member.Name[1..];
        var accessors = new[] { $"get{property}", $"is{property}", $"set{property}" };
        var annotated = new[] { member }.Concat(type?.Members.Where(x => x.Kind == "method" && accessors.Contains(x.Name)) ?? []);
        var stated = new List<(string Value, Span At)>();
        foreach (var annotation in annotated.SelectMany(x => x.Attributes).Where(x => x.Name is "JsonProperty" or "SerializedName"))
        {
            if (annotation.Values.FirstOrDefault(x => x.Name is null or "value") is not { } value)
            {
                continue;
            }

            if ((value.Literal ?? (type is null ? null : context.Code.LiteralOf(value.Text, type)?.Value)) is not { } name)
            {
                return ClaimReading.CantVerify("unreadable", $"The code names it `{value.Text}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", annotation.At);
            }

            stated.Add((name, annotation.At));
        }

        return stated.Select(x => x.Value).Distinct(StringComparer.Ordinal).Count() switch
        {
            0 => ClaimReading.Absent(member.At),
            1 => ClaimReading.Stated(stated[0].Value, stated[0].At),
            _ => ClaimReading.CantVerify("ambiguous", $"`{member.Name}` is given several serialized names: {string.Join(", ", stated.Select(x => $"`{x.Value}`"))}.", member.At, stated.Select(x => x.At).ToList())
        };
    }
}
