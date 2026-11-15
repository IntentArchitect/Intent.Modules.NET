using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Requests;

namespace Intent.Modules.Common.DriftEngine.Stacks.Wolverine;

/// <summary>
/// Wolverine's code (JasperFx) for the requests <c>intent.cqrs</c> claims. A request implements nothing: its handler is the
/// method Wolverine's conventions discover, and its result is what that method returns. Wolverine discovers a public, concrete,
/// non-generic class (a static one too) whose name ends in <c>Handler</c> or <c>Consumer</c>, or that has
/// <c>[WolverineHandler]</c> or implements <c>IWolverineHandler</c>, unless it has <c>[WolverineIgnore]</c>; on it, each public
/// method, static or instance, named <c>Handle</c>, <c>Handles</c>, <c>Consume</c> or <c>Consumes</c> (each also with
/// <c>Async</c>), or with <c>[WolverineHandler]</c>, handles the message its first parameter is. It answers only when Wolverine is
/// in the code (a file imports a <c>Wolverine</c> namespace, or a type has its handler attribute or interface), and never for a
/// request that is MediatR's or Mediator's, which their extensions read.
/// </summary>
public sealed class WolverineExtension : IDriftExtension
{
    public string Id => "intent.wolverine";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new WolverineResultReader(), new WolverineHandlerReader()];
}

/// <summary>A handler method Wolverine discovers, and the class it is on.</summary>
internal sealed record WolverineHandler(TypeNode Type, MemberNode Method)
{
    public string Name => $"{Type.Name}.{Method.Name}";
}

internal static class WolverineCode
{
    private static readonly HashSet<string> MethodNames = new(StringComparer.Ordinal)
    {
        "Handle", "Handles", "Consume", "Consumes", "HandleAsync", "HandlesAsync", "ConsumeAsync", "ConsumesAsync"
    };

    /// <summary>
    /// The handlers Wolverine discovers for the request, by the type their first parameter names (resolved, else by its name);
    /// null when Wolverine isn't in the code, or the request is a marker framework's (MediatR's or Mediator's) to read.
    /// </summary>
    public static IReadOnlyList<WolverineHandler>? HandlersOf(CodeGraph code, TypeNode request)
    {
        var index = Index(code);
        return !index.InCode || MarkerFramework.All.Any(x => x.Requests(code, request).Count > 0) ? null : index.Discovered.Of(request);
    }

    /// <summary>Methods named as a handler's that take the request, on a class Wolverine doesn't discover.</summary>
    public static IReadOnlyList<WolverineHandler> Undiscovered(CodeGraph code, TypeNode request) => Index(code).Undiscovered.Of(request);

    /// <summary>
    /// What a handler returns to the request's sender, Wolverine's <c>InvokeAsync&lt;T&gt;</c>: its return type, unwrapped from
    /// <c>Task&lt;T&gt;</c> or <c>ValueTask&lt;T&gt;</c>; null for nothing (<c>void</c>, <c>Task</c>, <c>ValueTask</c>). A tuple, or a
    /// type that cascades any number of messages (<c>OutgoingMessages</c>, <c>IEnumerable&lt;object&gt;</c>, <c>object</c>), is
    /// <see cref="Result.Unreadable"/>: which of what it returns answers the request depends on the type the sender asks for.
    /// </summary>
    public static Result ResultOf(MemberNode method)
    {
        if (method.Type is not { Name: not "void" } type)
        {
            return new Result(null, null);
        }

        if (type is { Name: "Task" or "ValueTask", IsArray: false })
        {
            if (type.Arguments.Count == 0)
            {
                return new Result(null, null);
            }

            if (type.Arguments.Count == 1)
            {
                type = type.Arguments[0];
            }
        }

        if (type.Name.StartsWith('('))
        {
            return new Result(type, "a tuple, each of whose items Wolverine cascades as a message");
        }

        if (type.Name is "object" or "OutgoingMessages" || type is { Name: "IEnumerable" or "IAsyncEnumerable", Arguments: [{ Name: "object" }] })
        {
            return new Result(type, "messages Wolverine cascades, any number of them");
        }

        return new Result(type, null);
    }

    /// <summary>What a handler returns: the type (null for nothing), and why it can't be read as one result, when it can't.</summary>
    internal sealed record Result(TypeRefNode? Type, string? Unreadable);

    private static Handlers Index(CodeGraph code)
    {
        if (code.Get<Handlers>() is { } index)
        {
            return index;
        }

        code.Set(index = new Handlers
        {
            InCode = code.Files.Any(x => x.Imports.Any(i => i.Target == "Wolverine" || i.Target.StartsWith("Wolverine.", StringComparison.Ordinal)))
                     || code.Types.Any(x => x.Attributes.Any(a => a.Name == "WolverineHandler") || x.BaseTypes.Any(b => b.Name == "IWolverineHandler")
                                                                                              || x.Members.Any(m => m.Attributes.Any(a => a.Name == "WolverineHandler")))
        });
        if (!index.InCode)
        {
            return index;
        }

        foreach (var type in code.Types.Where(x => x.Kind is "class" or "record"))
        {
            var discovered = IsHandlerType(code, type);
            foreach (var method in code.MembersOf(type).Where(x => x is { Kind: "method", Access: "public", Parameters.Count: > 0 } && !x.Attributes.Any(a => a.Name == "WolverineIgnore")))
            {
                var named = MethodNames.Contains(method.Name);
                if (discovered && (named || method.Attributes.Any(a => a.Name == "WolverineHandler")))
                {
                    index.Discovered.Add(code, new WolverineHandler(type, method));
                }
                else if (!discovered && named)
                {
                    index.Undiscovered.Add(code, new WolverineHandler(type, method));
                }
            }
        }

        return index;
    }

    private static bool IsHandlerType(CodeGraph code, TypeNode type) =>
        type is { IsAbstract: false, Access: "public", TypeParameters.Count: 0 }
        && !type.Attributes.Any(x => x.Name == "WolverineIgnore")
        && (type.Name.EndsWith("Handler", StringComparison.Ordinal) || type.Name.EndsWith("Consumer", StringComparison.Ordinal)
            || type.Attributes.Any(x => x.Name == "WolverineHandler") || code.Supertypes(type).Contains("IWolverineHandler"));

    /// <summary>Whether Wolverine is in the code, and its handler methods in the code, each by the message it takes.</summary>
    private sealed class Handlers
    {
        public required bool InCode { get; init; }

        public ByMessage Discovered { get; } = new();

        public ByMessage Undiscovered { get; } = new();
    }

    /// <summary>Handler methods by the message their first parameter names: an in-scope type, or the name of one the code doesn't declare.</summary>
    private sealed class ByMessage
    {
        private readonly Dictionary<TypeNode, List<WolverineHandler>> _resolved = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<string, List<WolverineHandler>> _named = new(StringComparer.Ordinal);

        public void Add(CodeGraph code, WolverineHandler handler)
        {
            if (handler.Method.Parameters[0].Type is not { } message)
            {
                return;
            }

            if (code.TypeFor(message, handler.Type) is { } resolved)
            {
                Add(_resolved, resolved, handler);
            }
            else if (message is { Arguments.Count: 0, IsArray: false })
            {
                Add(_named, message.Name, handler);
            }
        }

        public IReadOnlyList<WolverineHandler> Of(TypeNode request) =>
            (_resolved.GetValueOrDefault(request) ?? []).Concat(_named.GetValueOrDefault(request.Name) ?? []).Distinct().ToList();

        private static void Add<TKey>(Dictionary<TKey, List<WolverineHandler>> handlers, TKey message, WolverineHandler handler) where TKey : notnull
        {
            if (!handlers.TryGetValue(message, out var list))
            {
                handlers[message] = list = [];
            }

            list.Add(handler);
        }
    }
}

/// <summary>
/// A Wolverine request's result is what its handler returns, unwrapped from <c>Task&lt;T&gt;</c> or <c>ValueTask&lt;T&gt;</c>, since
/// <c>InvokeAsync&lt;T&gt;</c> answers with the handler's return value; a handler that returns nothing (<c>void</c>, <c>Task</c>,
/// <c>ValueTask</c>) states none. Compared as types, without nullability. With no handler, or several, the request's handler is
/// <c>request.handler</c>'s finding, so this claim makes none of its own.
/// </summary>
internal sealed class WolverineResultReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["request.returns"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Located<TypeNode>() is not { } type || WolverineCode.HandlersOf(context.Code, type) is not { } handlers)
        {
            return null;
        }

        if (handlers.Count == 0)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on request.handler: a Wolverine request's result is its handler's, and no handler takes `{type.Name}`.");
        }

        var results = handlers.Select(x => (Handler: x, Result: WolverineCode.ResultOf(x.Method))).ToList();
        var (handler, result) = results[0];
        if (handlers.Count > 1 && results.Any(x => x.Result.Unreadable is not null || (x.Result.Type?.Text ?? "") != (result.Type?.Text ?? "")))
        {
            return ClaimReading.CantVerify("blocked", $"Waits on request.handler: {handlers.Count} Wolverine handlers take `{type.Name}`, and they don't return the same result.");
        }

        if (result.Unreadable is { } why)
        {
            return ClaimReading.CantVerify("unreadable",
                $"`{handler.Name}` ({handler.Method.At}) returns `{handler.Method.Type!.Text}`, {why}, so which result answers `{type.Name}` can't be read.", handler.Method.At);
        }

        var model = context.ExpectedValue as ITypeReference;
        var same = model is null ? result.Type is null : result.Type is not null && context.TypeMatches(model, result.Type);
        var actual = result.Type?.Text ?? "none";
        var expected = model is null ? "nothing" : $"`{context.Expected}`";
        return ClaimReading.Compared(actual, handler.Method.At, same, same ? null
            : $"`{context.Element.Name}` returns {expected} in the model, but its Wolverine handler `{handler.Name}` ({handler.Method.At}) returns `{handler.Method.Type?.Text ?? "void"}`, so it returns {(result.Type is null ? "nothing" : $"`{actual}`")}. {(result.Type is null ? $"Clear the {context.Element.SpecializationType}'s type" : $"Set the {context.Element.SpecializationType}'s type to `{actual}`")} if the code is right.");
    }
}

/// <summary>
/// A Wolverine request's handler is the one handler method Wolverine discovers whose first parameter is the request. Two or more
/// can't be told apart from the model's one handler; none is contradicted, naming a method that would handle it if Wolverine
/// discovered its class.
/// </summary>
internal sealed class WolverineHandlerReader : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["request.handler"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Located<TypeNode>() is not { } type || WolverineCode.HandlersOf(context.Code, type) is not { } handlers)
        {
            return null;
        }

        switch (handlers.Count)
        {
            case 1:
                return ClaimReading.Compared("present", handlers[0].Method.At, true);
            case > 1:
                return ClaimReading.CantVerify("ambiguous", $"{handlers.Count} Wolverine handlers take `{type.Name}` ({string.Join(", ", handlers.Select(x => $"`{x.Name}`"))}), and the model states one.",
                    candidates: handlers.Select(x => x.Method.At).ToList());
        }

        if (RequestCode.HandlerOutOfScope(context, type) is { } shadow)
        {
            return ClaimReading.CantVerify("outOfScope", $"No in-scope Wolverine handler takes `{type.Name}`; {shadow} may hold one, but the Drift Scope leaves it out, so it isn't read.");
        }

        var why = WolverineCode.Undiscovered(context.Code, type) is [var other, ..]
            ? $" `{other.Name}` ({other.Method.At}) takes it, but Wolverine doesn't discover `{other.Type.Name}`: it isn't a public, concrete, non-generic class whose name ends in `Handler` or `Consumer`, nor marked as a handler."
            : "";
        return ClaimReading.Compared("absent", type.At, false,
            $"`{context.Element.Name}`'s handler isn't in the code: no Wolverine handler takes `{type.Name}` (a public `Handle`, `HandleAsync`, `Consume` or `ConsumeAsync` method whose first parameter is it, on a class whose name ends in `Handler` or `Consumer`), so Wolverine can't handle it.{why} Remove the {context.Element.SpecializationType} if nothing handles it.");
    }
}
