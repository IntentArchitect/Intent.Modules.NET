using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Constraints;
using Intent.Modules.Common.DriftEngine.Stacks.Java;

namespace Intent.Modules.Common.DriftEngine.Stacks.BeanValidation;

/// <summary>
/// How Java's Bean Validation (Jakarta Validation, and <c>javax.validation</c> before it) states a Domain attribute's
/// constraints (<see cref="ConstraintsExtension"/>): the constraint annotations on the field the attribute's
/// <c>member.exists</c> located, with Hibernate Validator's own <c>@URL</c>, <c>@Length</c> and <c>@Range</c>, since Bean
/// Validation has no URL constraint of its own. It adds no claims of its own, and leaves JPA's reading of a field's
/// annotations (<c>@Column(nullable, length)</c>) to <c>intent.jpa</c>.
/// </summary>
public sealed class BeanValidationExtension : IDriftExtension
{
    public string Id => "intent.beanvalidation";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new BeanValidationClaims()];
}

/// <summary>
/// Reads the constraint kinds from a Java field's Bean Validation annotations: <c>@NotNull</c>, <c>@NotBlank</c> or
/// <c>@NotEmpty</c> for Required; <c>@Size(min, max)</c> (and Hibernate's <c>@Length</c> for a string) for a length or a
/// count; <c>@Pattern(regexp, message)</c>; <c>@Email</c> and Hibernate's <c>@URL</c>; <c>@Min</c>, <c>@Max</c>,
/// <c>@DecimalMin</c> and <c>@DecimalMax</c> (with <c>inclusive</c>), <c>@Positive</c>, <c>@PositiveOrZero</c>,
/// <c>@Negative</c>, <c>@NegativeOrZero</c> and Hibernate's <c>@Range</c> for a numeric limit. An annotation counts only when
/// its file imports it from a Bean Validation package, so JetBrains' or Lombok's <c>@NotNull</c> isn't read as one; a file
/// that imports none isn't read, and its claims are left to another reader or can't be verified.
/// </summary>
internal sealed class BeanValidationClaims : IClaimReader
{
    private const string Framework = "Bean Validation";

    private static readonly string[] Standard = ["jakarta.validation.constraints", "javax.validation.constraints"];

    private const string Hibernate = "org.hibernate.validator.constraints";

    public IReadOnlyCollection<string> Kinds => Constraint.Kinds;

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Member is not { } member
            || context.Code.Files.FirstOrDefault(x => x.Path == member.At.File) is not { Language: JavaLanguage.LanguageName } file)
        {
            return null;
        }

        var imports = context.Code.ImportsOf(file).Where(x => !x.Static && x.Alias is null).ToList();
        if (!imports.Any(x => Standard.Concat([Hibernate]).Any(p => x.Target == p || x.Target.StartsWith(p + ".", StringComparison.Ordinal))))
        {
            return null;
        }

        // An annotation of the name the file imports from the package, by itself or with the whole package.
        AttributeNode? Find(string name, params string[] packages) =>
            member.Attributes.FirstOrDefault(a => a.Name == name
                                                  && imports.Any(i => packages.Any(p => i.Wildcard ? i.Target == p : i.Target == $"{p}.{name}")));
        AttributeNode? Standards(params string[] names) => names.Select(x => Find(x, Standard)).FirstOrDefault(x => x is not null);

        var owner = Constraint.Declaring(context, member);
        var collection = context.Element.TypeReference?.IsCollection == true;
        switch (context.Kind)
        {
            case Constraint.Required:
                return Marker(member, Standards("NotNull", "NotBlank", "NotEmpty"));
            case Constraint.Email:
                return Marker(member, Find("Email", [.. Standard, Hibernate]));
            case Constraint.Url:
                return Marker(member, Find("URL", Hibernate));
            case Constraint.Base64:
                return ClaimReading.CantVerify("unresolved", $"Bean Validation has no Base64 constraint, so `{member.Name}`'s annotations can't state one.", member.At);
            case Constraint.MinLength or Constraint.MaxLength when collection:
                return Constraint.PerItem(member, Framework);
            case Constraint.MinLength or Constraint.MaxLength or Constraint.MinItems or Constraint.MaxItems:
            {
                var minimum = context.Kind is Constraint.MinLength or Constraint.MinItems;
                var size = Standards("Size") ?? (context.Kind is Constraint.MinLength or Constraint.MaxLength ? Find("Length", Hibernate) : null);
                // @Size's min defaults to 0 and its max to Integer.MAX_VALUE: no maximum.
                return size is null
                    ? ClaimReading.Absent(member.At, minimum ? "0" : null)
                    : Constraint.Limit(context, size, Named(size, minimum ? "min" : "max"), owner, minimum ? "0" : null);
            }
            case Constraint.Pattern:
            {
                if (Standards("Pattern") is not { } pattern || Named(pattern, "regexp") is not { } regexp)
                {
                    return ClaimReading.Absent(member.At);
                }

                return Constraint.Text(context, regexp, owner) is { } value ? ClaimReading.Stated(value, pattern.At) : Constraint.Unreadable(pattern, regexp.Text);
            }
            case Constraint.PatternMessage:
            {
                if (Standards("Pattern") is not { } pattern)
                {
                    return ClaimReading.Absent(member.At);
                }

                if (Named(pattern, "message") is not { } message)
                {
                    return ClaimReading.Absent(pattern.At);
                }

                var value = Constraint.Text(context, message, owner);
                // {key} is a message the validator looks up in ValidationMessages.properties, which isn't code the rule reads.
                return value is null ? Constraint.Unreadable(pattern, message.Text)
                    : value.StartsWith('{') && value.EndsWith('}') ? ClaimReading.CantVerify("unreadable", $"`@Pattern` on `{member.Name}` takes its message from the key `{value}`, which isn't code the rule reads.", pattern.At)
                    : ClaimReading.Stated(value, pattern.At);
            }
            case Constraint.MinValue:
                return Bound(context, owner, member, Standards("Min"), Standards("DecimalMin"), Standards("Positive"), Standards("PositiveOrZero"), Find("Range", Hibernate), "min");
            case Constraint.MaxValue:
                return Bound(context, owner, member, Standards("Max"), Standards("DecimalMax"), Standards("Negative"), Standards("NegativeOrZero"), Find("Range", Hibernate), "max");
            case Constraint.MinExclusive:
                return Exclusive(context, owner, member, Standards("DecimalMin"), Standards("Positive"), Standards("Min") ?? Standards("PositiveOrZero") ?? Find("Range", Hibernate));
            case Constraint.MaxExclusive:
                return Exclusive(context, owner, member, Standards("DecimalMax"), Standards("Negative"), Standards("Max") ?? Standards("NegativeOrZero") ?? Find("Range", Hibernate));
            default:
                return null;
        }
    }

    /// <summary>An annotation's element by name; <c>value</c> is also an annotation's one unnamed argument (<c>@Min(1)</c>).</summary>
    private static AttributeValueNode? Named(AttributeNode annotation, string name) =>
        annotation.Values.FirstOrDefault(x => x.Name == name) ?? (name == "value" ? annotation.Values.FirstOrDefault(x => x.Name is null) : null);

    private static ClaimReading Marker(MemberNode member, AttributeNode? annotation) =>
        annotation is not null ? ClaimReading.Stated("true", annotation.At) : ClaimReading.Absent(member.At, "false");

    /// <summary>
    /// A numeric bound: <c>@Min(n)</c>/<c>@Max(n)</c>, <c>@DecimalMin("n")</c>/<c>@DecimalMax("n")</c>, a sign's 0
    /// (<c>@Positive</c>, <c>@PositiveOrZero</c>, <c>@Negative</c>, <c>@NegativeOrZero</c>), or Hibernate's <c>@Range</c>, whose
    /// <c>min</c> defaults to 0 and <c>max</c> to <c>Long.MAX_VALUE</c>, no maximum.
    /// </summary>
    private static ClaimReading Bound(ClaimContext context, TypeNode? owner, MemberNode member, AttributeNode? whole, AttributeNode? decimalBound,
        AttributeNode? strict, AttributeNode? orZero, AttributeNode? range, string side)
    {
        if ((whole ?? decimalBound) is { } bound)
        {
            return Constraint.Limit(context, bound, Named(bound, "value"), owner);
        }

        if ((strict ?? orZero) is { } sign)
        {
            return ClaimReading.Stated("0", sign.At);
        }

        return range is null ? ClaimReading.Absent(member.At) : Constraint.Limit(context, range, Named(range, side), owner, side == "min" ? "0" : null);
    }

    /// <summary>
    /// Whether a bound is exclusive: <c>@DecimalMin</c>/<c>@DecimalMax</c> with <c>inclusive = false</c>, or <c>@Positive</c>/<c>@Negative</c>;
    /// the inclusive forms state <c>false</c>, as silence does.
    /// </summary>
    private static ClaimReading Exclusive(ClaimContext context, TypeNode? owner, MemberNode member, AttributeNode? decimalBound, AttributeNode? strict, AttributeNode? inclusive)
    {
        if (decimalBound is not null)
        {
            if (Named(decimalBound, "inclusive") is not { } written)
            {
                return ClaimReading.Absent(decimalBound.At, "false");
            }

            var value = written.Literal ?? (owner is null ? null : context.Code.LiteralOf(written.Text, owner)?.Value);
            return value switch
            {
                "false" => ClaimReading.Stated("true", decimalBound.At),
                "true" => ClaimReading.Stated("false", decimalBound.At),
                _ => Constraint.Unreadable(decimalBound, written.Text)
            };
        }

        if (strict is not null)
        {
            return ClaimReading.Stated("true", strict.At);
        }

        return inclusive is not null ? ClaimReading.Stated("false", inclusive.At) : ClaimReading.Absent(member.At, "false");
    }
}
