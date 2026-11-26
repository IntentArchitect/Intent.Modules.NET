using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Constraints;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;

namespace Intent.Modules.Common.DriftEngine.Stacks.DataAnnotations;

/// <summary>
/// How .NET's System.ComponentModel.DataAnnotations states a Domain attribute's constraints (<see cref="ConstraintsExtension"/>):
/// the validation attributes on the member the attribute's <c>member.exists</c> located. It adds no claims of its own, and
/// leaves EF Core's reading of the same attributes (<c>[Required]</c> and <c>[MaxLength]</c> as a column's nullability and
/// length) to <c>intent.efcore</c>: each claim keeps its own meaning.
/// </summary>
public sealed class DataAnnotationsExtension : IDriftExtension
{
    public string Id => "intent.dataannotations";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new DataAnnotationsClaims()];
}

/// <summary>
/// Reads the constraint kinds from a C# member's DataAnnotations: <c>[Required]</c>; <c>[MinLength]</c>, <c>[MaxLength]</c>,
/// <c>[StringLength(max, MinimumLength = min)]</c> and <c>[Length(min, max)]</c>, a string's length or a collection's count;
/// <c>[RegularExpression(pattern, ErrorMessage = ...)]</c>; <c>[EmailAddress]</c>, <c>[Url]</c> and <c>[Base64String]</c>; and
/// <c>[Range(min, max)]</c> or <c>[Range(typeof(T), "min", "max")]</c> with <c>MinimumIsExclusive</c> and
/// <c>MaximumIsExclusive</c>. It answers only for a member of a C# type that validates with DataAnnotations (a member of it or
/// of its bases carries one of its validation attributes), so a type validated another way (FluentValidation, a domain
/// method) is left to another reader or can't be verified, never read as stating nothing.
/// </summary>
internal sealed class DataAnnotationsClaims : IClaimReader
{
    private const string Framework = "DataAnnotations";

    /// <summary>System.ComponentModel.DataAnnotations' validation attributes, by the name the C# reader gives them.</summary>
    private static readonly HashSet<string> Validation =
    [
        "Required", "MinLength", "MaxLength", "StringLength", "Length", "RegularExpression", "EmailAddress", "Url", "Base64String", "Range", "Phone",
        "CreditCard", "Compare", "AllowedValues", "DeniedValues", "FileExtensions", "CustomValidation"
    ];

    public IReadOnlyCollection<string> Kinds => Constraint.Kinds;

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Member is not { } member
            || context.Code.Files.FirstOrDefault(x => x.Path == member.At.File)?.Language != CSharpLanguage.LanguageName
            || Constraint.Declaring(context, member) is not { } owner
            || !new[] { owner }.Concat(context.Code.Bases(owner)).SelectMany(x => x.Members).SelectMany(x => x.Attributes).Any(x => Validation.Contains(x.Name)))
        {
            return null;
        }

        var collection = context.Element.TypeReference?.IsCollection == true;
        return context.Kind switch
        {
            Constraint.Required => Marker(member, "Required"),
            Constraint.Email => Marker(member, "EmailAddress"),
            Constraint.Url => Marker(member, "Url"),
            Constraint.Base64 => Marker(member, "Base64String"),
            Constraint.MinLength or Constraint.MaxLength when collection => Constraint.PerItem(member, Framework),
            Constraint.MinLength or Constraint.MinItems => Minimum(context, member, owner, text: context.Kind == Constraint.MinLength),
            Constraint.MaxLength or Constraint.MaxItems => Maximum(context, member, owner, text: context.Kind == Constraint.MaxLength),
            Constraint.Pattern => Pattern(context, member, owner),
            Constraint.PatternMessage => Message(context, member, owner),
            Constraint.MinValue => Bound(context, member, owner, 0),
            Constraint.MaxValue => Bound(context, member, owner, 1),
            Constraint.MinExclusive => Exclusive(context, member, owner, "MinimumIsExclusive"),
            Constraint.MaxExclusive => Exclusive(context, member, owner, "MaximumIsExclusive"),
            _ => null
        };
    }

    private static AttributeNode? Find(MemberNode member, string name) => member.Attributes.FirstOrDefault(x => x.Name == name);

    private static AttributeValueNode? Positional(AttributeNode attribute, int index) => attribute.Values.Where(x => x.Name is null).ElementAtOrDefault(index);

    private static AttributeValueNode? Named(AttributeNode attribute, string name) => attribute.Values.FirstOrDefault(x => x.Name == name);

    /// <summary>A constraint stated by an attribute's presence: without it, the code states <c>false</c>.</summary>
    private static ClaimReading Marker(MemberNode member, string name) =>
        Find(member, name) is { } attribute ? ClaimReading.Stated("true", attribute.At) : ClaimReading.Absent(member.At, "false");

    /// <summary>
    /// A minimum length or count: <c>[MinLength(n)]</c>, <c>[Length(n, _)]</c>, or a string's <c>[StringLength(_, MinimumLength = n)]</c>.
    /// None is <c>0</c>, as no minimum is.
    /// </summary>
    private static ClaimReading Minimum(ClaimContext context, MemberNode member, TypeNode owner, bool text)
    {
        if (Find(member, "MinLength") is { } min)
        {
            return Constraint.Limit(context, min, Positional(min, 0), owner);
        }

        if (Find(member, "Length") is { } length)
        {
            return Constraint.Limit(context, length, Positional(length, 0), owner);
        }

        if (text && Find(member, "StringLength") is { } stringLength)
        {
            return Constraint.Limit(context, stringLength, Named(stringLength, "MinimumLength"), owner, "0");
        }

        return ClaimReading.Absent(member.At, "0");
    }

    /// <summary>
    /// A maximum length or count: <c>[MaxLength(n)]</c>, a string's <c>[StringLength(n)]</c>, or <c>[Length(_, n)]</c>.
    /// <c>[MaxLength]</c> with no length allows the most there can be, so it states no maximum.
    /// </summary>
    private static ClaimReading Maximum(ClaimContext context, MemberNode member, TypeNode owner, bool text)
    {
        if (Find(member, "MaxLength") is { } max)
        {
            return Constraint.Limit(context, max, Positional(max, 0), owner);
        }

        if (text && Find(member, "StringLength") is { } stringLength)
        {
            return Constraint.Limit(context, stringLength, Positional(stringLength, 0), owner);
        }

        if (Find(member, "Length") is { } length)
        {
            return Constraint.Limit(context, length, Positional(length, 1), owner);
        }

        return ClaimReading.Absent(member.At);
    }

    private static ClaimReading Pattern(ClaimContext context, MemberNode member, TypeNode owner)
    {
        if (Find(member, "RegularExpression") is not { } regex || Positional(regex, 0) is not { } pattern)
        {
            return ClaimReading.Absent(member.At);
        }

        return Constraint.Text(context, pattern, owner) is { } value ? ClaimReading.Stated(value, regex.At) : Constraint.Unreadable(regex, pattern.Text);
    }

    /// <summary>The <c>ErrorMessage</c> a <c>[RegularExpression]</c> gives; one from resources can't be read from the code.</summary>
    private static ClaimReading Message(ClaimContext context, MemberNode member, TypeNode owner)
    {
        if (Find(member, "RegularExpression") is not { } regex)
        {
            return ClaimReading.Absent(member.At);
        }

        if (Named(regex, "ErrorMessage") is { } message)
        {
            return Constraint.Text(context, message, owner) is { } value ? ClaimReading.Stated(value, regex.At) : Constraint.Unreadable(regex, message.Text);
        }

        return Named(regex, "ErrorMessageResourceName") is { } resource
            ? ClaimReading.CantVerify("unreadable", $"`{regex.Name}` on `{member.Name}` takes its message from the resource `{resource.Text}`, which isn't code the rule reads.", regex.At)
            : ClaimReading.Absent(regex.At);
    }

    /// <summary><c>[Range(min, max)]</c>, or <c>[Range(typeof(T), "min", "max")]</c>, whose bounds follow the type.</summary>
    private static ClaimReading Bound(ClaimContext context, MemberNode member, TypeNode owner, int bound)
    {
        if (Find(member, "Range") is not { } range)
        {
            return ClaimReading.Absent(member.At);
        }

        var offset = Positional(range, 0)?.Text.StartsWith("typeof(", StringComparison.Ordinal) == true ? 1 : 0;
        return Constraint.Limit(context, range, Positional(range, bound + offset), owner);
    }

    /// <summary>Whether a <c>[Range]</c> bound is exclusive (.NET 8's <c>MinimumIsExclusive</c>, <c>MaximumIsExclusive</c>); by default it is inclusive.</summary>
    private static ClaimReading Exclusive(ClaimContext context, MemberNode member, TypeNode owner, string property)
    {
        if (Find(member, "Range") is not { } range)
        {
            return ClaimReading.Absent(member.At, "false");
        }

        if (Named(range, property) is not { } exclusive)
        {
            return ClaimReading.Absent(range.At, "false");
        }

        var value = exclusive.Literal ?? context.Code.LiteralOf(exclusive.Text, owner)?.Value;
        return value is "true" or "false" ? ClaimReading.Stated(value, range.At) : Constraint.Unreadable(range, exclusive.Text);
    }
}
