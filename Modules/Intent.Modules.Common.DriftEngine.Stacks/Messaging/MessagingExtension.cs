using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Messaging;

/// <summary>
/// Domain events raised, integration events published and integration commands sent, whatever does the raising (user
/// decision, 2026-11-14): code raises a message when it creates one and hands it to a call, through
/// <c>AddDomainEvent</c>, an outbox, a bus or a codebase's own wrapper alike. A link the model draws is a claim the code
/// must bear out (<c>intent.raises</c>, <c>intent.publishes</c>, <c>intent.sends</c>), and a message, or a raise of one,
/// that the diagram could show and doesn't is drift (<see cref="UnmodelledMessages"/>).
/// </summary>
public sealed class MessagingExtension : IDriftExtension
{
    public string Id => "intent.messaging";

    public IEnumerable<KindDefinition> Kinds => MessageLink.All.Select(x => new KindDefinition(x.Kind, x.Verb, ValueType: "presence", Comparer: "presence", Existence: true));

    public IEnumerable<IClaimSource> ClaimSources => [new MessageLinkSource(), new SubscribedCopySource()];

    public IEnumerable<IClaimReader> ClaimReaders => [new MessageLinkReader(), new SubscriptionReader(), new MessageCopyReader()];

    public IEnumerable<IDriftRule> Rules => [new UnmodelledMessages()];
}

/// <summary>A kind of link from code that runs to the message it raises.</summary>
/// <param name="Association">The association type the model draws it with.</param>
/// <param name="Message">The element type of what it links to.</param>
/// <param name="Link">What a finding calls the link: the designer's name for it.</param>
/// <param name="Verb">What the code does with the message: <c>raises</c>.</param>
/// <param name="Rule">The rule id of the finding for a message the code raises that no link covers.</param>
/// <param name="What">What a finding calls the message, in prose: <c>domain event</c>.</param>
/// <param name="Own">The word a call that does it is named with (<c>publish</c>), when buses share one.</param>
/// <param name="Other">The word of the link it isn't: a <c>Publish</c> call sends nothing.</param>
internal sealed record MessageLink(string Kind, string Designer, string Association, string Message, string Verb, string Rule, string Link, string What, string? Own = null, string? Other = null)
{
    public static IReadOnlyList<MessageLink> All { get; } =
    [
        new("intent.raises", "Domain", "Domain Event Origin Association", "Domain Event", "raises", "messaging/unmodelled.raise", "Domain Event Origin", "domain event"),
        new("intent.publishes", "Services", "Publish Integration Event", "Message", "publishes", "messaging/unmodelled.publish", "Publish Integration Event", "integration event", "publish", "send"),
        new("intent.sends", "Services", "Send Integration Command", "Integration Command", "sends", "messaging/unmodelled.send", "Send Integration Command", "integration command", "send", "publish")
    ];

    public static MessageLink Of(string kind) => All.Single(x => x.Kind == kind);
}

/// <summary>
/// Each link the model draws, as a claim on its target end: from a Domain constructor or operation; from a Services
/// operation or command; or from an event handler's subscription, which is an association end itself.
/// </summary>
internal sealed class MessageLinkSource : IClaimSource
{
    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        foreach (var link in MessageLink.All.Where(x => model.Designers.Contains(x.Designer)))
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var source in model.Elements(link.Designer).SelectMany(x => new[] { x }.Concat(Subscriptions(x))))
            {
                foreach (var target in model.Links(source, link.Association).Where(x => seen.Add(x.End.Id)))
                {
                    yield return new ClaimSpec
                    {
                        Designer = link.Designer,
                        Element = target.End,
                        Kind = link.Kind,
                        Expected = "present",
                        DependsOn = DependsOn(link.Designer, source),
                        Source = $"its {link.Link} link"
                    };
                }
            }
        }
    }

    /// <summary>A handler's subscriptions: the target ends of its own associations, which a publish or send may start from.</summary>
    private static IEnumerable<IElement> Subscriptions(IElement element) =>
        (element.AssociatedElements ?? []).Where(x => x.Association?.TargetEnd?.Id == x.Id);

    /// <summary>The source's existence claim, which locates the code it runs; a handler is found by what it takes instead.</summary>
    private static ClaimRef? DependsOn(string designer, IElement source) => source.SpecializationType switch
    {
        "Operation" or "Class Constructor" => new ClaimRef(designer, source.Id, "member.exists"),
        "Command" => new ClaimRef(designer, source.Id, "type.exists"),
        _ => null
    };
}

/// <summary>A handler's subscription to a message, which the Services vocabulary claims on the link.</summary>
internal static class Subscription
{
    public const string Kind = "subscription.handler";

    public const string Designer = "Services";

    public static readonly string[] Associations = ["Subscribe Integration Event", "Subscribe Integration Command"];
}

/// <summary>
/// A subscription to a message another application owns, a referenced package's, as a <c>message.copy</c> claim on the
/// subscription: the subscriber deserialises the message into a copy of its own, which only the handler names. A
/// subscription to this application's own message gives none, as the message is its own type.
/// </summary>
internal sealed class SubscribedCopySource : IClaimSource
{
    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Subscription.Designer))
        {
            yield break;
        }

        // Another application's message is one this application's designer doesn't hold: its link reaches a referenced package.
        var own = model.Elements(Subscription.Designer).Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var handler in model.Elements(Subscription.Designer))
        {
            foreach (var link in Subscription.Associations.SelectMany(x => model.Links(handler, x)).Where(x => !own.Contains(x.Target.Id)))
            {
                yield return new ClaimSpec
                {
                    Designer = Subscription.Designer,
                    Element = link.End,
                    Kind = "message.copy",
                    Expected = "present",
                    DependsOn = new ClaimRef(Subscription.Designer, handler.Id, "type.exists"),
                    Source = $"its {link.End.Association?.SpecializationType} link"
                };
            }
        }
    }
}

/// <summary>
/// The handler's class handles the message: it, or a class of the code it derives from, declares a base over it
/// (<c>IHandleMessages&lt;T&gt;</c>, <c>IIntegrationEventHandler&lt;T&gt;</c>, <c>INotificationHandler&lt;T&gt;</c>, whatever the
/// framework), or else has a method that takes it, as a convention-based framework's handler does.
/// </summary>
internal sealed class SubscriptionReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = [Subscription.Kind];

    public ClaimReading? Read(ClaimContext context)
    {
        // A handler whose class isn't found is its own existence claim's finding.
        if (context.Element is not IAssociationEnd { TypeReference.Element: IElement message } end || context.Owner(end) is not { } handler
            || context.Located<TypeNode>() is not { } type)
        {
            return null;
        }

        var designer = message.SpecializationType == "Domain Event" ? "Domain" : Subscription.Designer;
        var located = context.LocatedOf(message, "type.exists", designer) as TypeNode;
        // A service's own copy of a message it shares is the message too.
        bool Is(TypeRefNode reference, TypeNode from) =>
            context.Code.TypeFor(reference, from) is { } resolved ? resolved == located || resolved.Name == message.Name : reference.Name == message.Name;
        bool Over(TypeRefNode reference, TypeNode from) => reference.Arguments.Any(x => Is(x, from) || Over(x, from));

        // The methods that take the message are where it is handled (a framework's Handle, and the implementation method a
        // partial half hands it to), so they are the subscription's code, not unmodelled members.
        var methods = type.Members.Where(x => x.Kind == "method" && x.Parameters.Any(p => p.Type is { } t && (Is(t, type) || Over(t, type)))).ToList();
        if (methods.Count > 0)
        {
            return ClaimReading.Found(methods, methods[0].At);
        }

        foreach (var current in new[] { type }.Concat(context.Code.Bases(type)))
        {
            if (current.BaseTypes.Any(x => Over(x, current)))
            {
                return ClaimReading.Compared("present", type.At, true);
            }
        }

        var association = end.Association?.SpecializationType ?? "subscription";
        return ClaimReading.Compared("absent", type.At, false,
            $"The model says `{handler.Name}` handles `{message.Name}`, but the code doesn't: `{type.Name}` ({type.At.File}) neither implements a handler of `{message.Name}` nor has a method that takes one. Remove the {association} link if the code is right.");
    }
}

/// <summary>
/// A subscriber's copy of a message another service publishes (Integration Event Subscription, from the copy to the
/// published message): every property the copy has, the published message has, of the same type, so what is sent can be
/// read. The copy may leave properties out.
/// </summary>
internal sealed class MessageCopyReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["message.copy"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is IAssociationEnd { TypeReference.Element: IElement subscribed } subscription
            && Subscription.Associations.Contains(subscription.Association?.SpecializationType))
        {
            return Subscribed(context, subscribed);
        }

        if (context.Element is not IAssociationEnd { TypeReference.Element: IElement published } || context.Located<TypeNode>() is not { } copy)
        {
            return null;
        }

        // A published message that isn't found is its own existence claim's finding.
        if (context.LocatedOf(published, "type.exists") is not TypeNode source)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{published.Name}`'s type.exists");
        }

        var sent = Properties(context.Code, source).GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        var problems = new List<string>();
        foreach (var property in Properties(context.Code, copy))
        {
            if (!sent.TryGetValue(property.Name, out var original))
            {
                problems.Add($"`{property.Name}` isn't one of its properties");
            }
            else if (property.Type is { } type && original.Type is { } sentType && Shown(type) != Shown(sentType) && !ReadsEnum(context.Code, source, sentType, type))
            {
                problems.Add($"`{property.Name}` is `{type.Text}` in the copy and `{sentType.Text}` in it");
            }
        }

        return problems.Count == 0
            ? ClaimReading.Compared("present", copy.At, true)
            : ClaimReading.Compared("absent", copy.At, false,
                $"`{copy.Name}` ({copy.At.File}) is a copy of `{published.Name}` ({source.At.File}), but {string.Join("; ", problems)}. Make the copy's properties those of the message it copies, or remove the Integration Event Subscription link if it copies no message.");
    }

    /// <summary>
    /// A subscription to another application's message: the type the handler takes for it is the subscriber's copy, read
    /// against the message's modelled properties, as the publisher's code isn't this application's to read. Only the copy's
    /// own properties count; what it inherits is the bus's (an <c>IntegrationEvent</c>'s id and date).
    /// </summary>
    private static ClaimReading? Subscribed(ClaimContext context, IElement message)
    {
        if (context.Located<TypeNode>() is not { } handler)
        {
            return null;
        }

        static IEnumerable<TypeRefNode> Nested(TypeRefNode type) => new[] { type }.Concat(type.Arguments.SelectMany(Nested));
        var copy = new[] { handler }.Concat(context.Code.Bases(handler))
            .SelectMany(x => x.BaseTypes.Concat(x.Members.Where(m => m.Kind == "method").SelectMany(m => m.Parameters).Select(p => p.Type).OfType<TypeRefNode>()))
            .SelectMany(Nested)
            .Where(x => x.Name == message.Name)
            .Select(x => context.Code.TypeFor(x, handler))
            .OfType<TypeNode>()
            .FirstOrDefault();
        if (copy is null)
        {
            // A handler that takes no such type is its subscription's finding; one that takes a library's has no copy to read.
            return ClaimReading.CantVerify("blocked", $"`{handler.Name}` takes no type of the code named `{message.Name}`, so there is no copy to read.");
        }

        var modelled = message.ChildElements.Where(x => x.TypeReference?.Element is not null)
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First().TypeReference, StringComparer.OrdinalIgnoreCase);
        var problems = new List<string>();
        foreach (var property in copy.Members.Where(x => x.Kind == "property" && x.Access == "public" && !x.IsStatic))
        {
            if (!modelled.TryGetValue(property.Name, out var sent))
            {
                problems.Add($"`{property.Name}` isn't one of its modelled properties");
            }
            else if (property.Type is { } type && !context.TypeMatches(sent, type) && !(sent.Element is { SpecializationType: "Enum" } && NameOrNumber(type)))
            {
                problems.Add($"`{property.Name}` is `{type.Text}` in the copy and `{sent.Element!.Name}{(sent.IsCollection ? "[]" : "")}` in the model");
            }
        }

        return problems.Count == 0
            ? ClaimReading.Compared("present", copy.At, true)
            : ClaimReading.Compared("absent", copy.At, false,
                $"`{copy.Name}` ({copy.At.File}) is `{handler.Name}`'s copy of `{message.Name}`, which it subscribes to from another application, but {string.Join("; ", problems)}. Make the copy's properties those of the message, or model what the message sends.");
    }

    private static IEnumerable<MemberNode> Properties(CodeGraph code, TypeNode type) =>
        new[] { type }.Concat(code.Bases(type)).SelectMany(x => x.Members).Where(x => x.Kind == "property" && x.Access == "public" && !x.IsStatic);

    /// <summary>A type as compared: its text without nullability or whitespace.</summary>
    private static string Shown(TypeRefNode type) => type.Text.Replace(" ", "").TrimEnd('?');

    /// <summary>The message sends an enum, which the copy reads as its name or number: an enum is serialized as one or the other.</summary>
    private static bool ReadsEnum(CodeGraph code, TypeNode message, TypeRefNode sent, TypeRefNode copy) =>
        code.TypeFor(sent, message) is { Kind: "enum" } && NameOrNumber(copy);

    private static bool NameOrNumber(TypeRefNode type) => Shown(type) is "string" or "String" or "int" or "long" or "short" or "byte";
}

/// <summary>
/// Whether the code the link starts from raises its message: the method, or the code's own methods it calls, hands one on
/// (<see cref="MessageCode.HandsOn"/>), a <c>new</c> one or a value the code types as one (<see cref="MessageCode.MessagesOf"/>).
/// A command's code is the method that takes it; a handler subscription's is the handler's method that takes what it subscribes to.
/// </summary>
internal sealed class MessageLinkReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = MessageLink.All.Select(x => x.Kind).ToList();

    public ClaimReading? Read(ClaimContext context)
    {
        var link = MessageLink.Of(context.Kind);
        if (context.Element is not IAssociationEnd { TypeReference.Element: IElement message } end || context.Owner(end) is not { } source)
        {
            return null;
        }

        var units = MessageCode.Units(context.Code, source, context.MemberOf, context.TypeOf, context.Owner);
        if (units.Count == 0)
        {
            return ClaimReading.CantVerify("notFound", MessageCode.NotFound(context, source, link));
        }

        var type = MessageCode.TypeOf(context.Code, (designer, element) => context.LocatedOf(element, "type.exists", designer) as TypeNode, link, message);
        // A publish isn't a send, nor a send a publish, on any bus: NServiceBus's, MassTransit's, MediatR's, Wolverine's.
        var calls = units.SelectMany(x => MessageCode.Raising(context.Code, x.Type, x.Member, created => MessageCode.Is(context.Code, created, x.Type, type, message.Name)))
            .Where(x => link.Other is not { } other || !Interactions.Reach.Words(x.Call.Name).Contains(other))
            .ToList();
        MessageCode.Cover(context.Code, calls.Select(x => x.Call));
        if (calls.Count > 0)
        {
            // Where the member does it: the call itself, or its call of the method that does.
            return ClaimReading.Compared("present", calls[0].Root.At, true);
        }

        var unit = units[0];
        var where = units.Count == 1 ? $"`{MessageCode.Display(unit.Type, unit.Member)}`" : $"any of {string.Join(", ", units.Select(x => $"`{MessageCode.Display(x.Type, x.Member)}`"))}";
        // A message handed on that the code doesn't say, such as a library factory's, may be this message or another.
        var supertypes = type is not null ? context.Code.Supertypes(type) : [];
        if (units.SelectMany(x => x.Member.Calls.Select(call => (x.Type, x.Member, Call: call)))
                .FirstOrDefault(x => (link.Own is not { } own || Interactions.Reach.Words(x.Call.Name).Contains(own))
                    && x.Call.Arguments.Select((a, i) => (Value: a, Index: i))
                        .Any(a => MessageCode.Untyped(context.Code, x.Type, a.Value, supertypes) && MessageCode.HandsOn(context.Code, x.Type, x.Member, x.Call, a.Index, message.Name)))
                is { Call: { } unreadable })
        {
            return ClaimReading.CantVerify("unreadable",
                $"{where} {link.Verb} a message the code doesn't type, so it can't read whether it is `{message.Name}`.", unreadable.At);
        }

        return ClaimReading.Compared("absent", unit.Member.At, false,
            $"{MessageCode.Describe(source)} {link.Verb} `{message.Name}` in the model, but it isn't in the code: no `{message.Name}`, new or typed as one, is handed on in {where} or the code's own methods it calls ({unit.Member.At.File}). Remove the {link.Link} link if the code is right.");
    }
}

/// <summary>
/// The diagram showing the code's messages wherever it can (user decision, 2026-11-14: "if the diagram can't reflect it,
/// it's not drift; if the diagram could reflect it and they're not in sync, it's drift"). On by default, unlike the opt-in
/// rules for other code the model doesn't describe; a Severity Overrides row can lower them.
/// <list type="bullet">
/// <item>A message type in the code must be modelled (<c>messaging/unmodelled.message</c>).</item>
/// <item>
/// A raise, publish or send in the code needs its link wherever the diagram has somewhere to draw it from
/// (<c>messaging/unmodelled.raise</c>, <c>.publish</c>, <c>.send</c>): a modelled constructor, operation, command or handler
/// subscription whose code reaches it, helpers it calls included; a method of a modelled class or service, which the
/// diagram can show as an operation; a handler of a modelled message, which it can show as a subscription. A send from code
/// the diagram can't show, such as a background router, is no drift.
/// </item>
/// </list>
/// </summary>
internal sealed class UnmodelledMessages : IDriftRule
{
    private sealed record Send(TypeNode Type, MemberNode Member, CallNode Call, TypeNode Message, IElement? Modelled);

    public string Id => "messaging/unmodelled";

    public IEnumerable<Finding> Check(RuleContext context)
    {
        var covered = context.Code.Get<CoveredCalls>()?.Calls ?? [];
        var kinds = MessageLink.All
            .Where(x => context.Model.Designers.Contains(x.Designer))
            .Select(x => (Link: x, Messages: MessageCode.Messages(context, x)))
            .Where(x => x.Messages.Types.Count > 0)
            .ToList();
        var modelled = new Dictionary<TypeNode, IElement>(ReferenceEqualityComparer.Instance);
        foreach (var (type, element) in kinds.SelectMany(x => x.Messages.Types).Where(x => x.Value is not null))
        {
            modelled.TryAdd(type, element!);
        }

        // An unmodelled message a link's finding already asks to model isn't reported again on its own.
        var asked = new HashSet<TypeNode>(ReferenceEqualityComparer.Instance);
        foreach (var (link, messages) in kinds)
        {
            var sends = Sends(context, messages.Types, covered);
            if (sends.Count == 0)
            {
                continue;
            }

            var reach = Reach(context, link, new HashSet<CallNode>(sends.Select(x => x.Call), ReferenceEqualityComparer.Instance));
            foreach (var send in sends)
            {
                if (Fix(context, link, send, reach, modelled) is not { } fix)
                {
                    continue;
                }

                var name = send.Message.Name;
                var text = send.Modelled is null ? $"Model the {link.Message} `{name}`, then {char.ToLowerInvariant(fix.Text[0])}{fix.Text[1..]}" : fix.Text;
                if (send.Modelled is null)
                {
                    asked.Add(send.Message);
                }

                yield return new Finding(link.Rule, link.Designer, "ModelCode", "error",
                    $"`{MessageCode.Display(send.Type, send.Member)}` {link.Verb} `{name}` ({send.Call.At.File}:{send.Call.At.Line}), which the diagram doesn't show. {text}",
                    new Anchor(File: send.Call.At.File, Line: send.Call.At.Line),
                    Against: fix.Against is { } against ? [new Anchor(Element: against.Id)] : null);
            }
        }

        var reported = new HashSet<TypeNode>(ReferenceEqualityComparer.Instance);
        foreach (var (link, messages) in kinds)
        {
            var bases = string.Join(", ", messages.Bases.Select(x => $"`{x.Split('.')[^1]}`").Distinct(StringComparer.Ordinal));
            foreach (var type in messages.Types.Where(x => x.Value is null).Select(x => x.Key))
            {
                if (context.Scope.Checks(type.At.File) && !asked.Contains(type) && reported.Add(type))
                {
                    yield return new Finding("messaging/unmodelled.message", link.Designer, "ModelCode", "error",
                        $"`{type.Name}` ({type.At.File}:{type.At.Line}) is {(link.What[0] is 'a' or 'e' or 'i' or 'o' or 'u' ? "an" : "a")} {link.What} the diagram doesn't show: it has what every modelled {link.What} has ({bases}). Model the {link.Message} `{type.Name}`.",
                        new Anchor(File: type.At.File, Line: type.At.Line));
                }
            }
        }
    }

    /// <summary>Each call in checked code that hands a message on, new or typed as one, which no link claim covered.</summary>
    private static List<Send> Sends(RuleContext context, IReadOnlyDictionary<TypeNode, IElement?> messages, IReadOnlySet<CallNode> covered)
    {
        var sends = new List<Send>();
        var seen = new HashSet<(CallNode, TypeNode)>();
        foreach (var type in context.Code.Types.Where(x => context.Scope.Checks(x.At.File)))
        {
            foreach (var member in type.Members)
            {
                foreach (var call in member.Calls.Where(x => !covered.Contains(x)))
                {
                    for (var i = 0; i < call.Arguments.Count; i++)
                    {
                        foreach (var sent in MessageCode.MessagesOf(context.Code, type, call.Arguments[i]))
                        {
                            if (MessageCode.Resolve(context.Code, sent, type, messages.Keys) is { } message
                                && !seen.Contains((call, message))
                                && MessageCode.HandsOn(context.Code, type, member, call, i, sent)
                                && seen.Add((call, message)))
                            {
                                sends.Add(new Send(type, member, call, message, messages[message]));
                            }
                        }
                    }
                }
            }
        }

        return sends;
    }

    /// <summary>
    /// Which modelled source's code reaches each send, the nearest: a source's own method first, then the code's own methods
    /// it calls, its class's and its collaborators' (<see cref="MessageCode.Bodies"/>), so a helper's send is the handler's
    /// that calls it. Whether the source's own method makes the call.
    /// </summary>
    private static Dictionary<CallNode, (IElement Source, bool Own)> Reach(RuleContext context, MessageLink link, HashSet<CallNode> sends)
    {
        var reached = new Dictionary<CallNode, (IElement Source, bool Own)>(ReferenceEqualityComparer.Instance);
        var seen = new HashSet<MemberNode>();
        var queue = new Queue<(IElement Source, TypeNode Type, MemberNode Member, bool Own)>();
        foreach (var source in Sources(context, link))
        {
            foreach (var unit in MessageCode.Units(context.Code, source, x => context.MemberOf(link.Designer, x), x => context.TypeOf(link.Designer, x), context.Model.Owner))
            {
                if (seen.Add(unit.Member))
                {
                    queue.Enqueue((source, unit.Type, unit.Member, true));
                }
            }
        }

        while (reached.Count < sends.Count && queue.TryDequeue(out var current))
        {
            foreach (var call in current.Member.Calls)
            {
                if (sends.Contains(call))
                {
                    reached.TryAdd(call, (current.Source, current.Own));
                }

                foreach (var body in MessageCode.Bodies(context.Code, current.Type, call).Where(x => seen.Add(x.Member)))
                {
                    queue.Enqueue((current.Source, body.Type, body.Member, false));
                }
            }
        }

        return reached;
    }

    /// <summary>
    /// What the diagram draws the link's kind from: constructors, operations, commands and queries, and handlers'
    /// subscriptions, the target ends of their associations to a message, other than the message links themselves.
    /// </summary>
    private static IEnumerable<IElement> Sources(RuleContext context, MessageLink link)
    {
        var links = MessageLink.All.Select(x => x.Association).ToHashSet(StringComparer.Ordinal);
        var messages = MessageLink.All.Select(x => x.Message).ToHashSet(StringComparer.Ordinal);
        foreach (var element in context.Model.Elements(link.Designer))
        {
            if (element.SpecializationType is "Operation" or "Class Constructor" or "Command" or "Query")
            {
                yield return element;
            }

            foreach (var end in (element.AssociatedElements ?? []).Where(x => x.Association?.TargetEnd?.Id == x.Id
                         && !links.Contains(x.Association.SpecializationType)
                         && x.TypeReference?.Element is IElement { } target && messages.Contains(target.SpecializationType)))
            {
                yield return end;
            }
        }
    }

    /// <summary>
    /// Where the diagram would show the send: the fix, and the element it is against. Null when the code is nothing the
    /// diagram can show, so the send is no drift.
    /// </summary>
    private static (string Text, IElement? Against)? Fix(RuleContext context, MessageLink link, Send send, Dictionary<CallNode, (IElement Source, bool Own)> reach, IReadOnlyDictionary<TypeNode, IElement> modelled)
    {
        var name = send.Message.Name;
        if (reach.TryGetValue(send.Call, out var reached))
        {
            return ($"Add a {link.Link} link to `{name}` from {Name(context, reached.Source)}{(reached.Own ? "" : ", whose code calls this")}.",
                reached.Source is IAssociationEnd ? context.Model.Owner(reached.Source) : reached.Source);
        }

        if (context.ElementOf(send.Type) is { } owner && owner.Designer == link.Designer && owner.Element.SpecializationType is "Class" or "Service")
        {
            var member = send.Member.Kind == "constructor" ? "the constructor" : $"`{send.Member.Name}`";
            return ($"Model {member} on `{owner.Element.Name}`, then add a {link.Link} link from it to `{name}`.", owner.Element);
        }

        if (link.Designer == "Services"
            && send.Member.Parameters.Select(x => x.Type is { } type ? context.Code.TypeFor(type, send.Type) : null).FirstOrDefault(x => x is not null && modelled.ContainsKey(x)) is { } handled)
        {
            return ($"Model `{send.Type.Name}` as a handler of `{handled.Name}`, then add a {link.Link} link to `{name}` from its subscription.", modelled[handled]);
        }

        return null;
    }

    /// <summary>A link source as a finding names it.</summary>
    private static string Name(RuleContext context, IElement source) => source switch
    {
        IAssociationEnd => MessageCode.Describe(source),
        { SpecializationType: "Class Constructor" } => $"`{context.Model.Owner(source)?.Name}`'s constructor",
        { SpecializationType: "Operation" } => $"`{context.Model.Owner(source)?.Name}.{source.Name}`",
        _ => $"`{source.Name}`"
    };
}

/// <summary>The calls a link claim read as raising its message, so the rule for unmodelled ones passes over them.</summary>
internal sealed class CoveredCalls
{
    public HashSet<CallNode> Calls { get; } = new(ReferenceEqualityComparer.Instance);
}

/// <summary>
/// The types of the code that are messages of a kind of link, each with its modelled element, or null for one the model
/// doesn't have; and what every modelled one derives from or implements, which made the others messages.
/// </summary>
internal sealed record MessageTypes(IReadOnlyDictionary<TypeNode, IElement?> Types, IReadOnlyCollection<string> Bases);

/// <summary>The code's own methods each call reaches (<see cref="MessageCode.Bodies"/>), found once per run.</summary>
internal sealed class BodiesOf
{
    public Dictionary<CallNode, IReadOnlyList<(TypeNode Type, MemberNode Member)>> Calls { get; } = new(ReferenceEqualityComparer.Instance);
}

/// <summary>Reading the code for messages, shared by the claims and the rule.</summary>
internal static class MessageCode
{
    /// <summary>
    /// The code a link starts from: a constructor's or operation's located member; for a command or query, the methods that
    /// take it; for a handler's subscription, the handler's methods that take what it subscribes to.
    /// </summary>
    /// <param name="memberOf">The member an element's existence claim located.</param>
    /// <param name="typeOf">The type an element's existence claim located.</param>
    /// <param name="owner">What an element is a member of: for a subscription, its handler.</param>
    public static IReadOnlyList<(TypeNode Type, MemberNode Member)> Units(CodeGraph code, IElement source, Func<IElement, MemberNode?> memberOf, Func<IElement, TypeNode?> typeOf, Func<IElement, IElement?> owner)
    {
        if (source is IAssociationEnd subscription)
        {
            return subscription.TypeReference?.Element is IElement handled
                ? Handlers(code, owner(subscription) is { } handler ? typeOf(handler) : null, owner(subscription)?.Name, handled.Name)
                : [];
        }

        if (source.SpecializationType is "Command" or "Query")
        {
            return Handlers(code, null, null, typeOf(source)?.Name ?? source.Name);
        }

        // A Minimal API lambda is no type's member: it runs in its router's, the type its service located.
        return memberOf(source) is { } member && (Declaring(code, member) ?? (owner(source) is { } service ? typeOf(service) : null)) is { } type ? [(type, member)] : [];
    }

    /// <summary>
    /// The methods that take a message: in the handler's type the model's handler located, else in a type of its name (several
    /// services may each have an OrderStatusChangedToPaidIntegrationEventHandler), otherwise anywhere.
    /// </summary>
    private static IReadOnlyList<(TypeNode Type, MemberNode Member)> Handlers(CodeGraph code, TypeNode? located, string? handler, string message)
    {
        var all = code.Types
            .SelectMany(t => t.Members.Where(m => m.Kind == "method" && m.Parameters.Any(p => p.Type is { } x && Mentions(x, message))).Select(m => (Type: t, Member: m)))
            .ToList();
        var own = all.Where(x => x.Type == located).ToList();
        if (own.Count == 0)
        {
            own = all.Where(x => x.Type.Name == handler).ToList();
        }

        return own.Count > 0 ? own : all;
    }

    private static bool Mentions(TypeRefNode type, string name) => type.Name == name || type.Arguments.Any(x => Mentions(x, name));

    private static TypeNode? Declaring(CodeGraph code, MemberNode member) => code.Types.FirstOrDefault(x => x.Members.Contains(member));

    public static string NotFound(ClaimContext context, IElement source, MessageLink link) => source is IAssociationEnd subscription
        ? $"No method takes `{subscription.TypeReference?.Element?.Name}`, so what `{context.Owner(subscription)?.Name}` {link.Verb} can't be read."
        : $"No method takes `{source.Name}`, so what its handler {link.Verb} can't be read.";

    /// <summary>
    /// The calls in the method, or in the code's own methods it calls (transitively), that hand a message on (<see cref="MessagesOf"/>),
    /// each with the method's own call that leads to it: itself, or its call of the method that does. A method does what the
    /// methods it delegates to do, its type's and its collaborators' alike (<see cref="Bodies"/>): a handler that has a helper send.
    /// </summary>
    public static IEnumerable<(CallNode Root, CallNode Call)> Raising(CodeGraph code, TypeNode type, MemberNode member, Func<TypeRefNode, bool> isMessage)
    {
        var seen = new HashSet<MemberNode> { member };
        var queue = new Queue<(TypeNode Type, MemberNode Member, CallNode? Root)>([(type, member, null)]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Member.Calls)
            {
                var root = current.Root ?? call;
                if (call.Arguments.Select((x, i) => (Value: x, Index: i)).Any(a => MessagesOf(code, current.Type, a.Value).Any(c => isMessage(c) && HandsOn(code, current.Type, current.Member, call, a.Index, c))))
                {
                    yield return (root, call);
                }

                foreach (var body in Bodies(code, current.Type, call).Where(x => seen.Add(x.Member)))
                {
                    queue.Enqueue((body.Type, body.Member, root));
                }
            }
        }
    }

    /// <summary>
    /// The messages a value can be, as types: what it creates (<see cref="Created"/>), and the type the code gives it, unless it
    /// is the method's own parameter, whose message its caller handed in. <c>context.SendLocal(nextMessage)</c> sends what
    /// <c>var nextMessage = await router.NextAsync(...)</c> types it as, <c>context.SendLocal(outputs.SendEmail)</c> the
    /// property's type.
    /// </summary>
    public static IEnumerable<TypeRefNode> MessagesOf(CodeGraph code, TypeNode from, ValueNode value)
    {
        var seen = new HashSet<TypeRefNode>(ReferenceEqualityComparer.Instance);
        foreach (var creation in Created(code, from, value))
        {
            if (seen.Add(creation.Type))
            {
                yield return creation.Type;
            }
        }

        if (!value.IsKnown || FromParameter(value, 0))
        {
            yield break;
        }

        foreach (var type in value.Types.Where(seen.Add))
        {
            yield return type;
        }
    }

    /// <summary>The value is a parameter of its unit, or a local given one.</summary>
    private static bool FromParameter(ValueNode value, int depth) =>
        value.Parameter is not null || depth < 3 && value.Given.Any(x => FromParameter(x, depth + 1));

    /// <summary>
    /// The messages a value creates: a creation, each message the code's own method it is the result of can return, through
    /// the methods that method returns the result of in turn, and what a local is given. <c>AddDomainEvent(StatusChanged())</c>
    /// raises each event <c>StatusChanged()</c> returns, whichever branch returns it: a link says what the code can do, not
    /// what it always does.
    /// </summary>
    public static IEnumerable<CreationNode> Created(CodeGraph code, TypeNode from, ValueNode value) => Created(code, from, value, 0);

    private static IEnumerable<CreationNode> Created(CodeGraph code, TypeNode from, ValueNode value, int depth)
    {
        foreach (var creation in value.Creations)
        {
            yield return creation;
        }

        if (depth >= 3)
        {
            yield break;
        }

        if (value.Call is not { } call)
        {
            // A local, or a conditional: what each of its values creates (var next = await router.NextAsync(message)).
            foreach (var creation in value.Given.SelectMany(x => Created(code, from, x, depth + 1)))
            {
                yield return creation;
            }

            yield break;
        }

        foreach (var body in Bodies(code, from, call))
        {
            foreach (var creation in (body.Member.Body?.Returns ?? []).SelectMany(x => Created(code, body.Type, x, depth + 1)))
            {
                yield return creation;
            }
        }
    }

    /// <summary>
    /// The value may be a message the code doesn't say: a type the code doesn't give, or one the message derives from or
    /// implements, such as a library factory's <c>INotification</c>. The result of the code's own method is what that
    /// method returns (<see cref="Created"/>), so a helper that returns other messages says it isn't this one; a local is
    /// what it is given.
    /// </summary>
    /// <param name="supertypes">What the message derives from or implements (<see cref="CodeGraph.Supertypes"/>).</param>
    public static bool Untyped(CodeGraph code, TypeNode from, ValueNode value, IReadOnlyCollection<string> supertypes) => Untyped(code, from, value, supertypes, 0);

    private static bool Untyped(CodeGraph code, TypeNode from, ValueNode value, IReadOnlyCollection<string> supertypes, int depth)
    {
        if (depth < 3 && value.Call is { } call && Bodies(code, from, call) is { Count: > 0 } bodies)
        {
            return bodies.Any(x => (x.Member.Body?.Returns ?? []).Any(r => Untyped(code, x.Type, r, supertypes, depth + 1)));
        }

        if (depth < 3 && value.Call is null && value.Creations.Count == 0 && value.Given.Count > 0)
        {
            return value.Given.Any(x => Untyped(code, from, x, supertypes, depth + 1));
        }

        return !value.IsKnown || value.Types.Any(x => code.TypeFor(x, from) is { } type
            ? supertypes.Contains(type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name)
            : supertypes.Contains(x.QualifiedName) || supertypes.Contains(x.Name));
    }

    /// <summary>
    /// The call hands the message it is given as an argument on, out of the code. A library's method (a bus, an outbox, the
    /// list behind an entity base's <c>AddDomainEvent</c>) takes it. A method of the code takes it on only when its body does
    /// with the parameter it binds to (<see cref="Passes"/>): one that only reads it processes it in place
    /// (<c>Archive(new OrderShipped(id))</c> that logs its fields), and is no raise. Nor is a call in what a method
    /// returns when the method returns the message: it builds it for its caller (<c>return Task.FromResult(message)</c>).
    /// </summary>
    public static bool HandsOn(CodeGraph code, TypeNode type, MemberNode member, CallNode call, int argument, TypeRefNode message) =>
        HandsOn(code, type, member, call, argument, message.Name);

    /// <inheritdoc cref="HandsOn(CodeGraph, TypeNode, MemberNode, CallNode, int, TypeRefNode)"/>
    public static bool HandsOn(CodeGraph code, TypeNode type, MemberNode member, CallNode call, int argument, string message) =>
        HandsOn(code, type, member, call, argument, message, new HashSet<MemberNode>(ReferenceEqualityComparer.Instance));

    private static bool HandsOn(CodeGraph code, TypeNode type, MemberNode member, CallNode call, int argument, string message, HashSet<MemberNode> seen)
    {
        if (member.Type is { } returns && Mentions(returns, message) && (member.Body?.Returns ?? []).Any(x => x.At.Contains(call.At)))
        {
            return false;
        }

        var bodies = Bodies(code, type, call).ToList();
        return bodies.Count == 0 || bodies.Any(x => seen.Add(x.Member) && ParameterAt(x.Member, argument) is { } parameter && Passes(code, x.Type, x.Member, parameter, message, seen));
    }

    /// <summary>
    /// The method hands its parameter on: to a call that hands it on, into something it creates, such as an outbox's entry
    /// (<c>new IntegrationEventLogEntry(@event, transactionId)</c>), or into what it keeps (<c>_events = [.._events, item]</c>).
    /// </summary>
    private static bool Passes(CodeGraph code, TypeNode type, MemberNode member, ParameterNode parameter, string message, HashSet<MemberNode> seen) =>
        member.Creations.Any(x => x.Arguments.Concat(x.Initializers.Select(i => i.Value)).Any(v => Names(v, parameter, 0)))
        || (member.Body?.Assignments ?? []).Any(x => Names(x.Value, parameter, 0))
        || member.Calls.Any(call => call.Arguments.Select((x, i) => (Value: x, Index: i)).Any(a => Names(a.Value, parameter, 0) && HandsOn(code, type, member, call, a.Index, message, seen)));

    /// <summary>The value is the parameter, a local given it, or a collection of it.</summary>
    private static bool Names(ValueNode value, ParameterNode parameter, int depth) =>
        ReferenceEquals(value.Parameter, parameter)
        || depth < 3 && value.Given.Concat(value.Items).Any(x => Names(x, parameter, depth + 1));

    /// <summary>The parameter an argument binds to, by position; the last takes any after it (<c>params</c>).</summary>
    private static ParameterNode? ParameterAt(MemberNode member, int argument)
    {
        var parameters = member.Body?.Parameters is { Count: > 0 } own ? own : member.Parameters;
        return argument < parameters.Count ? parameters[argument] : parameters.Count > 0 && parameters[^1].Type is { IsArray: true } ? parameters[^1] : null;
    }

    /// <summary>
    /// The code's own methods a call reaches, with their bodies: its type's, inherited ones included, or, through what it is
    /// called on, a collaborator's (<c>_packer.Pack(...)</c>). A method an interface or abstract method stands for is each
    /// implementation's the code has. None for a library's method. A method passed on as a value (<c>GetCacheData(key,
    /// gradeBO.GetGradeDifferences)</c>) is one that runs, as <c>CallsFrom</c> has it, so a reference reaches its method too.
    /// Found once per call and run.
    /// </summary>
    public static IReadOnlyList<(TypeNode Type, MemberNode Member)> Bodies(CodeGraph code, TypeNode from, CallNode call)
    {
        if (code.Get<BodiesOf>() is not { } known)
        {
            code.Set(known = new BodiesOf());
        }

        if (!known.Calls.TryGetValue(call, out var bodies))
        {
            known.Calls[call] = bodies = FindBodies(code, from, call).ToList();
        }

        return bodies;
    }

    private static IEnumerable<(TypeNode Type, MemberNode Member)> FindBodies(CodeGraph code, TypeNode from, CallNode call)
    {
        // A reference has no arguments of its own: it reaches any overload.
        foreach (var target in code.Targets(call.Name, call.Receiver, call.ReceiverType, call.IsReference ? null : call.Arguments.Count, from))
        {
            if (HasBody(target.Type, target.Member))
            {
                yield return (target.Type, target.Member);
                continue;
            }

            foreach (var implementation in Implementations(code, target.Type))
            {
                foreach (var member in implementation.Members.Where(x => x.Kind == "method" && x.Name == target.Member.Name && x.Parameters.Count == target.Member.Parameters.Count && HasBody(implementation, x)))
                {
                    yield return (implementation, member);
                }
            }
        }
    }

    /// <summary>An interface's or an abstract method has no body of its own, whatever unit the reader gives it.</summary>
    private static bool HasBody(TypeNode type, MemberNode member) =>
        member.Body is not null && type.Kind != "interface" && !member.Modifiers.Contains("abstract");

    private static IEnumerable<TypeNode> Implementations(CodeGraph code, TypeNode type) => code.ImplementationsOf(type).Where(x => x.Kind is not "interface");

    public static void Cover(CodeGraph code, IEnumerable<CallNode> calls)
    {
        if (code.Get<CoveredCalls>() is not { } covered)
        {
            code.Set(covered = new CoveredCalls());
        }

        covered.Calls.UnionWith(calls);
    }

    /// <summary>
    /// The type handed on is the message: the type the model's element is in the code, or one of its name. Services each keep
    /// their own copy of a message they share, so a copy of the name is the message too.
    /// </summary>
    public static bool Is(CodeGraph code, TypeRefNode sent, TypeNode from, TypeNode? message, string name) =>
        code.TypeFor(sent, from) is { } type ? type == message || type.Name == name : sent.Name == name;

    /// <summary>
    /// The messages the code can raise for a kind of link: each modelled one, as its existence claim located it or by name,
    /// and every other type of the code that has what all the modelled ones derive from or implement, such as
    /// <c>INotification</c>, with what that is. With no modelled message there is nothing to learn from, and none.
    /// </summary>
    public static MessageTypes Messages(RuleContext context, MessageLink link)
    {
        var messages = new Dictionary<TypeNode, IElement?>(ReferenceEqualityComparer.Instance);
        foreach (var element in context.Model.Elements(link.Designer, link.Message).Concat(context.Model.ReferencedElements(link.Designer).Where(x => x.SpecializationType == link.Message)))
        {
            if (TypeOf(context.Code, (designer, x) => context.LocatedOf(designer, x, "type.exists") as TypeNode, link, element) is { } type)
            {
                messages.TryAdd(type, element);
            }
        }

        var bases = messages.Keys.Select(x => context.Code.Supertypes(x).ToHashSet(StringComparer.Ordinal)).Aggregate((HashSet<string>?)null, (all, x) =>
        {
            if (all is null)
            {
                return x;
            }

            all.IntersectWith(x);
            return all;
        }) ?? [];
        if (bases.Count > 0)
        {
            // Another service's copy of a modelled message, of its name, is that message.
            var byName = messages.GroupBy(x => x.Key.Name).ToDictionary(x => x.Key, x => x.First().Value);
            foreach (var type in context.Code.Types.Where(x => x.Kind is "class" or "record" && !x.IsAbstract && context.Code.Supertypes(x).Any(bases.Contains)))
            {
                messages.TryAdd(type, byName.GetValueOrDefault(type.Name));
            }
        }

        return new MessageTypes(messages, bases);
    }

    /// <summary>The type a message element is in the code: where its existence claim located it, or the type of its name.</summary>
    public static TypeNode? TypeOf(CodeGraph code, Func<string, IElement, TypeNode?> located, MessageLink link, IElement element) =>
        located(link.Designer, element)
        ?? code.Types.FirstOrDefault(x => x.Name == element.Name && x.Kind is "class" or "record");

    public static TypeNode? Resolve(CodeGraph code, TypeRefNode sent, TypeNode from, IEnumerable<TypeNode> messages) =>
        code.TypeFor(sent, from) is { } type && messages.Contains(type) ? type : null;

    /// <summary>A method as a finding names it: <c>Order.SetShippedStatus</c>, a constructor by its type's name.</summary>
    public static string Display(TypeNode type, MemberNode member) => member.Kind == "constructor" ? $"{type.Name}.{type.Name}" : $"{type.Name}.{member.Name}";

    /// <summary>Where a link starts, as a finding words it.</summary>
    public static string Describe(IElement source) => source is IAssociationEnd subscription
        ? $"`{subscription.OtherEnd()?.TypeReference?.Element?.Name}`'s handling of `{subscription.TypeReference?.Element?.Name}`"
        : $"`{source.Name}`";
}
