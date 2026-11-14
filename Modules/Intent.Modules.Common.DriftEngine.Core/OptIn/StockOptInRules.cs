using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Scope;

namespace Intent.Modules.Common.DriftEngine.OptIn;

/// <summary><c>unmodelled.file</c>: an in-scope file in which no claim located anything (rules.md, Opt-in rules).</summary>
internal sealed class UnmodelledFile : IOptInRule
{
    public string Id => "unmodelled.file";

    public IEnumerable<UnmodelledCode> Find(RuleContext context) => context.Code.Files
        .Where(x => context.Scope.Checks(x.Path) && !context.IsLocated(x.Path))
        .Select(x => new UnmodelledCode(null, new Span(x.Path, 1), x.Path,
            $"`{x.Path}` is in scope, but the model describes none of its code: no claim located anything in it. Model what it holds, or add an exception with a reason to the Root Folder's Drift Check."));
}

/// <summary>
/// <c>unmodelled.type</c>: an in-scope type that follows an element type's convention, by the convention's <c>find</c> and
/// <c>files</c>, and that no claim located. A stock pack's conventions have no <c>find</c>, so a house pack says what its
/// code's element types look like.
/// </summary>
internal sealed class UnmodelledType : IOptInRule
{
    public string Id => "unmodelled.type";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var types = context.Code.Types.Where(x => context.Scope.Checks(x.At.File)).ToList();
        var reported = new HashSet<TypeNode>();
        foreach (var (designer, elementType, convention) in context.Engine.TypeConventions())
        {
            if (convention["find"]?.GetValue<string>() is not { } find)
            {
                continue;
            }

            var files = Globs(convention["files"]);
            var kinds = (convention["typeKinds"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToHashSet();
            foreach (var type in FindPath.Run(find, types, context.Code).Select(x => x.Type).Distinct())
            {
                if ((files.Count == 0 || files.Any(x => x.IsMatch(type.At.File))) && (kinds is null || kinds.Contains(type.Kind)) && !context.IsLocated(type) && reported.Add(type))
                {
                    yield return new UnmodelledCode(designer, type.At, type.Name,
                        $"`{type.Name}` follows the {designer} designer's {elementType} convention (`{find}`), but no {elementType} in the model describes it. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                        Type: type.Name);
                }
            }
        }
    }

    internal static IReadOnlyList<GlobPattern> Globs(JsonNode? files) => files switch
    {
        JsonArray list => list.Select(x => new GlobPattern(x!.GetValue<string>())).ToList(),
        JsonValue value => [new GlobPattern(value.GetValue<string>())],
        _ => []
    };
}

/// <summary>
/// <c>unmodelled.member</c>: inside a type an element's <c>type.exists</c> claim located, a public member of a kind its
/// element type can state (its convention's <c>states</c>) that no claim located; and whatever a designer's <c>members</c>
/// conventions find inside located types. Overrides, the members every type has, and a record's primary constructor (its
/// declaration) are never reported; nor, where the language profile names <c>accessors</c>, a located field's accessors.
/// </summary>
internal sealed class UnmodelledMember : IOptInRule
{
    private static readonly HashSet<string> EveryTypeHas = ["Equals", "GetHashCode", "ToString", "equals", "hashCode", "toString"];

    public string Id => "unmodelled.member";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var engine = context.Engine;
        var reported = new HashSet<object>();
        foreach (var (claim, type) in engine.LocatedTypes())
        {
            var element = claim.Source;
            var states = engine.States(claim.Designer, element);
            if (states.Count == 0)
            {
                continue;
            }

            var accessors = (engine.ProfileOf(type)["accessors"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToList() ?? [];
            var fields = type.Members.Where(x => x.Kind is "field" or "property" && context.IsLocated(x)).Select(x => x.Name).ToList();
            var located = fields.SelectMany(field => accessors.Select(x => Naming.Fill(x, new Dictionary<string, string> { ["Name"] = field }))).ToHashSet(StringComparer.Ordinal);

            // An override of a modelled base type's member is that type's; one of a framework's base (a hosted service's
            // StopAsync, a gRPC service's methods) is the type's own operation.
            var modelledBase = type.BaseTypes.Any(b => context.Code.Types.Any(x => x.Name == b.Name && context.ElementOf(x) is not null));
            foreach (var member in type.Members)
            {
                if (!states.Contains(member.Kind) || member.Access != "public" || modelledBase && member.Modifiers.Contains("override") || EveryTypeHas.Contains(member.Name) ||
                    member is { Kind: "constructor", IsPrimary: true } && type.Kind == "record" || member.Kind == "method" && located.Contains(member.Name) ||
                    context.IsLocated(member) || !reported.Add(member))
                {
                    continue;
                }

                var what = member.Kind == "enumMember" ? "enum member" : member.Kind;
                yield return new UnmodelledCode(claim.Designer, member.At, $"{type.Name}.{member.Name}",
                    $"`{type.Name}.{member.Name}` is a public {what} of `{type.Name}`, which the {claim.Designer} {element.SpecializationType} `{element.Name}` describes, but the model doesn't state it. Add it to `{element.Name}`, or add an exception with a reason to the Root Folder's Drift Check.",
                    element.Id, type.Name);
            }
        }

        // A designer's members conventions widen the rule beyond an element type's own public members.
        var types = context.Code.Types.Where(x => context.Scope.Checks(x.At.File) && context.IsLocated(x)).ToList();
        foreach (var (designer, convention) in engine.CodeConventions("members"))
        {
            var files = UnmodelledType.Globs(convention["files"]);
            var kind = convention["kind"]?.GetValue<string>() ?? "member";
            foreach (var hit in FindPaths(convention["find"]).SelectMany(x => FindPath.Run(x, types, context.Code)))
            {
                // What the path ends at: a member, or a place inside one, such as a bound parameter.
                if (hit.Member is null || (files.Count > 0 && !files.Any(x => x.IsMatch(hit.At.File))) || !reported.Add(hit.Node) ||
                    (hit.Node is MemberNode member ? context.IsLocated(member) : context.IsLocated(hit.At)))
                {
                    continue;
                }

                var name = hit.Name(convention["name"]?.GetValue<string>());
                var owner = context.ElementOf(hit.Type);
                yield return new UnmodelledCode(owner?.Designer ?? designer, hit.At, name,
                    $"`{name}` is a {kind} of `{hit.Type.Name}` the model doesn't state. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                    owner?.Element.Id, hit.Type.Name);
            }
        }
    }

    private static IEnumerable<string> FindPaths(JsonNode? find) => find switch
    {
        JsonArray list => list.Select(x => x!.GetValue<string>()),
        JsonValue value => [value.GetValue<string>()],
        _ => []
    };
}
