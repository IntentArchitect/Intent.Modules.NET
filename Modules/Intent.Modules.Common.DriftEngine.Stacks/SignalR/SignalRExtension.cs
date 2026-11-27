using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

namespace Intent.Modules.Common.DriftEngine.Stacks.SignalR;

/// <summary>
/// ASP.NET Core SignalR's JavaScript client (<c>@microsoft/signalr</c>, earlier <c>@aspnet/signalr</c>), as the User Interface
/// claims read it. A call made on a <c>HubConnection</c>, <c>connection.invoke("SendComment", values)</c> (or <c>send</c>,
/// <c>stream</c>), calls the hub method of that name, matched without case as SignalR matches it: the Services Operation of a
/// Service whose class derives from <c>Hub</c>. <c>connection.on("ReceiveComment", comment =&gt; ...)</c> handles the client
/// method a hub sends (<c>Clients.All.SendAsync("ReceiveComment", comment)</c>), which the User Interface designer models as an
/// App Event of that name: the handler is the event's code, its parameters the event's Properties in order, and the code that
/// registers it subscribes to it. Registered after the TypeScript front ends, so it is asked before their links, and it answers
/// only for SignalR's own calls.
/// </summary>
public sealed class SignalRExtension : IDriftExtension
{
    public string Id => "intent.signalr";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new HubInvocations(), new ClientMethods()];
}

/// <summary>What the SignalR readers share: the calls a <c>HubConnection</c> makes, and the hub a Service is.</summary>
internal static class HubClient
{
    private static readonly HashSet<string> Invocations = ["invoke", "send", "stream"];

    /// <summary>The call is made on a <c>HubConnection</c> (its type, or a union with it: <c>HubConnection | null</c>), naming its method first.</summary>
    private static bool OnConnection(CallNode call) =>
        call.ReceiverType is { } type && (type.Name == "HubConnection" || type.Union.Any(x => x.Name == "HubConnection"))
        && call.Arguments.Count > 0 && call.Arguments[0].Literal is { Kind: "string", Value: { Length: > 0 } };

    /// <summary>The hub method a call invokes, when it is a <c>HubConnection</c>'s <c>invoke</c>, <c>send</c> or <c>stream</c>.</summary>
    public static string? Invoked(CallNode call) => Invocations.Contains(call.Name) && OnConnection(call) ? call.Arguments[0].Literal!.Value : null;

    /// <summary>The client method a call handles, when it is a <c>HubConnection</c>'s <c>on</c>.</summary>
    public static string? Handled(CallNode call) => call.Name == "on" && OnConnection(call) ? call.Arguments[0].Literal!.Value : null;

    /// <summary>The class a Service is, when it is a SignalR hub: it derives from <c>Hub</c> or <c>Hub&lt;T&gt;</c>.</summary>
    public static TypeNode? Hub(ClaimContext context, IElement service) =>
        (context.TypeOf(service) ?? context.LocatedOf(service, "type.exists", "Services") as TypeNode) is { } type && context.Code.Supertypes(type).Contains("Hub") ? type : null;

    /// <summary>The calls the code makes, nearest first: in its units, then in the code's own functions they reach.</summary>
    public static IEnumerable<CallNode> Calls(CodeGraph code, TypeNode type)
    {
        if (UiCode.FileOf(code, type.At.File) is not { } file)
        {
            yield break;
        }

        var seen = new HashSet<FunctionNode>();
        var queue = new Queue<(TypeNode Type, FunctionNode Unit)>(UiCode.Units(code, file).Where(x => x.Type == type).Select(x => (x.Type, x.Unit)).Where(x => seen.Add(x.Unit)));
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Unit.Calls)
            {
                yield return call;
                foreach (var (owner, member) in UiCode.Reached(code, current.Type, call))
                {
                    if (member.Body is { } body && seen.Add(body))
                    {
                        queue.Enqueue((owner, body));
                    }
                }
            }
        }
    }

    /// <summary>Every handler the TypeScript code registers on a <c>HubConnection</c>, with the client method it handles.</summary>
    public static IEnumerable<(CallNode Call, string Method)> Handlers(CodeGraph code) =>
        code.Types.Where(UiCode.IsTypeScript).SelectMany(x => x.Members).Select(x => x.Body).OfType<FunctionNode>().SelectMany(x => x.Calls)
            .Select(x => (Call: x, Method: Handled(x))).Where(x => x.Method is not null).Select(x => (x.Call, x.Method!));
}

/// <summary>
/// A TypeScript component's or store's call of a hub method (<c>intent.invokes</c>, and <c>intent.target</c>, the hub): through
/// its <c>HubConnection</c>, itself or in the code's own functions it calls (a component calling a store action that invokes it).
/// </summary>
internal sealed class HubInvocations : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.invokes", "intent.target"];

    public ClaimReading? Read(ClaimContext context)
    {
        var (source, target) = context.Element is IAssociationEnd end ? (context.Owner(end), end.TypeReference?.Element as IElement) : (null, null);
        if (source is null || target?.ParentElement is not { } service || HubClient.Hub(context, service) is not { } hub
            || UiCode.CodeOf(context, source) is not { Type: var type } || !UiCode.IsTypeScript(type))
        {
            return null;
        }

        var call = HubClient.Calls(context.Code, type).FirstOrDefault(x => string.Equals(HubClient.Invoked(x), target.Name, StringComparison.OrdinalIgnoreCase));
        if (call is null)
        {
            return context.Kind == "intent.invokes"
                ? ClaimReading.Compared("absent", type.At, false,
                    $"`{source.Name}` invokes hub method `{service.Name}.{target.Name}` in the model, but its code makes no `invoke`, `send` or `stream` of `{target.Name}` on a `HubConnection`. Remove the Call Service Operation if the code is right.")
                : null;
        }

        return context.Kind == "intent.target" ? ClaimReading.Compared(hub.Name, call.At, true) : ClaimReading.Found(call, call.At);
    }
}

/// <summary>
/// A hub's client method the TypeScript code handles with <c>connection.on(...)</c>: an App Event's code (<c>type.exists</c>,
/// where no other code of its name is), its Properties (<c>member.exists</c>, the handler's parameters in order), and a Subscribes
/// from code that registers the handler (<c>appEvent.subscribes</c>).
/// </summary>
internal sealed class ClientMethods : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["type.exists", "member.exists", "appEvent.subscribes"];

    public ClaimReading? Read(ClaimContext context)
    {
        switch (context.Kind)
        {
            case "type.exists":
            {
                // A type of the name, or another reader's code, is the element's first: only a name nothing else declares is a handler's.
                var next = context.ReadNext();
                if (next is not null || context.Code.TypesNamed(context.Element.Name).Count > 0
                    || HubClient.Handlers(context.Code).FirstOrDefault(x => x.Method == context.Element.Name) is not { Call: { } handler })
                {
                    return next;
                }

                return ClaimReading.Found(handler, handler.At);
            }
            case "member.exists":
            {
                if (context.Element.ParentElement is not { } owner || context.LocatedOf(owner, "type.exists") is not CallNode handler || HubClient.Handled(handler) is null)
                {
                    return null;
                }

                // The event's members its claims locate, in order: the handler's parameters.
                var index = owner.ChildElements.Where(x => context.ClaimOn(x, "member.exists") is not null).ToList().IndexOf(context.Element);
                return handler.Arguments.ElementAtOrDefault(1)?.Function?.Parameters.ElementAtOrDefault(index) is { } parameter
                    ? ClaimReading.Found(parameter, parameter.At)
                    : ClaimReading.Missing(handler.At.File, $"parameter {index + 1} of the `{owner.Name}` handler");
            }
            default:
            {
                if (context.Element is not IAssociationEnd { TypeReference.Element: IElement appEvent } end || context.Owner(end) is not { } source
                    || UiCode.CodeOf(context, source) is not { Type: var type } || !UiCode.IsTypeScript(type))
                {
                    return null;
                }

                var handler = HubClient.Calls(context.Code, type).FirstOrDefault(x => HubClient.Handled(x) == appEvent.Name);
                if (handler is not null)
                {
                    return ClaimReading.Found(handler, handler.At);
                }

                // Code that registers no hub handler at all may subscribe some other way, another reader's to read.
                return HubClient.Calls(context.Code, type).Any(x => HubClient.Handled(x) is not null) ? ClaimReading.Absent(type.At, "absent") : null;
            }
        }
    }
}
