using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;

namespace Intent.Modules.Common.DriftEngine.Stacks.Interactions;

/// <summary>
/// What an operation, command, query or handler does, as the Services designer links it: what it invokes
/// (<c>intent.invokes</c>, a Perform Invocation) and which entities it creates, updates, deletes or queries (the Entity
/// Actions). The Services vocabulary claims each link; this reads it from the code the source runs, the code's own methods it
/// calls included, in any language and with any data access: a <c>DbSet&lt;T&gt;</c>, a repository of the entity, a
/// generic repository's <c>Find&lt;T&gt;()</c>.
/// </summary>
public sealed class InteractionsExtension : IDriftExtension
{
    public string Id => "intent.interactions";

    public IEnumerable<IClaimReader> ClaimReaders => [new InvocationReader(), new EntityActionReader()];
}

/// <summary>The code a link's source runs: its own methods, and every method of the code they call, transitively.</summary>
internal static class Reach
{
    private sealed class Cache
    {
        public Dictionary<IElement, IReadOnlyList<(TypeNode Type, MemberNode Member)>> Sources { get; } = new(ReferenceEqualityComparer.Instance);
    }

    /// <summary>The source of a link's target end: an operation, a command or query, or a handler.</summary>
    public static IElement? Source(ClaimContext context, IAssociationEnd end) => context.Owner(end);

    /// <summary>The source's own methods (<see cref="MessageCode.Units"/>), then what they reach, nearest first.</summary>
    public static IReadOnlyList<(TypeNode Type, MemberNode Member)> From(ClaimContext context, IElement source, out int own)
    {
        var units = MessageCode.Units(context.Code, source, context.MemberOf, context.TypeOf, context.Owner);
        own = units.Count;
        if (context.Code.Get<Cache>() is not { } cache)
        {
            context.Code.Set(cache = new Cache());
        }

        if (cache.Sources.TryGetValue(source, out var reached))
        {
            return reached;
        }

        var all = new List<(TypeNode Type, MemberNode Member)>();
        var seen = new HashSet<MemberNode>();
        var queue = new Queue<(TypeNode Type, MemberNode Member)>(units.Where(x => seen.Add(x.Member)));
        while (queue.TryDequeue(out var current))
        {
            all.Add(current);
            foreach (var call in current.Member.Calls)
            {
                foreach (var body in MessageCode.Bodies(context.Code, current.Type, call).Where(x => seen.Add(x.Member)))
                {
                    queue.Enqueue(body);
                }
            }
        }

        cache.Sources[source] = all;
        return all;
    }

    /// <summary>Where a finding says the code was read: the source's own method, or its handlers.</summary>
    public static string Where(IReadOnlyList<(TypeNode Type, MemberNode Member)> units, int own) =>
        own == 1 ? $"`{MessageCode.Display(units[0].Type, units[0].Member)}` ({units[0].Member.At.File})"
            : $"any of {string.Join(", ", units.Take(own).Select(x => $"`{MessageCode.Display(x.Type, x.Member)}`"))}";

    /// <summary>A model element as a finding names its source.</summary>
    public static string Describe(ClaimContext context, IElement source) => source switch
    {
        IAssociationEnd => MessageCode.Describe(source),
        { SpecializationType: "Operation" } when context.Owner(source) is { } owner => $"`{owner.Name}.{source.Name}`",
        _ => $"`{source.Name}`"
    };

    /// <summary>What an element's existence claim located, in this claim's designer or another.</summary>
    public static T? Located<T>(ClaimContext context, IElement element, string kind) where T : class =>
        context.LocatedOf(element, kind) as T
        ?? context.LocatedOf(element, kind, "Domain") as T
        ?? context.LocatedOf(element, kind, "Services") as T;

    /// <summary>
    /// The value names the type: it creates one, is one, or is given one, or a wrapper over one (<c>IdentifiedCommand&lt;T, R&gt;</c>);
    /// or, when <paramref name="name"/> is given, the expression it is given names it as a type argument, as a load through a
    /// generic repository does (<c>Find&lt;Order&gt;(...).FirstOrDefault()</c>), whose result's type the code doesn't declare.
    /// </summary>
    public static bool Carries(ValueNode value, Func<TypeRefNode, bool> isType, string? name = null, int depth = 0) =>
        value.Creations.Any(x => Over(x.Type, isType))
        || value.Parameter?.Type is { } parameter && Over(parameter, isType)
        || value.Types.Any(x => Over(x, isType))
        || name is not null && System.Text.RegularExpressions.Regex.IsMatch(value.Text, $@"<\s*{System.Text.RegularExpressions.Regex.Escape(name)}\s*[,>]")
        || depth < 3 && value.Given.Any(x => Carries(x, isType, name, depth + 1));

    /// <summary>A name's words, lower case: <c>BulkInsert</c> is <c>bulk</c>, <c>insert</c>; <c>save_all</c> is <c>save</c>, <c>all</c>.</summary>
    public static IReadOnlyList<string> Words(string name) =>
        System.Text.RegularExpressions.Regex.Matches(name, "[A-Z]?[a-z0-9]+|[A-Z]+(?![a-z])").Select(x => x.Value.ToLowerInvariant()).ToList();

    /// <summary>The type is the one, or names it as a type argument.</summary>
    public static bool Over(TypeRefNode type, Func<TypeRefNode, bool> isType) => isType(type) || type.Arguments.Any(x => Over(x, isType));
}

/// <summary>
/// A Perform Invocation: the source's code calls the target operation (on its type, an interface of it, or an implementation),
/// or hands on the target command or query, created, given or wrapped.
/// </summary>
internal sealed class InvocationReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.invokes"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is not IAssociationEnd { TypeReference.Element: IElement target } end || Reach.Source(context, end) is not { } source)
        {
            return null;
        }

        var reached = Reach.From(context, source, out var own);
        if (own == 0)
        {
            return ClaimReading.CantVerify("notFound", $"No method of the code is {Reach.Describe(context, source)}, so what it invokes can't be read.");
        }

        if (target.SpecializationType is "Command" or "Query")
        {
            var request = Reach.Located<TypeNode>(context, target, "type.exists");
            foreach (var (type, member) in reached)
            {
                bool IsRequest(TypeRefNode reference) => context.Code.TypeFor(reference, type) is { } resolved
                    ? request is null ? resolved.Name == target.Name : resolved == request
                    : reference.Name == target.Name;
                if (member.Calls.FirstOrDefault(call => call.Arguments.Any(x => Reach.Carries(x, IsRequest))) is { } send)
                {
                    return ClaimReading.Compared("present", send.At, true);
                }
            }

            return Absent(context, source, target, reached, own, $"no `{target.Name}` is handed on");
        }

        var owner = context.Owner(target);
        var method = Reach.Located<MemberNode>(context, target, "member.exists");
        var declaring = owner is null ? null : Reach.Located<TypeNode>(context, owner, "type.exists");
        var name = method?.Name ?? target.Name;
        foreach (var (type, member) in reached)
        {
            foreach (var call in member.Calls.Where(x => x.Name == name))
            {
                if (method is not null && (call.Target?.Member == method
                                           || context.Code.Targets(call.Name, call.Receiver, call.ReceiverType, call.Arguments.Count, type).Any(x => x.Member == method))
                    || OnTarget(context.Code, call, type, declaring, owner?.Name))
                {
                    return ClaimReading.Compared("present", call.At, true);
                }
            }
        }

        return Absent(context, source, target, reached, own, $"no call of `{owner?.Name}.{target.Name}` is made");
    }

    /// <summary>
    /// The call is made on the target's type: its class, an interface it implements or a class implementing it, or, called
    /// unqualified, the caller's own type or a base of it. By name when the target's type isn't located.
    /// </summary>
    private static bool OnTarget(CodeGraph code, CallNode call, TypeNode from, TypeNode? declaring, string? owner)
    {
        if (call.ReceiverType is null)
        {
            return call.Receiver is null or "this" or "base" or "super"
                ? declaring is not null && (from == declaring || code.Bases(from).Contains(declaring))
                : call.Receiver == owner || declaring is not null && call.Receiver == declaring.Name;
        }

        if (code.TypeFor(call.ReceiverType, from) is { } receiver && declaring is not null)
        {
            return receiver == declaring || code.Bases(declaring).Contains(receiver) || code.Bases(receiver).Contains(declaring);
        }

        return owner is not null && (call.ReceiverType.Name == owner || call.ReceiverType.Name == $"I{owner}")
               || declaring is not null && (call.ReceiverType.Name == declaring.Name || call.ReceiverType.Name == $"I{declaring.Name}");
    }

    private static ClaimReading Absent(ClaimContext context, IElement source, IElement target, IReadOnlyList<(TypeNode Type, MemberNode Member)> reached, int own, string what) =>
        ClaimReading.Compared("absent", reached[0].Member.At, false,
            $"{Reach.Describe(context, source)} invokes `{(context.Owner(target) is { SpecializationType: not ("Folder") } owner ? $"{owner.Name}." : "")}{target.Name}` in the model, but it isn't in the code: {what} in {Reach.Where(reached, own)} or the code's own methods it calls. Remove the Perform Invocation if the code is right.");
}

/// <summary>
/// An Entity Action. The code the source runs creates the entity (<c>new</c> it, or add or insert one), deletes it
/// (<c>Delete…</c> or <c>Remove…</c> of it), queries it (any other data access that names it), or updates it (an update or
/// save of it, or a load of it then a change, by a method of its own or by setting its properties; a save alone isn't one). Data access names the entity by a type argument
/// (<c>Find&lt;Order&gt;()</c>, <c>Set&lt;Order&gt;()</c>), by what it is made on (a <c>DbSet&lt;Order&gt;</c>, a repository
/// whose bases name <c>Order</c>, an <c>OrderRepository</c>), or by what it is given (an <c>Order</c>).
/// </summary>
internal sealed class EntityActionReader : IClaimReader
{
    private static readonly string[] DeleteVerbs = ["delete", "remove"];
    private static readonly string[] CreateVerbs = ["add", "insert", "create", "persist"];
    private static readonly string[] UpdateVerbs = ["update", "save", "upsert", "attach", "merge", "modify", "patch", "replace", "set"];

    private static readonly IReadOnlyDictionary<string, (string Verb, string Link)> Actions = new Dictionary<string, (string, string)>
    {
        ["intent.creates"] = ("creates", "Create Entity Action"),
        ["intent.updates"] = ("updates", "Update Entity Action"),
        ["intent.deletes"] = ("deletes", "Delete Entity Action"),
        ["intent.queries"] = ("queries", "Query Entity Action")
    };

    public IReadOnlyCollection<string> Kinds { get; } = Actions.Keys.ToList();

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is not IAssociationEnd { TypeReference.Element: IElement target } end || Reach.Source(context, end) is not { } source)
        {
            return null;
        }

        // A link to a constructor or operation acts on its class.
        var entity = target.SpecializationType == "Class" ? target : context.Owner(target) ?? target;
        var (verb, link) = Actions[context.Kind];
        var reached = Reach.From(context, source, out var own);
        if (own == 0)
        {
            return ClaimReading.CantVerify("notFound", $"No method of the code is {Reach.Describe(context, source)}, so what it {verb} can't be read.");
        }

        var located = Reach.Located<TypeNode>(context, entity, "type.exists");
        // A link to an operation states that it is the way: ShipOrderCommand updates Order by calling SetShippedStatus.
        var operation = target.SpecializationType == "Operation" ? target.Name : null;
        Span? created = null, deleted = null, updated = null, queried = null, changed = null, called = null;
        foreach (var (type, member) in reached)
        {
            // By name: a DbSet<Order> read from a caller resolves Order in the caller's namespace, which may hold a query's own
            // Order.
            bool IsEntity(TypeRefNode reference) => reference.Name == entity.Name
                                                   || context.Code.TypeFor(reference, type) is { } resolved && (resolved == located || resolved.Name == entity.Name);

            created ??= member.Creations.FirstOrDefault(x => IsEntity(x.Type))?.At;
            foreach (var call in WithLambdas(member.Calls))
            {
                if (call.Name == operation && (call.ReceiverType is null || IsEntity(call.ReceiverType)))
                {
                    called ??= call.At;
                }

                var words = Reach.Words(call.Name);
                var touch = Touches(context.Code, call, type, IsEntity, entity.Name);

                // Only data access names the entity by what it is made on; a call that is only given one, such as a
                // mapper's or a logger's, counts when it adds, changes or removes it: Insert(order), BulkInsert(orders).
                if (touch == Touch.Accessed || touch == Touch.Given && words.Any(x => DeleteVerbs.Contains(x) || CreateVerbs.Contains(x) && x != "create" || UpdateVerbs.Contains(x)))
                {
                    if (words.Any(DeleteVerbs.Contains))
                    {
                        deleted ??= call.At;
                    }
                    else if (words.Any(CreateVerbs.Contains))
                    {
                        created ??= call.At;
                    }
                    else if (words.Any(UpdateVerbs.Contains))
                    {
                        updated ??= call.At;
                    }
                    else
                    {
                        queried ??= call.At;
                    }
                }
                else if (words.FirstOrDefault() is not ("get" or "is" or "has" or "to" or "equals" or "find")
                         && (call.ReceiverType is { } receiver ? IsEntity(receiver) : call.Receiver is not null && OwnMethod(context.Code, located, call)))
                {
                    // A method of the entity itself changes it: order.Cancel(). Made on what the code doesn't type, a load's
                    // result (var customer = customers.findById(id).orElseThrow()) or a lambda's parameter (s -> s.receive(n)),
                    // it is the entity's when it can only be one of the entity's own methods.
                    changed ??= call.At;
                }
            }

            // Setting the entity's properties changes it, with nothing else to call: EF Core writes a tracked entity's
            // changes back when the unit of work commits (order.Status = command.Status). Set on what the code doesn't type, a
            // load's result or a loop's item, it is the entity's when the entity has a property or field of the name.
            foreach (var assignment in member.Body?.Assignments ?? [])
            {
                if (assignment.Receiver is { } receiver
                    && (receiver.Types.Any(IsEntity) || !receiver.IsKnown && OwnMember(context.Code, located, assignment.Target[(assignment.Target.LastIndexOf('.') + 1)..])))
                {
                    changed ??= assignment.At;
                }
            }
        }

        // A read the calls don't show, as of a cast's local (var root = (IQueryable<CatalogItem>)services.Context.CatalogItems):
        // the method names a property of the code's own that holds the entity, a DbSet<CatalogItem>.
        if (queried is null)
        {
            var members = MembersNamed(context.Code);
            queried = reached.FirstOrDefault(x => System.Text.RegularExpressions.Regex.Matches(x.Member.Text, @"\.\s*([A-Za-z_]\w*)")
                .Any(m => members.TryGetValue(m.Groups[1].Value, out var types) && types.Any(t => t.Arguments.Any(a => Reach.Over(a, r => r.Name == entity.Name))))).Member?.At;
        }

        // A load, then a change, writes it back. A save alone changes nothing: the one a handler reaches may be another
        // context's (an integration event log's), and a unit of work commits whatever is tracked.
        updated ??= queried is not null ? changed : null;
        var at = operation is not null ? called : context.Kind switch
        {
            "intent.creates" => created,
            "intent.deletes" => deleted,
            "intent.updates" => updated,
            _ => queried
        };
        if (at is { } found)
        {
            return ClaimReading.Compared("present", found, true);
        }

        var what = operation is not null ? $"no call of `{entity.Name}.{operation}` is made" : context.Kind switch
        {
            "intent.creates" => $"no `new {entity.Name}(...)`, and no add or insert of one, is made",
            "intent.deletes" => $"no delete or remove of `{entity.Name}` is made",
            "intent.updates" => queried is null ? $"`{entity.Name}` is neither updated nor saved, nor loaded and then changed" : $"`{entity.Name}` is loaded but never changed",
            _ => $"`{entity.Name}` is never read"
        };
        return ClaimReading.Compared("absent", reached[0].Member.At, false,
            $"{Reach.Describe(context, source)} {verb} `{entity.Name}` in the model, but it isn't in the code: {what} in {Reach.Where(reached, own)} or the code's own methods it calls. Remove the {link} if the code is right.");
    }

    private enum Touch
    {
        None,
        Given,
        Accessed
    }

    /// <summary>
    /// How the call names the entity: <see cref="Touch.Accessed"/> when a type argument names it or what it is made on is a
    /// collection or repository of it (its type, or a base of it, has the entity as a type argument); <see cref="Touch.Given"/>
    /// when it is only given one.
    /// </summary>
    private static Touch Touches(CodeGraph code, CallNode call, TypeNode from, Func<TypeRefNode, bool> isEntity, string entity)
    {
        if (call.TypeArguments.Any(x => Reach.Over(x, isEntity)))
        {
            return Touch.Accessed;
        }

        // What it is made on, when the code doesn't type it (services.Context.CatalogItems): the type of the member the chain
        // ends with, a DbSet<CatalogItem> property of the code's own; or what a method of the code's own returns, a collection
        // of the entity (wishlist.getItems().remove(item), an owned item's removal).
        var receiverType = call.ReceiverType;
        if (receiverType is null && call.Receiver?.Split('.') is { Length: > 1 } chain && MembersNamed(code).TryGetValue(chain[^1], out var types))
        {
            receiverType = types.FirstOrDefault(x => x.Arguments.Any(a => Reach.Over(a, isEntity)));
        }

        if (receiverType is null && call.On is { ReceiverType: null } on)
        {
            receiverType = code.Types.SelectMany(x => x.Members)
                .Where(x => x.Kind == "method" && x.Name == on.Name && x.Parameters.Count == on.Arguments.Count)
                .Select(x => x.Type)
                .FirstOrDefault(x => x is not null && x.Arguments.Any(a => Reach.Over(a, isEntity)));
        }

        if (receiverType is { } receiver)
        {
            if (isEntity(receiver))
            {
                return Touch.None;
            }

            if (receiver.Arguments.Any(x => Reach.Over(x, isEntity))
                || code.TypeFor(receiver, from) is { } type
                && new[] { type }.Concat(code.Bases(type)).Any(x => x.BaseTypes.Any(b => b.Arguments.Any(a => Reach.Over(a, isEntity)))))
            {
                return Touch.Accessed;
            }
        }

        return call.Arguments.Any(x => Reach.Carries(x, isEntity, entity)) ? Touch.Given : Touch.None;
    }

    /// <summary>The calls, and those of the lambdas they are given, which run as part of the method: change(id, s -> s.receive(n)).</summary>
    private static IEnumerable<CallNode> WithLambdas(IEnumerable<CallNode> calls)
    {
        foreach (var call in calls)
        {
            yield return call;
            foreach (var nested in WithLambdas(call.Arguments.Select(x => x.Function).OfType<FunctionNode>().SelectMany(x => x.Calls)))
            {
                yield return nested;
            }
        }
    }

    /// <summary>The call can only be one of the entity type's own methods, its bases' included: a method of its name taking its arguments.</summary>
    private static bool OwnMethod(CodeGraph code, TypeNode? entity, CallNode call) =>
        entity is not null && new[] { entity }.Concat(code.Bases(entity)).SelectMany(x => x.Members)
            .Any(x => x.Kind == "method" && x.Name == call.Name && x.Parameters.Count == call.Arguments.Count);

    /// <summary>The entity type, or a base of it, has a property or field of the name.</summary>
    private static bool OwnMember(CodeGraph code, TypeNode? entity, string name) =>
        entity is not null && new[] { entity }.Concat(code.Bases(entity)).SelectMany(x => x.Members).Any(x => x.Kind is "property" or "field" && x.Name == name);

    private sealed class MemberTypes
    {
        public required Dictionary<string, List<TypeRefNode>> ByName { get; init; }
    }

    /// <summary>The types of the code's own properties and fields, by name, found once per run.</summary>
    private static Dictionary<string, List<TypeRefNode>> MembersNamed(CodeGraph code)
    {
        if (code.Get<MemberTypes>() is not { } known)
        {
            code.Set(known = new MemberTypes
            {
                ByName = code.Types.SelectMany(x => x.Members)
                    .Where(x => x.Kind is "property" or "field" && x.Type is not null)
                    .GroupBy(x => x.Name)
                    .ToDictionary(x => x.Key, x => x.Select(m => m.Type!).ToList())
            });
        }

        return known.ByName;
    }
}
