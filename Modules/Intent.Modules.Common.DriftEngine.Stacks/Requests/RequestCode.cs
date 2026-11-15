using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Requests;

/// <summary>
/// A type reference as a type's bases give it, with the base types' own type parameters replaced by what the type passes them,
/// and the type whose code wrote it, where its name resolves.
/// </summary>
internal sealed record Bound(TypeRefNode Type, TypeNode From, IReadOnlyList<Bound> Arguments)
{
    /// <summary>A library's type of the name, as a request framework's are: the code doesn't declare it.</summary>
    public bool IsLibrary(CodeGraph code, string name) => Type.Name == name && code.TypeFor(Type, From) is null;

    /// <summary>MediatR's and Mediator's nothing: no result, or their <c>Unit</c>.</summary>
    public static bool IsNothing(CodeGraph code, Bound? bound) => bound is null || bound.Arguments.Count == 0 && bound.IsLibrary(code, "Unit");
}

/// <summary>A request as a marker framework's code states it: what it returns (null for nothing), and the interface that says so.</summary>
internal sealed record Request(Bound Interface, Bound? Result);

/// <summary>A class that handles a request: the handler interface it implements, and the result it gives (null for nothing).</summary>
internal sealed record Handler(TypeNode Type, Bound Interface, Bound? Result);

/// <summary>What every request framework's reading shares: bases followed with their type arguments, and results compared.</summary>
internal static class RequestCode
{
    /// <summary>Everything a type derives from or implements, nearest first, its bases' type parameters given what it passes them.</summary>
    public static IEnumerable<Bound> Supertypes(CodeGraph code, TypeNode type)
    {
        var seen = new HashSet<TypeNode> { type };
        var queue = new Queue<(TypeNode Type, IReadOnlyDictionary<string, Bound> Given)>([(type, new Dictionary<string, Bound>())]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var reference in current.Type.BaseTypes)
            {
                var bound = Substitute(reference, current.Type, current.Given);
                yield return bound;
                if (code.TypeFor(reference, current.Type) is { } resolved && seen.Add(resolved))
                {
                    var given = new Dictionary<string, Bound>(StringComparer.Ordinal);
                    for (var i = 0; i < resolved.TypeParameters.Count && i < bound.Arguments.Count; i++)
                    {
                        given[resolved.TypeParameters[i]] = bound.Arguments[i];
                    }

                    queue.Enqueue((resolved, given));
                }
            }
        }
    }

    private static Bound Substitute(TypeRefNode reference, TypeNode from, IReadOnlyDictionary<string, Bound> given)
    {
        if (reference.Arguments.Count == 0)
        {
            return !reference.IsArray && given.TryGetValue(reference.Name, out var parameter) ? parameter : new Bound(reference, from, []);
        }

        var arguments = reference.Arguments.Select(x => Substitute(x, from, given)).ToList();
        if (arguments.Zip(reference.Arguments).All(x => ReferenceEquals(x.First.Type, x.Second)))
        {
            return new Bound(reference, from, arguments);
        }

        var text = $"{reference.Name}<{string.Join(", ", arguments.Select(x => x.Type.Text))}>{(reference.IsArray ? "[]" : "")}{(reference.NullableMarked ? "?" : "")}";
        return new Bound(new TypeRefNode
        {
            Text = text,
            Name = reference.Name,
            QualifiedName = reference.QualifiedName,
            Arguments = arguments.Select(x => x.Type).ToList(),
            IsArray = reference.IsArray,
            NullableMarked = reference.NullableMarked,
            Nullable = reference.Nullable,
            Language = reference.Language
        }, from, arguments);
    }

    /// <summary>Two results are the same type: both nothing, or the same type with the same arguments, whatever their nullability.</summary>
    public static bool Same(CodeGraph code, Bound? a, Bound? b)
    {
        if (Bound.IsNothing(code, a) || Bound.IsNothing(code, b))
        {
            return Bound.IsNothing(code, a) && Bound.IsNothing(code, b);
        }

        var left = code.TypeFor(a!.Type, a.From);
        var right = code.TypeFor(b!.Type, b.From);
        var named = left is not null && right is not null ? left == right : a.Type.Name == b.Type.Name;
        return named && a.Type.IsArray == b.Type.IsArray && a.Arguments.Count == b.Arguments.Count && a.Arguments.Zip(b.Arguments).All(x => Same(code, x.First, x.Second));
    }

    /// <summary>
    /// A file the Drift Scope leaves out that is named for the request's handler (<c>CancelOrderCommandHandler.cs</c>), so may
    /// hold it; null when there is none.
    /// </summary>
    public static string? HandlerOutOfScope(ClaimContext context, TypeNode request) =>
        context.Scope.Files.FirstOrDefault(x => !context.Scope.Includes(x) && Path.GetFileName(x).StartsWith(request.Name + "Handler.", StringComparison.Ordinal)
                                                && !x.Split('/').Any(s => s is "bin" or "obj" or "node_modules"));
}

/// <summary>
/// A request framework whose requests implement its marker interface and whose handlers implement its handler interface for
/// them: MediatR and Mediator. Its interfaces are a library's, found by name (the code doesn't declare them), and one framework's
/// are told from another's of the same name (both have <c>IRequest&lt;T&gt;</c> and <c>IRequestHandler&lt;X, T&gt;</c>) by the
/// namespace the code that writes them imports, or qualifies them with.
/// </summary>
internal sealed class MarkerFramework
{
    /// <summary>MediatR: <c>IRequest&lt;T&gt;</c>, handled by <c>IRequestHandler&lt;X, T&gt;</c> or, before version 12, the base classes that implement it.</summary>
    public static readonly MarkerFramework MediatR = new("MediatR", "MediatR", true, new Dictionary<string, string[]>
    {
        ["IRequest"] = ["IRequestHandler", "RequestHandler", "AsyncRequestHandler"]
    });

    /// <summary>
    /// Mediator (martinothamar/Mediator, source-generated): <c>IRequest</c>, <c>ICommand</c> and <c>IQuery</c>, each handled by
    /// its own handler interface, as its handlers' type constraints require.
    /// </summary>
    public static readonly MarkerFramework Mediator = new("Mediator", "Mediator", false, new Dictionary<string, string[]>
    {
        ["IRequest"] = ["IRequestHandler"],
        ["ICommand"] = ["ICommandHandler"],
        ["IQuery"] = ["IQueryHandler"]
    });

    /// <summary>Every marker framework, so a framework without one (Wolverine) leaves their requests to them.</summary>
    public static IReadOnlyList<MarkerFramework> All { get; } = [MediatR, Mediator];

    private readonly IReadOnlyDictionary<string, string[]> _handlers;
    private readonly HashSet<string> _handlerTypes;

    private MarkerFramework(string name, string ns, bool isDefault, IReadOnlyDictionary<string, string[]> handlers)
    {
        Name = name;
        Namespace = ns;
        IsDefault = isDefault;
        _handlers = handlers;
        _handlerTypes = handlers.Values.SelectMany(x => x).ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>As a finding names it.</summary>
    public string Name { get; }

    /// <summary>The namespace its interfaces are in.</summary>
    public string Namespace { get; }

    /// <summary>
    /// Its interfaces are the ones code writes when it imports no marker framework's namespace: MediatR's, the most common,
    /// whose <c>using</c> a project often keeps in a global file the scope may leave out.
    /// </summary>
    public bool IsDefault { get; }

    /// <summary>
    /// The interface is this framework's: qualified with its namespace, or written in code that imports it. Code that imports no
    /// marker framework's namespace writes the default framework's.
    /// </summary>
    private bool Owns(CodeGraph code, Bound bound)
    {
        var files = code.Files.Where(x => bound.From.Files.Contains(x.Path)).ToList();
        var name = bound.Type.Name;
        var qualified = bound.Type.QualifiedName.Replace("global::", "");
        if (qualified != name && qualified.EndsWith("." + name, StringComparison.Ordinal))
        {
            var qualifier = qualified[..^(name.Length + 1)];
            var aliased = files.SelectMany(code.ImportsOf).FirstOrDefault(x => x.Alias == qualifier)?.Target;
            return (aliased ?? qualifier) == Namespace;
        }

        var imported = All.Where(f => files.Any(x => code.Reaches(x, f.Namespace))).ToList();
        return imported.Count > 0 ? imported.Contains(this) : IsDefault;
    }

    /// <summary>
    /// The request interfaces of this framework the type implements, each with its result; none when it isn't this framework's
    /// request. An interface the code declares is its own, not the framework's, unless it derives the framework's in turn.
    /// </summary>
    public IReadOnlyList<Request> Requests(CodeGraph code, TypeNode type) =>
        RequestCode.Supertypes(code, type)
            .Where(x => x.Arguments.Count <= 1 && _handlers.ContainsKey(x.Type.Name) && x.IsLibrary(code, x.Type.Name) && Owns(code, x))
            .Select(x => new Request(x, x.Arguments.Count == 1 && !Bound.IsNothing(code, x.Arguments[0]) ? x.Arguments[0] : null))
            .ToList();

    /// <summary>A handler of this framework can handle a request through the interface: <c>ICommandHandler</c> handles <c>ICommand</c>.</summary>
    public bool Handles(Handler handler, Request request) => _handlers[request.Interface.Type.Name].Contains(handler.Interface.Type.Name);

    /// <summary>
    /// The classes that implement one of this framework's handlers for the request, the type it names (resolved, else by its
    /// name): both frameworks register concrete, non-generic ones. Every handler in the code is found once per run.
    /// </summary>
    public IReadOnlyList<Handler> Handlers(CodeGraph code, TypeNode request)
    {
        if (code.Get<HandlerIndexes>() is not { } indexes)
        {
            code.Set(indexes = new HandlerIndexes());
        }

        if (!indexes.TryGetValue(this, out var index))
        {
            indexes[this] = index = new HandlerIndex();
            foreach (var type in code.Types.Where(x => x.Kind is "class" or "record" or "struct" && !x.IsAbstract && x.TypeParameters.Count == 0))
            {
                foreach (var handled in RequestCode.Supertypes(code, type)
                             .Where(x => x.Arguments.Count is 1 or 2 && _handlerTypes.Contains(x.Type.Name) && x.IsLibrary(code, x.Type.Name) && Owns(code, x)))
                {
                    var handler = new Handler(type, handled, handled.Arguments.Count == 2 && !Bound.IsNothing(code, handled.Arguments[1]) ? handled.Arguments[1] : null);
                    var target = handled.Arguments[0];
                    if (code.TypeFor(target.Type, target.From) is { } resolved)
                    {
                        index.Add(index.Resolved, resolved, handler);
                    }
                    else if (target.Arguments.Count == 0)
                    {
                        index.Add(index.Named, target.Type.Name, handler);
                    }
                }
            }
        }

        return (index.Resolved.GetValueOrDefault(request) ?? []).Concat(index.Named.GetValueOrDefault(request.Name) ?? []).DistinctBy(x => x.Type).ToList();
    }

    /// <summary>
    /// A request's handler interface as the code would write it: <c>IRequestHandler&lt;X, R&gt;</c>, or <c>IRequestHandler&lt;X&gt;</c>
    /// for none; Mediator's <c>ICommandHandler&lt;X, R&gt;</c> for an <c>ICommand&lt;R&gt;</c>.
    /// </summary>
    public string HandlerFor(TypeNode type, Request request) =>
        request.Result is null ? $"{_handlers[request.Interface.Type.Name][0]}<{type.Name}>" : $"{_handlers[request.Interface.Type.Name][0]}<{type.Name}, {request.Result.Type.Text}>";

    /// <summary>Every marker framework's handlers in the code, each by the request it handles.</summary>
    private sealed class HandlerIndexes : Dictionary<MarkerFramework, HandlerIndex>;

    /// <summary>Every handler of one framework in the code, by the request it handles: an in-scope type, or the name of one it doesn't declare.</summary>
    private sealed class HandlerIndex
    {
        public Dictionary<TypeNode, List<Handler>> Resolved { get; } = new(ReferenceEqualityComparer.Instance);

        public Dictionary<string, List<Handler>> Named { get; } = new(StringComparer.Ordinal);

        public void Add<TKey>(Dictionary<TKey, List<Handler>> handlers, TKey request, Handler handler) where TKey : notnull
        {
            if (!handlers.TryGetValue(request, out var list))
            {
                handlers[request] = list = [];
            }

            list.Add(handler);
        }
    }
}

/// <summary>
/// The result a marker framework's request returns is the <c>R</c> of its <c>IRequest&lt;R&gt;</c> (Mediator's <c>ICommand&lt;R&gt;</c>
/// and <c>IQuery&lt;R&gt;</c> too); <c>IRequest</c> and <c>IRequest&lt;Unit&gt;</c> return nothing. Compared as types, without nullability.
/// </summary>
internal sealed class MarkerResultReader(MarkerFramework framework) : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["request.returns"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Located<TypeNode>() is not { } type || framework.Requests(context.Code, type) is not { Count: > 0 } requests)
        {
            return null;
        }

        if (requests.DistinctBy(x => x.Result?.Type.Text ?? "").Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"`{type.Name}` implements {string.Join(" and ", requests.Select(x => $"`{x.Interface.Type.Text}`"))}, so which result it returns can't be read.", type.At);
        }

        var result = requests[0].Result;
        var model = context.ExpectedValue as ITypeReference;
        var same = model is null ? result is null : result is not null && context.TypeMatches(model, result.Type);
        var actual = result?.Type.Text ?? "none";
        var expected = model is null ? "nothing" : $"`{context.Expected}`";
        return ClaimReading.Compared(actual, type.At, same, same ? null
            : $"`{context.Element.Name}` returns {expected} in the model, but `{type.Name}` ({type.At}) is `{requests[0].Interface.Type.Text}`, so it returns {(result is null ? "nothing" : $"`{actual}`")}. {(result is null ? $"Clear the {context.Element.SpecializationType}'s type" : $"Set the {context.Element.SpecializationType}'s type to `{actual}`")} if the code is right.");
    }
}

/// <summary>
/// A marker framework's request's handler is the one class that implements the framework's handler interface for it with the
/// result the request declares (<c>IRequestHandler&lt;X&gt;</c> for none): a handler of another result is one the framework never
/// finds. A result the model states differently is <c>request.returns</c>'s finding, not this one's.
/// </summary>
internal sealed class MarkerHandlerReader(MarkerFramework framework) : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["request.handler"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Located<TypeNode>() is not { } type || framework.Requests(context.Code, type) is not { Count: > 0 } requests)
        {
            return null;
        }

        var request = requests[0];
        var others = framework.Handlers(context.Code, type);
        var handlers = others.Where(x => requests.Any(r => framework.Handles(x, r)) && RequestCode.Same(context.Code, x.Result, request.Result)).ToList();
        switch (handlers.Count)
        {
            case 1:
                return ClaimReading.Compared("present", handlers[0].Type.At, true);
            case > 1:
                return ClaimReading.CantVerify("ambiguous", $"{handlers.Count} classes handle `{type.Name}`, and {framework.Name} uses one.", candidates: handlers.Select(x => x.Type.At).ToList());
        }

        var expected = framework.HandlerFor(type, request);
        if (RequestCode.HandlerOutOfScope(context, type) is { } shadow)
        {
            return ClaimReading.CantVerify("outOfScope", $"No in-scope class implements `{expected}`; {shadow} may, but the Drift Scope leaves it out, so it isn't read.");
        }

        var why = others.Count > 0
            ? $" `{others[0].Type.Name}` ({others[0].Type.At}) implements `{others[0].Interface.Type.Text}`, which {framework.Name} never uses for `{request.Interface.Type.Text}`."
            : "";
        return ClaimReading.Compared("absent", type.At, false,
            $"`{context.Element.Name}`'s handler isn't in the code: no class implements `{expected}`, so {framework.Name} can't handle it.{why} Remove the {context.Element.SpecializationType} if nothing handles it.");
    }
}
