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
            $"Nothing in the model describes the code in `{x.Path}`. Model what it holds, or add an exception with a reason to the Root Folder's Drift Check."));
}

/// <summary>
/// <c>unmodelled.type</c>: an in-scope type that no claim located. The Drift Check row's Glob says where the rule looks, so
/// the model alone sets it up (user decision, 2026-11-16). A module that wants more, such as only a base class's subclasses,
/// gives a convention a <c>find</c>: within its <c>files</c> only the types it finds are reported, as that element type.
/// A type nested in another is part of it, and what a file declares outside its types isn't a type.
/// </summary>
internal sealed class UnmodelledType : IOptInRule
{
    public string Id => "unmodelled.type";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var types = context.Code.Types.Where(x => context.Scope.Checks(x.At.File) && x.Kind != "module").ToList();
        var patterns = context.Engine.TypeConventions()
            .Where(x => x.Convention["find"] is not null)
            .Select(x => new Pattern(x.Designer, x.ElementType, Globs(x.Convention["files"]),
                FindPath.Run(x.Convention["find"]!.GetValue<string>(), types, context.Code).Select(h => h.Type)
                    .Where(t => (x.Convention["typeKinds"] as JsonArray)?.Any(k => k!.GetValue<string>() == t.Kind) ?? true)
                    .ToHashSet()))
            .ToList();
        // Every part of a partial type encloses what is nested in it: a ClientB handler declares a nested enum in its
        // second part, which isn't where the type is said to be (2026-11-24).
        var byFile = context.Code.Types.SelectMany(x => x.Declarations.Select(at => (Type: x, At: at))).ToLookup(x => x.At.File, StringComparer.Ordinal);
        var designers = context.Engine.LocatedTypes()
            .GroupBy(x => Folder(x.Type.At.File)!, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.GroupBy(c => c.Claim.Designer).OrderByDescending(c => c.Count()).First().Key, StringComparer.Ordinal);
        foreach (var type in types)
        {
            if (context.IsLocated(type) || byFile[type.At.File].Any(x => x.Type != type && Encloses(x.At, type.At)))
            {
                continue;
            }

            var applying = patterns.Where(x => x.Files.Count == 0 || x.Files.Any(g => g.IsMatch(type.At.File))).ToList();
            if (applying.Count == 0)
            {
                var designer = DesignerNear(designers, type.At.File);
                yield return new UnmodelledCode(designer, type.At, type.Name,
                    $"`{type.Name}` isn't in the model. Model it{(designer is null ? "" : $" in the {designer} designer")}, or add an exception with a reason to the Root Folder's Drift Check.",
                    Type: type.Name);
            }
            else if (applying.FirstOrDefault(x => x.Found.Contains(type)) is { } pattern)
            {
                yield return new UnmodelledCode(pattern.Designer, type.At, type.Name,
                    $"`{type.Name}` isn't in the model: no {pattern.ElementType} in the {pattern.Designer} designer describes it. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                    Type: type.Name);
            }
        }
    }

    private sealed record Pattern(string Designer, string ElementType, IReadOnlyList<GlobPattern> Files, HashSet<TypeNode> Found);

    private static bool Encloses(Span outer, Span inner) =>
        outer.File == inner.File && (outer.Line < inner.Line || outer.Line == inner.Line && (outer.Column ?? 0) < (inner.Column ?? 0)) && inner.Line <= (outer.EndLine ?? outer.Line);

    /// <summary>
    /// The designer whose elements describe the types nearest the file, by the folders' located types: its folder's, else
    /// the closest folder's above it. A type of an entities folder is reported with the Domain designer's findings.
    /// </summary>
    private static string? DesignerNear(IReadOnlyDictionary<string, string> designers, string file)
    {
        for (var folder = Folder(file); folder is not null; folder = folder.Length == 0 ? null : Folder(folder))
        {
            if (designers.TryGetValue(folder, out var designer))
            {
                return designer;
            }
        }

        return null;
    }

    private static string? Folder(string path) => path.LastIndexOf('/') is var i and >= 0 ? path[..i] : path.Length == 0 ? null : "";

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
/// declaration) are never reported; nor, where the language profile names <c>accessors</c>, a located field's accessors; nor
/// a member a framework reader knows no designer can state (<see cref="CodeGraph.Unmodellable"/>), or the element's type can't
/// (<see cref="CodeGraph.Unstatable"/>, a Proxy's method that sends no request), nor one a link states
/// (<see cref="CodeGraph.StatedByLink"/>, a Blazor component's injected service). Where the element type's convention says
/// its members are named uniquely (<c>uniqueNames</c>), only one of a type's overloads is asked for, and none when one is
/// located.
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
            var modelledBase = type.BaseTypes.Any(b => context.Code.TypesNamed(b.Name).Any(x => context.ElementOf(x) is not null));
            // Where the designer names members uniquely, of a type's overloads only one can be stated: none when one is
            // located, else the first.
            var uniqueNames = engine.UniqueNames(claim.Designer, element);
            var named = new HashSet<string>(type.Members.Where(x => x.Kind == "method" && context.IsLocated(x)).Select(x => x.Name), StringComparer.Ordinal);
            foreach (var member in type.Members)
            {
                if (!states.Contains(member.Kind) || member.Access != "public" || context.Code.StatedByLink.ContainsKey(member) || modelledBase && member.Modifiers.Contains("override") || EveryTypeHas.Contains(member.Name) ||
                    member is { Kind: "constructor", IsPrimary: true } && type.Kind == "record" || member.Kind == "method" && located.Contains(member.Name) ||
                    context.IsLocated(member) || context.Code.Unmodellable.ContainsKey(member) ||
                    uniqueNames && member.Kind == "method" && !named.Add(member.Name) ||
                    context.Code.Unstatable.Any(x => x(element.SpecializationType, type, member) is not null) || !reported.Add(member))
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

    internal static IEnumerable<string> FindPaths(JsonNode? find) => find switch
    {
        JsonArray list => list.Select(x => x!.GetValue<string>()),
        JsonValue value => [value.GetValue<string>()],
        _ => []
    };
}

/// <summary>
/// <c>unmodelled.interaction</c>: a call, or a markup element, that a designer's <c>interactions</c> conventions find in
/// checked code and that no <c>intent.*</c> claim located: a raise, send or call the model's links could state and don't.
/// Every checked type is looked in, not only located ones, since an interaction in a method the model doesn't have is as
/// unmodelled as one in a method it has.
/// </summary>
internal sealed class UnmodelledInteraction : IOptInRule
{
    public string Id => "unmodelled.interaction";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var types = context.Code.Types.Where(x => context.Scope.Checks(x.At.File)).ToList();
        // What the model's links located: the places their claims read.
        var linked = context.Claims.Where(x => x.Kind.StartsWith("intent.", StringComparison.Ordinal) && x.Reading?.At is { File.Length: > 0 })
            .Select(x => x.Reading!.At!)
            .ToLookup(x => x.File, StringComparer.Ordinal);
        var reported = new HashSet<object>(ReferenceEqualityComparer.Instance);
        foreach (var (designer, convention) in context.Engine.CodeConventions("interactions"))
        {
            var files = UnmodelledType.Globs(convention["files"]);
            var kind = convention["kind"]?.GetValue<string>() ?? "interaction";
            foreach (var hit in UnmodelledMember.FindPaths(convention["find"]).SelectMany(x => FindPath.Run(x, types, context.Code)))
            {
                if ((files.Count > 0 && !files.Any(x => x.IsMatch(hit.At.File))) || linked[hit.At.File].Any(x => DriftEngine.Inside(hit.At, x)) || !reported.Add(hit.Node))
                {
                    continue;
                }

                var name = hit.Name(convention["name"]?.GetValue<string>());
                var owner = context.ElementOf(hit.Type);
                yield return new UnmodelledCode(owner?.Designer ?? designer, hit.At, name,
                    $"`{name}` is a {kind} the {designer} designer's links can state, but no link in the model states it. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                    owner?.Element.Id, hit.Type.Name);
            }
        }
    }
}

/// <summary>
/// <c>unmodelled.exposed</c>: an in-scope type an endpoint exposes that no element describes. A designer's <c>exposed</c>
/// conventions find where a type is exposed (a parameter, or a member, whose type it is); the rule follows that type to its
/// declaration, through type arguments and union members, then the types of its public properties in turn, and reports each
/// checked type on the way that no claim located. Library types, which aren't declared in scope, are passed through.
/// </summary>
internal sealed class UnmodelledExposed : IOptInRule
{
    public string Id => "unmodelled.exposed";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var types = context.Code.Types.Where(x => context.Scope.Checks(x.At.File)).ToList();
        var reported = new HashSet<TypeNode>();
        foreach (var (designer, convention) in context.Engine.CodeConventions("exposed"))
        {
            var files = UnmodelledType.Globs(convention["files"]);
            var kind = convention["kind"]?.GetValue<string>() ?? "exposed type";
            foreach (var hit in UnmodelledMember.FindPaths(convention["find"]).SelectMany(x => FindPath.Run(x, types, context.Code)))
            {
                if (files.Count > 0 && !files.Any(x => x.IsMatch(hit.At.File)))
                {
                    continue;
                }

                var by = hit.Node is ParameterNode parameter && hit.Member is { } member ? $"{hit.Type.Name}.{member.Name}({parameter.Name})" : hit.Name(null);
                foreach (var type in Exposed(context.Code, hit))
                {
                    if (!context.Scope.Checks(type.At.File) || context.IsLocated(type) || !reported.Add(type))
                    {
                        continue;
                    }

                    yield return new UnmodelledCode(designer, type.At, type.Name,
                        $"`{type.Name}` is exposed as the {kind} of `{by}`, but no element of the {designer} designer describes it. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                        Type: type.Name);
                }
            }
        }
    }

    /// <summary>The in-scope types a place exposes: its type, what that type is written of, and its public properties' types, transitively.</summary>
    private static IEnumerable<TypeNode> Exposed(CodeGraph code, FindHit hit)
    {
        var root = hit.Node switch { ParameterNode parameter => parameter.Type, MemberNode member => member.Type, _ => null };
        var seen = new HashSet<TypeNode>();
        var queue = new Queue<(TypeRefNode Type, TypeNode From)>();
        if (root is not null)
        {
            queue.Enqueue((root, hit.Type));
        }

        while (queue.TryDequeue(out var current))
        {
            foreach (var inner in current.Type.Arguments.Concat(current.Type.Union))
            {
                queue.Enqueue((inner, current.From));
            }

            if (Resolve(code, current.Type, current.From) is not { } type || !seen.Add(type))
            {
                continue;
            }

            yield return type;
            foreach (var property in code.MembersOf(type).Where(x => x.Kind is "property" or "field" && x.Access == "public" && x.Type is not null))
            {
                queue.Enqueue((property.Type!, type));
            }
        }
    }

    /// <summary>
    /// The in-scope type a reference names, as its file's imports and namespaces resolve it, else the only in-scope type of
    /// its name. A generic type parameter is none.
    /// </summary>
    private static TypeNode? Resolve(CodeGraph code, TypeRefNode type, TypeNode from) =>
        from.TypeParameters.Contains(type.Name) ? null
            : code.TypeFor(type, from) ?? (code.Types.Where(x => x.Name == type.Name).Take(2).ToList() is [var only] ? only : null);
}

/// <summary>
/// <c>unmodelled.value</c>: an explicit value the code states for a <c>whenSet</c> kind that the model leaves unset. For
/// each element a <c>whenSet</c> vocabulary entry selects (its <c>for</c>, ignoring <c>where</c> and <c>when</c>) with no
/// claim of that kind, the kind's rule reads the code as it would for a claim; a value read from the code is reported where
/// the code states it. Silence is never a value, so a framework's convention is never reported.
/// </summary>
internal sealed class UnmodelledValue : IOptInRule
{
    public string Id => "unmodelled.value";

    public IEnumerable<UnmodelledCode> Find(RuleContext context) => context.Engine.UnmodelledValues();
}
