using System.Globalization;
using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Constraints;

/// <summary>
/// What a model says about a Domain attribute's validation, whatever framework enforces it: the Domain Constraints stereotypes
/// (Intent.Metadata.Domain.Constraints) Required, Text Limits, Collection Limits, Regular Expression, Email, Url, Base64 and
/// Numeric Limits. It reads no code; a validation framework's extension (<c>intent.dataannotations</c> for .NET's
/// System.ComponentModel.DataAnnotations, <c>intent.beanvalidation</c> for Java's Bean Validation) reads these claims from its
/// own code, as <c>intent.rdbms</c>'s are read by EF Core's or JPA's.
/// </summary>
public sealed class ConstraintsExtension : IDriftExtension
{
    public string Id => "intent.constraints";

    public IEnumerable<Pack> Packs { get; } = [StockExtensions.LoadPack("intent.constraints")];
}

/// <summary>The constraint kinds, and what their readers share: reading a limit as a number, and the readings that can't be read.</summary>
internal static class Constraint
{
    public const string Required = "constraint.required";
    public const string MinLength = "constraint.minLength";
    public const string MaxLength = "constraint.maxLength";
    public const string MinItems = "constraint.minItems";
    public const string MaxItems = "constraint.maxItems";
    public const string Pattern = "constraint.pattern";
    public const string PatternMessage = "constraint.patternMessage";
    public const string Email = "constraint.email";
    public const string Url = "constraint.url";
    public const string Base64 = "constraint.base64";
    public const string MinValue = "constraint.minValue";
    public const string MaxValue = "constraint.maxValue";
    public const string MinExclusive = "constraint.minExclusive";
    public const string MaxExclusive = "constraint.maxExclusive";

    public static readonly IReadOnlyCollection<string> Kinds =
        [Required, MinLength, MaxLength, MinItems, MaxItems, Pattern, PatternMessage, Email, Url, Base64, MinValue, MaxValue, MinExclusive, MaxExclusive];

    /// <summary>A number's suffix (<c>10L</c>, <c>1.5m</c>, <c>2.0f</c>) and digit separators, which aren't its value.</summary>
    private static readonly Regex Decoration = new(@"(?<=\d)[_']|(?<=[\d.])[mMdDfFlLuU]+$", RegexOptions.CultureInvariant);

    /// <summary>A number as the <c>number</c> comparer reads it, or null for anything that isn't one.</summary>
    public static string? Number(string? text) =>
        text is not null && decimal.TryParse(Decoration.Replace(text.Trim(), ""), NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var number)
            ? number.ToString(CultureInfo.InvariantCulture)
            : null;

    /// <summary>
    /// A bound written as the type's own extreme (<c>int.MaxValue</c>, <c>double.NegativeInfinity</c>, Java's
    /// <c>Long.MAX_VALUE</c>): the code saying there is no bound on that side, as <c>[Range(1, int.MaxValue)]</c> does.
    /// </summary>
    public static bool Unbounded(string written) =>
        Regex.IsMatch(written.Trim(), @"\.(MaxValue|MinValue|PositiveInfinity|NegativeInfinity|MAX_VALUE|MIN_VALUE|POSITIVE_INFINITY|NEGATIVE_INFINITY)$", RegexOptions.CultureInvariant);

    /// <summary>
    /// A limit a constraint attribute gives, read as a number: a literal, or a constant the code declares
    /// (<c>[MaxLength(OrderLimits.Name)]</c>); an extreme of its type is no limit (<see cref="Unbounded"/>).
    /// </summary>
    public static ClaimReading Limit(ClaimContext context, AttributeNode attribute, AttributeValueNode? value, TypeNode? owner, string? silence = null)
    {
        if (value is null)
        {
            return ClaimReading.Absent(attribute.At, silence);
        }

        if (Unbounded(value.Text))
        {
            return ClaimReading.Absent(attribute.At);
        }

        var text = value.Literal ?? (owner is null ? null : context.Code.LiteralOf(value.Text, owner)?.Value);
        return Number(text) is { } number ? ClaimReading.Stated(number, attribute.At) : Unreadable(attribute, value.Text);
    }

    /// <summary>A text a constraint attribute gives: a string literal's value, or a string constant's the code declares.</summary>
    public static string? Text(ClaimContext context, AttributeValueNode value, TypeNode? owner) =>
        value.Literal ?? (owner is null ? null : context.Code.ConstantOf(value.Text, owner));

    public static ClaimReading Unreadable(AttributeNode attribute, string written) =>
        ClaimReading.CantVerify("unreadable",
            $"`{attribute.Name}` gives it as `{written}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.",
            attribute.At);

    /// <summary>
    /// Text Limits on a collection limit each item's length, which a constraint on the member itself doesn't state: it limits
    /// how many items there are.
    /// </summary>
    public static ClaimReading PerItem(MemberNode member, string framework) =>
        ClaimReading.CantVerify("unresolved",
            $"`{member.Name}` is a collection, and its Text Limits limit each item's length, which {framework} doesn't state on the member: a length constraint there limits how many items it has.",
            member.At);

    /// <summary>The type that declares a member, as written: its own declaration, else the type the attribute's owner located.</summary>
    public static TypeNode? Declaring(ClaimContext context, MemberNode member) =>
        context.Code.Types.FirstOrDefault(x => x.Members.Contains(member))
        ?? (context.Owner(context.Element) is { } owner ? context.TypeOf(owner) : null);
}
