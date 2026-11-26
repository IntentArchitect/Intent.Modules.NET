using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Blazor;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;

namespace Intent.Modules.Common.DriftEngine.Stacks.MvvmToolkit;

/// <summary>
/// The MVVM Toolkit (<c>CommunityToolkit.Mvvm</c>), as MAUI, WPF, WinUI, Avalonia and Blazor code uses it: an element's
/// Publishes and Subscribes of an App Event read from its messenger (<c>appEvent.publishes</c>, <c>appEvent.subscribes</c>), and
/// the toolkit's message bases, whose members its code doesn't hold: <c>ValueChangedMessage&lt;T&gt;</c>'s <c>Value</c> and
/// <c>PropertyChangedMessage&lt;T&gt;</c>'s, so an App Event deriving from one has them as properties to check. It answers only
/// where the toolkit is in the code, for an element whose code is C#.
/// </summary>
public sealed class MvvmToolkitExtension : IDriftExtension
{
    public string Id => "intent.mvvmtoolkit";

    public IEnumerable<ICodeReader> Readers { get; } = [new MessageTypes()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Messenger()];

    /// <summary>The toolkit's message bases, declared as the stock EF Core stack declares ASP.NET Core Identity's.</summary>
    private sealed class MessageTypes : ICodeReader
    {
        private static readonly Span Library = new("", 0);

        public void Read(CodeGraph graph)
        {
            if (CSharpSyntax.Of(graph) is null)
            {
                return;
            }

            var t = new TypeRefNode { Text = "T", Name = "T", QualifiedName = "T" };
            var objectType = new TypeRefNode { Text = "object", Name = "object", QualifiedName = "object" };
            graph.AddLibraryType("ValueChangedMessage", ["T"], [objectType], [Property("Value", t)]);
            graph.AddLibraryType("PropertyChangedMessage", ["T"], [objectType],
            [
                Property("Sender", objectType),
                Property("PropertyName", new TypeRefNode { Text = "string", Name = "string", QualifiedName = "string" }),
                Property("OldValue", t),
                Property("NewValue", t)
            ]);
        }

        private static MemberNode Property(string name, TypeRefNode type) => new() { Name = name, Kind = "property", Type = type, At = Library };
    }
}

/// <summary>
/// <c>appEvent.publishes</c> and <c>appEvent.subscribes</c> on the messenger. Publishing is a <c>Send</c> on a messenger
/// (<c>WeakReferenceMessenger.Default</c>, <c>StrongReferenceMessenger.Default</c>, an <c>IMessenger</c>, an
/// <c>ObservableRecipient</c>'s <c>Messenger</c>) of the event, created in place or anywhere it comes from
/// (<c>Send(new ProductCountChangedMessage(...))</c>, a local, the code's own helper that sends what it is handed), or named
/// as its type argument (<c>Send&lt;ProductCountChangedMessage&gt;()</c>). Subscribing is a <c>Register</c> naming the event as
/// a type argument (<c>Register&lt;CatalogView, ProductCountChangedMessage&gt;</c>), or the class being an
/// <c>IRecipient&lt;ProductCountChangedMessage&gt;</c>, which <c>RegisterAll</c> or an active recipient registers. The element's
/// code is its own members and the code's own methods they reach.
/// </summary>
internal sealed class Messenger : IClaimReader
{
    private static readonly HashSet<string> Messengers = new(StringComparer.Ordinal) { "IMessenger", "WeakReferenceMessenger", "StrongReferenceMessenger" };

    public IReadOnlyCollection<string> Kinds { get; } = ["appEvent.publishes", "appEvent.subscribes"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is not IAssociationEnd end || end.TypeReference?.Element is not IElement appEvent || context.Owner(end) is not { } source
            || BlazorExtension.CodeOf(context, source) is not { } located || !IsCSharp(context.Code, located.Type) || !InCode(context.Code))
        {
            return null;
        }

        var (owner, type) = located;
        if (context.TypeOf(appEvent) is not { } eventType)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{appEvent.Name}`, which nothing located.");
        }

        bool IsEvent(TypeRefNode reference) =>
            reference.Name == eventType.Name && (reference.QualifiedName == reference.Name || $".{MvvmQualified(eventType)}".EndsWith("." + reference.QualifiedName, StringComparison.Ordinal));

        var calls = Calls(context.Code, type);
        return context.Kind == "appEvent.publishes" ? Publishes(context, owner, type, appEvent, calls, IsEvent) : Subscribes(context, owner, type, appEvent, calls, IsEvent);
    }

    private static ClaimReading Publishes(ClaimContext context, IElement owner, TypeNode type, IElement appEvent, IReadOnlyList<(CallNode Call, TypeNode In)> calls, Func<TypeRefNode, bool> isEvent)
    {
        var code = context.Code;

        // A message handed to Send, through the code's own helpers that send what they are handed.
        foreach (var member in code.MembersOf(type).Where(x => x.Body is not null))
        {
            foreach (var (root, call, @in) in MessageCode.Raising(code, type, member, isEvent))
            {
                if (IsSend(code, call, @in) || MessageCode.HandedTo(code, @in, call).Any(x => IsSend(code, x.Call, x.Type)))
                {
                    return ClaimReading.Found(root, root.At);
                }
            }
        }

        // Send<TMessage>() makes the message itself.
        if (calls.FirstOrDefault(x => IsSend(code, x.Call, x.In) && x.Call.TypeArguments.Count > 0 && isEvent(x.Call.TypeArguments[0])) is { Call: { } made })
        {
            return ClaimReading.Found(made, made.At);
        }

        var sends = calls.Where(x => IsSend(code, x.Call, x.In)).Select(x => x.Call).ToList();
        return ClaimReading.Compared(sends.Count == 0 ? "none" : string.Join(", ", sends.Select(x => x.Text)), sends.Count == 0 ? type.At : sends[0].At, false,
            $"`{owner.Name}` publishes `{appEvent.Name}`: in the model, not in the code. `{type.Name}` {(sends.Count == 0 ? "sends no message" : "sends " + string.Join(", ", sends.Select(x => $"`{x.Text}`")))} on a messenger. The model states it in its Publishes. Send the message, or remove the Publishes.");
    }

    private static ClaimReading Subscribes(ClaimContext context, IElement owner, TypeNode type, IElement appEvent, IReadOnlyList<(CallNode Call, TypeNode In)> calls, Func<TypeRefNode, bool> isEvent)
    {
        var code = context.Code;
        var registers = calls.Where(x => x.Call.Name == "Register" && IsMessenger(code, x.Call, x.In)).Select(x => x.Call).ToList();
        if (registers.FirstOrDefault(x => x.TypeArguments.Any(isEvent)) is { } registered)
        {
            return ClaimReading.Found(registered, registered.At);
        }

        var recipient = new[] { type }.Concat(code.Bases(type))
            .SelectMany(x => x.BaseTypes)
            .FirstOrDefault(x => x.Name == "IRecipient" && x.Arguments.Count == 1 && isEvent(x.Arguments[0]));
        if (recipient is not null)
        {
            return ClaimReading.Found(type, type.At);
        }

        return ClaimReading.Compared(registers.Count == 0 ? "none" : string.Join(", ", registers.Select(x => x.Text)), registers.Count == 0 ? type.At : registers[0].At, false,
            $"`{owner.Name}` subscribes to `{appEvent.Name}`: in the model, not in the code. `{type.Name}` {(registers.Count == 0 ? "registers for no message" : "registers " + string.Join(", ", registers.Select(x => $"`{x.Text}`")))} on a messenger, and isn't an `IRecipient<{appEvent.Name}>`. The model states it in its Subscribes. Register for the message, or remove the Subscribes.");
    }

    /// <summary>The calls the type's members make, and those of the code's own methods they reach, each with the type making it.</summary>
    private static IReadOnlyList<(CallNode Call, TypeNode In)> Calls(CodeGraph code, TypeNode type)
    {
        var found = new List<(CallNode, TypeNode)>();
        var seen = new HashSet<MemberNode>();
        var queue = new Queue<(TypeNode Type, MemberNode Member)>(code.MembersOf(type).Where(x => x.Body is not null && seen.Add(x)).Select(x => (type, x)));
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Member.Calls)
            {
                found.Add((call, current.Type));
                foreach (var body in MessageCode.Bodies(code, current.Type, call).Where(x => seen.Add(x.Member)))
                {
                    queue.Enqueue(body);
                }
            }
        }

        return found;
    }

    private static bool IsSend(CodeGraph code, CallNode call, TypeNode from) => call.Name == "Send" && IsMessenger(code, call, from);

    /// <summary>
    /// The call is made on a messenger: one the code types as the toolkit's, a messenger's <c>Default</c> instance, or the
    /// <c>Messenger</c> an <c>ObservableRecipient</c> has.
    /// </summary>
    private static bool IsMessenger(CodeGraph code, CallNode call, TypeNode from)
    {
        if (call.ReceiverType is { } typed && code.TypeFor(typed, from) is null && Messengers.Contains(typed.Name))
        {
            return true;
        }

        var segments = (call.Receiver ?? "").Split('.');
        return segments is [.., "WeakReferenceMessenger" or "StrongReferenceMessenger", "Default"]
               || segments is ["Messenger"] or ["this" or "base", "Messenger"] && code.Supertypes(from).Any(x => x.Split('.')[^1] == "ObservableRecipient");
    }

    /// <summary>The toolkit is in the code: a file imports it, or a type is one of its observable objects or recipients.</summary>
    private static bool InCode(CodeGraph code)
    {
        if (code.Get<ToolkitPresence>() is not { } presence)
        {
            code.Set(presence = new ToolkitPresence(
                code.Files.Any(x => x.Imports.Any(i => i.Target.StartsWith("CommunityToolkit.Mvvm", StringComparison.Ordinal)))
                || code.Types.Any(x => x.BaseTypes.Any(b => b.Name is "ObservableObject" or "ObservableRecipient" or "ObservableValidator" or "IRecipient"))));
        }

        return presence.InCode;
    }

    private static bool IsCSharp(CodeGraph code, TypeNode type) =>
        code.Files.Any(x => x.Language == "csharp" && x.Path == type.At.File);

    private static string MvvmQualified(TypeNode type) => type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name;

    private sealed record ToolkitPresence(bool InCode);
}
