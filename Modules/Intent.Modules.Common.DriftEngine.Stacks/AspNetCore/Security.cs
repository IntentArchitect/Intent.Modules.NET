using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// How ASP.NET Core secures an endpoint (<c>security</c>, <c>security.roles</c>, <c>security.policy</c>), from what each place
/// that can state it says, most specific first: a Minimal API endpoint's own call, its handler's attributes, its group's
/// call, its router's mapping call; or an action's attributes, then its controller's and the controller's bases'. As ASP.NET
/// Core applies them, <c>AllowAnonymous</c> anywhere makes the endpoint anonymous (<c>Unsecured</c>); otherwise any
/// <c>RequireAuthorization</c> or <c>[Authorize]</c> secures it (<c>Secured</c>); with neither it is <c>None</c>, which the code
/// states by saying nothing. Roles and policies are the most specific place's that names any.
/// </summary>
internal static class Security
{
    public static readonly string[] Kinds = ["security", "security.roles", "security.policy"];

    public static ClaimReading Read(ClaimContext context, IReadOnlyList<Authorization> levels, TypeNode from, Span at)
    {
        switch (context.Kind)
        {
            case "security":
                if (levels.FirstOrDefault(x => x.Anonymous) is { } anonymous)
                {
                    return ClaimReading.Stated("Unsecured", anonymous.At);
                }

                return levels.FirstOrDefault() is { } secured ? ClaimReading.Stated("Secured", secured.At) : ClaimReading.Absent(at, "None");
            default:
                return Named(context, levels.Where(x => !x.Anonymous).ToList(), from, at, context.Kind == "security.roles");
        }
    }

    /// <summary>
    /// The roles or policies the most specific securing place names. An argument whose value the code doesn't give, or that
    /// may be a policy object (<c>RequireAuthorization(policy)</c>), is can't verify: it may require what the model states.
    /// </summary>
    private static ClaimReading Named(ClaimContext context, IReadOnlyList<Authorization> levels, TypeNode from, Span at, bool roles)
    {
        var what = roles ? "roles" : "policy";
        foreach (var level in levels)
        {
            var named = new List<string>();
            foreach (var value in roles ? level.Roles : level.Policies)
            {
                if ((value.Literal ?? context.Code.ConstantOf(value.Written, from)) is not { } text)
                {
                    return ClaimReading.CantVerify("unreadable", $"The code can't read `{value.Written}`, the {what} `{Name(level)}` gives: it isn't a string constant of the in-scope code.", level.At);
                }

                named.Add(text);
            }

            // An argument that is neither a name nor an AuthorizeAttribute: a constant naming a policy, or a policy itself.
            foreach (var opaque in level.Opaque)
            {
                if (context.Code.ConstantOf(opaque, from) is { } policy)
                {
                    if (!roles)
                    {
                        named.Add(policy);
                    }

                    continue;
                }

                return ClaimReading.CantVerify("unreadable", $"The code can't read `{opaque}`, which `{Name(level)}` is given: it isn't a policy's name the in-scope code declares, so the {what} it requires aren't known.", level.At);
            }

            if (named.Count > 0)
            {
                return ClaimReading.Stated(string.Join(", ", named.SelectMany(x => x.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))), level.At);
            }

            // The code's own attribute requires what its class sets, as ASP.NET Core reads it from the instance: Roles or
            // Policy assigned in a constructor, or the policy handed to AuthorizeAttribute's. One that sets none requires none.
            if (level.Custom)
            {
                if (level.Class is null || SetBy(context, level.Class, roles) is not { } set)
                {
                    return ClaimReading.CantVerify("unreadable",
                        $"`[{level.Kind}]` is the code's own attribute deriving from `AuthorizeAttribute`, which may set the {what} itself, so the code doesn't say which it requires.", level.At);
                }

                if (set.Values.Count > 0)
                {
                    return ClaimReading.Stated(string.Join(", ", set.Values.SelectMany(x => x.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))), set.At!);
                }
            }
        }

        return ClaimReading.Absent(levels.FirstOrDefault()?.At ?? at);
    }

    private static string Name(Authorization level) => level.Kind is "RequireAuthorization" or "AllowAnonymous" ? level.Kind : $"[{level.Kind}]";

    private static readonly Regex Assigned = new(@"(?<![\w.])(?:this\.|base\.)?(?<name>Roles|Policy)=(?<value>@?""(?:[^""\\]|\\.)*""|[A-Za-z_][\w.]*)", RegexOptions.CultureInvariant);

    private static readonly Regex BasePolicy = new(@":base\((?<value>@?""(?:[^""\\]|\\.)*""|[A-Za-z_][\w.]*)\)", RegexOptions.CultureInvariant);

    /// <summary>
    /// The roles or policies an attribute class of the code's own sets, in its constructors and its in-scope bases':
    /// <c>Roles = "Admin"</c>, <c>Policy = Policies.Staff</c>, or <c>: base("Staff")</c>, AuthorizeAttribute's policy. Null when
    /// it sets one the code can't read, so what it requires isn't known.
    /// </summary>
    private static (List<string> Values, Span? At)? SetBy(ClaimContext context, TypeNode type, bool roles)
    {
        var values = new List<string>();
        Span? at = null;
        foreach (var constructor in new[] { type }.Concat(context.Code.Bases(type)).SelectMany(x => context.Code.MembersOf(x)).Where(x => x.Kind == "constructor"))
        {
            var written = Assigned.Matches(constructor.Text).Where(x => x.Groups["name"].Value == (roles ? "Roles" : "Policy")).Select(x => x.Groups["value"].Value)
                .Concat(roles ? [] : BasePolicy.Matches(constructor.Text).Select(x => x.Groups["value"].Value));
            foreach (var value in written)
            {
                if ((value.StartsWith('"') || value.StartsWith("@\"") ? Regex.Unescape(value.TrimStart('@')[1..^1]) : context.Code.ConstantOf(value, type)) is not { } text)
                {
                    return null;
                }

                values.Add(text);
                at ??= constructor.At;
            }
        }

        return (values, at);
    }

    /// <summary>
    /// A class's or method's authorization attributes as places that secure it: <c>[AllowAnonymous]</c>, and
    /// <c>[Authorize]</c> or an attribute of the code's own deriving from <c>AuthorizeAttribute</c>, with the roles and policy
    /// it names (<c>[Authorize("Policy")]</c>, <c>Roles = ...</c>, <c>Policy = ...</c>).
    /// </summary>
    public static IEnumerable<Authorization> Attributes(CodeGraph code, IEnumerable<AttributeNode> attributes, TypeNode from)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.Name == "AllowAnonymous")
            {
                yield return new Authorization(attribute.Name, attribute.At, [], [], []);
            }
            else if (attribute.Name == "Authorize" || ClassOf(code, attribute, from) is not null)
            {
                yield return new Authorization(attribute.Name, attribute.At,
                    attribute.Values.Where(x => x.Name == "Roles").Select(x => new WrittenValue(x.Literal, x.Text)).ToList(),
                    attribute.Values.Where(x => x.Name is null or "Policy").Select(x => new WrittenValue(x.Literal, x.Text)).ToList(),
                    []) { Custom = attribute.Name != "Authorize", Class = attribute.Name == "Authorize" ? null : ClassOf(code, attribute, from) };
            }
        }
    }

    /// <summary>A type's own attributes, then its in-scope bases', as ASP.NET Core inherits <c>[Authorize]</c>.</summary>
    public static IEnumerable<Authorization> OfType(CodeGraph code, TypeNode type) =>
        new[] { type }.Concat(code.Bases(type)).SelectMany(x => Attributes(code, x.Attributes, x));

    /// <summary>The attribute is a class of the code's own (<c>X</c> or <c>XAttribute</c>) that derives from <c>AuthorizeAttribute</c>.</summary>
    public static bool Authorizes(CodeGraph code, AttributeNode attribute, TypeNode from) => ClassOf(code, attribute, from) is not null;

    /// <summary>The attribute's class, when it is the code's own (<c>X</c> or <c>XAttribute</c>) and derives from <c>AuthorizeAttribute</c>.</summary>
    private static TypeNode? ClassOf(CodeGraph code, AttributeNode attribute, TypeNode from) =>
        new[] { $"{attribute.Name}Attribute", attribute.Name }
            .Select(x => code.TypeFor(new TypeRefNode { Text = x, Name = x, QualifiedName = x }, from))
            .FirstOrDefault(x => x is not null) is { } type
        && code.Supertypes(type).Any(x => x is "AuthorizeAttribute" or "Authorize")
            ? type
            : null;
}
