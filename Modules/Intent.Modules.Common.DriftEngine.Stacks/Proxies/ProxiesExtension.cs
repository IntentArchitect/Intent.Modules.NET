using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Http;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;

namespace Intent.Modules.Common.DriftEngine.Stacks.Proxies;

/// <summary>
/// Reads what a front end's C# service proxy sends (the <c>proxy.*</c> claims) from .NET's own clients: the request an
/// <c>HttpClient</c> method makes (its verb, the route its URI is, its headers and API version), directly or through the code's
/// own request wrapper, and the rpc a generated gRPC client is called with, and the service that client is for. A proxy
/// Operation is its method; the methods of its own type that method calls are read with it. It answers only for a method that
/// makes such a call, so a proxy written another way is left to a house's rule.
/// </summary>
public sealed class ProxiesExtension : IDriftExtension
{
    public string Id => "intent.proxies";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new ProxyRequests()];

    public IEnumerable<ICodeReader> Readers { get; } = [new ProxyMethods()];
}

/// <summary>
/// A User Interface Proxy's C# method that sends no request (a <c>Dispose</c>, a mapping helper) can't be a Proxy Operation,
/// which the designer requires to call a backend operation or describe an HTTP request (<see cref="CodeGraph.Unstatable"/>),
/// so <c>unmodelled.member</c> doesn't ask for one. A method with no body, an interface's, is left to the rule: what it sends
/// can't be read.
/// </summary>
internal sealed class ProxyMethods : ICodeReader
{
    public void Read(CodeGraph graph) => graph.Unstatable.Add((elementType, type, member) =>
        elementType == "Proxy" && member is { Kind: "method", Body: not null } && type.Files.Any(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        && !ProxyRequests.Sends(graph, type, member)
            ? "it sends no request, and a Proxy Operation calls a backend operation or describes an HTTP request"
            : null);
}

internal sealed class ProxyRequests : IClaimReader
{
    private static readonly Dictionary<string, string> Verbs = new(StringComparer.Ordinal)
    {
        ["GetAsync"] = "GET", ["GetFromJsonAsync"] = "GET", ["GetStringAsync"] = "GET", ["GetStreamAsync"] = "GET", ["GetByteArrayAsync"] = "GET",
        ["PostAsync"] = "POST", ["PostAsJsonAsync"] = "POST",
        ["PutAsync"] = "PUT", ["PutAsJsonAsync"] = "PUT",
        ["PatchAsync"] = "PATCH", ["PatchAsJsonAsync"] = "PATCH",
        ["DeleteAsync"] = "DELETE", ["DeleteFromJsonAsync"] = "DELETE",
        ["SendAsync"] = ""
    };

    public IReadOnlyCollection<string> Kinds { get; } = ["proxy.request", "proxy.verb", "proxy.route", "proxy.header", "proxy.version", "proxy.rpc", "proxy.grpcService"];

    public ClaimReading? Read(ClaimContext context)
    {
        // The User Interface designer (2.0) claims on a Proxy Operation's Call Service Operation Action, whose target end is the
        // claim's element: the proxy operation is the link's source. A house pack may claim on the operation itself.
        var element = context.Element is IAssociationEnd end ? context.Owner(end) : context.Element;
        if (element?.ParentElement is not { } proxy || (context.TypeOf(proxy) ?? ClassNamed(context.Code, proxy.Name)) is not { } type
            || !type.Files.Any(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            || (context.MemberOf(element) ?? MethodOf(context.Code, type, element)) is not { Kind: "method" } method)
        {
            return null;
        }

        var file = context.Code.Files.FirstOrDefault(x => x.Path == type.At.File);
        var reached = Reached(context.Code, type, method);
        var calls = reached.SelectMany(x => x.Calls).ToList();
        var http = Requests(context.Code, type, reached);
        var grpc = calls.Where(x => GrpcClient(x, file) is not null).ToList();
        if (http.Count == 0 && grpc.Count == 0)
        {
            // Neither kind of client is called: a proxy that makes no request states none of them.
            return context.Kind is "proxy.request" or "proxy.rpc" or "proxy.grpcService" ? ClaimReading.Absent(method.At) : null;
        }

        switch (context.Kind)
        {
            case "proxy.request":
            {
                var call = http.Select(x => x.Call).Concat(grpc).First();
                return ClaimReading.Compared("present", call.At, context.Expected == "present", $"{method.Name} sends a request ({call.Text}) at {call.At.File}:{call.At.Line}.");
            }
            case "proxy.rpc":
                return grpc.FirstOrDefault() is { } called
                    ? ClaimReading.Compared(TrimAsync(called.Name), called.At, TrimAsync(called.Name) == context.Expected)
                    : ClaimReading.Absent(http[0].Call.At);
            case "proxy.grpcService":
            {
                if (grpc.FirstOrDefault() is not { } rpc)
                {
                    return ClaimReading.Absent(http[0].Call.At);
                }

                var client = GrpcClient(rpc, file)!;
                var service = client.Split('.') is { Length: > 1 } parts ? parts[^2] : client;
                return ClaimReading.Compared(service, ClientAt(type, rpc), service == context.Expected);
            }
        }

        if (http.Count == 0)
        {
            return null;
        }

        var request = http[0];
        var at = request.Call.At;
        switch (context.Kind)
        {
            case "proxy.verb":
                return request.Verb is not { } verb
                    ? ClaimReading.CantVerify("unreadable", "The request's method is computed.", at)
                    : ClaimReading.Compared(verb, at, string.Equals(verb, context.Expected, StringComparison.OrdinalIgnoreCase));
            case "proxy.route":
                return Route(request.Uri) is { } route
                    ? ClaimReading.Compared(route.Shown, at, RouteMatches(route.Segments, context.Expected))
                    : ClaimReading.CantVerify("unreadable", "The request's URI is computed.", at);
            case "proxy.header":
            {
                var headers = calls.Where(x => x.Name is "Add" or "TryAddWithoutValidation" && x.Receiver?.EndsWith("Headers", StringComparison.Ordinal) == true && x.Arguments.Count > 0)
                    .Select(x => (Name: context.Code.StringOf(x.Arguments[0]) is { Kind: "string", Value: { } name } ? name : null, x.At))
                    .Where(x => x.Name is not null)
                    .ToList();
                return headers.Count == 0
                    ? ClaimReading.Absent(at)
                    : ClaimReading.Stated(string.Join(", ", headers.Select(x => x.Name).Order(StringComparer.Ordinal)), headers[0].At);
            }
            default:
            {
                // An api-version in the request's query string, else the one the proxy's AddHttpClient registration adds to every
                // request. The model may allow several versions; the request sends one of them.
                var (version, stated) = Text(request.Uri) is { } text && Regex.Match(text, @"[?&]api-version=([^&#]+)") is { Success: true } query
                    ? (query.Groups[1].Value, at)
                    : Registered(context.Code, proxy.Name) is { } registered ? (registered.Version, registered.At) : (null, at);
                return version is null
                    ? ClaimReading.Absent(at)
                    : ClaimReading.Compared(version, stated, context.Expected.Split(',', StringSplitOptions.TrimEntries).Any(x => HttpExtension.Version(x) == HttpExtension.Version(version)));
            }
        }
    }

    /// <summary>Whether the method, or a method of its own type it calls, sends an HTTP request or calls a gRPC client.</summary>
    internal static bool Sends(CodeGraph code, TypeNode type, MemberNode method)
    {
        var reached = Reached(code, type, method);
        var file = code.Files.FirstOrDefault(x => x.Path == type.At.File);
        return reached.SelectMany(x => x.Calls).Any(x => GrpcClient(x, file) is not null) || Requests(code, type, reached).Count > 0;
    }

    /// <summary>
    /// A request the proxy method sends: the call in its own code that sends it (where it is reported), the verb, and the URI
    /// as the call gives it.
    /// </summary>
    private sealed record Sent(CallNode Call, string? Verb, LiteralNode? Uri);

    /// <summary>
    /// The requests the method and the methods of its own type it calls send: an <c>HttpClient</c> call, or a call of the code's
    /// own request wrapper (<c>_requestProvider.PutAsync(uri, order)</c>, through its interface to an implementation) whose
    /// implementation sends an <c>HttpClient</c> request to a URI it is given, read with the wrapper's parameters bound to the
    /// call's arguments. A wrapper that sends a URI of its own (a token service's) isn't the proxy's request.
    /// </summary>
    private static List<Sent> Requests(CodeGraph code, TypeNode type, IReadOnlyList<MemberNode> reached)
    {
        var requests = new List<Sent>();
        foreach (var call in reached.SelectMany(x => x.Calls))
        {
            if (IsHttpClient(call) && Verbs.ContainsKey(call.Name))
            {
                var (verb, uri) = Request(call);
                requests.Add(new Sent(call, verb, uri is null ? null : code.StringOf(uri)));
                continue;
            }

            if (call.Target?.Type == type)
            {
                continue;
            }

            foreach (var (wrapper, body) in MessageCode.Bodies(code, type, call).Where(x => x.Type != type))
            {
                var sent = body.Calls.Where(x => IsHttpClient(x) && Verbs.ContainsKey(x.Name))
                    .Select(x => Request(x))
                    .FirstOrDefault(x => x.Uri is { } uri && Given(uri, body));
                if (sent.Uri is { } given)
                {
                    requests.Add(new Sent(call, sent.Verb, code.StringOf(given, [(call, body)])));
                    break;
                }
            }
        }

        return requests;
    }

    /// <summary>The value comes from one of the method's parameters: as it is, or through a local, a join or an interpolation.</summary>
    private static bool Given(ValueNode value, MemberNode method, int depth = 0) =>
        value.Parameter is { } parameter && method.Parameters.Contains(parameter)
        || depth < 4 && (value.Given.Concat(value.Joined).Concat(value.Literal?.Parts ?? []).Concat(value.Creations.SelectMany(x => x.Arguments))
            .Append(value.Trimmed?.Value).OfType<ValueNode>().Any(x => Given(x, method, depth + 1)));

    /// <summary>
    /// The proxy's class, where no claim of a vocabulary locates it (a house pack that claims only what its proxies send): the
    /// one in-scope class of its name.
    /// </summary>
    private static TypeNode? ClassNamed(CodeGraph code, string name) =>
        code.Types.Where(x => x.Name == name && x.Kind is "class" or "record").ToList() is [var only] ? only : null;

    /// <summary>The Operation's method: of its name, one whose parameters are the model's by name when it is overloaded.</summary>
    private static MemberNode? MethodOf(CodeGraph code, TypeNode type, IElement operation)
    {
        var methods = code.MembersOf(type).Where(x => x.Kind == "method" && x.Name == operation.Name).ToList();
        var parameters = operation.ChildElements.Where(x => x.SpecializationType == "Parameter").Select(x => x.Name).ToList();
        return methods.Count == 1 ? methods[0] : methods.FirstOrDefault(x => x.Parameters.Select(p => p.Name).SequenceEqual(parameters));
    }

    /// <summary>The method and the methods of its own type it calls, each once.</summary>
    private static IReadOnlyList<MemberNode> Reached(CodeGraph code, TypeNode type, MemberNode method)
    {
        var reached = new List<MemberNode> { method };
        for (var i = 0; i < reached.Count; i++)
        {
            foreach (var call in reached[i].Calls)
            {
                if (call.Target is { } target && target.Type == type && !reached.Contains(target.Member))
                {
                    reached.Add(target.Member);
                }
            }
        }

        return reached;
    }

    private static bool IsHttpClient(CallNode call) => call.ReceiverType?.Name == "HttpClient"
                                                       || call.ReceiverType is null && call.Receiver is { } receiver && Regex.IsMatch(receiver, @"(?i)(^|\.)_?http(client)?$");

    /// <summary>The generated gRPC client a call is made on, qualified as far as the code says (an alias's target): <c>eShop.Basket.API.Grpc.Basket.BasketClient</c>.</summary>
    private static string? GrpcClient(CallNode call, FileNode? file)
    {
        if (call.ReceiverType is not { } receiver || receiver.Name == "HttpClient" || !call.Name.EndsWith("Async", StringComparison.Ordinal))
        {
            return null;
        }

        var name = file?.Imports.FirstOrDefault(x => x.Alias == receiver.Name)?.Target ?? receiver.QualifiedName;
        return name.EndsWith("Client", StringComparison.Ordinal) && name.Contains('.') ? name : null;
    }

    /// <summary>Where the proxy holds its client: the field, property or primary constructor parameter the call is made on.</summary>
    private static Span ClientAt(TypeNode type, CallNode call)
    {
        var holder = call.Receiver?.TrimStart('_');
        return type.Members.FirstOrDefault(x => x.Kind is "field" or "property" && x.Name.TrimStart('_') == holder)?.At
               ?? type.Members.Where(x => x.IsPrimary).SelectMany(x => x.Parameters).FirstOrDefault(x => x.Name == holder)?.At
               ?? type.At;
    }

    /// <summary>A request's verb and URI: a verb method's own and its first argument, or a sent HttpRequestMessage's.</summary>
    private static (string? Verb, ValueNode? Uri) Request(CallNode call)
    {
        if (Verbs[call.Name] is { Length: > 0 } verb)
        {
            return (verb, call.Arguments.FirstOrDefault());
        }

        // SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)), through a local.
        if (call.Arguments.FirstOrDefault()?.Creations.FirstOrDefault(x => x.Type.Name == "HttpRequestMessage") is { } message && message.Arguments.Count > 0)
        {
            var method = Regex.Match(message.Arguments[0].Text, @"HttpMethod\.(\w+)") is { Success: true } named ? named.Groups[1].Value.ToUpperInvariant() : null;
            return (method, message.Arguments.ElementAtOrDefault(1));
        }

        return (null, null);
    }

    private const char HoleStart = '\u0001';
    private const char HoleEnd = '\u0002';

    /// <summary>
    /// A URI's path as a route, shown with each value the code computes as <c>{value}</c>, and its segments, each value a hole:
    /// without a scheme and host, a query string, or a leading value before any text, which is a base address the
    /// application's settings give (<c>{GatewayOrdersEndpointBase}api/orders</c>). Empty segments are dropped, as the model
    /// can't state one (<c>api/catalog//items</c>). Null when the code computes it whole.
    /// </summary>
    internal static (string Shown, IReadOnlyList<string> Segments)? Route(LiteralNode? uri)
    {
        var parts = uri switch
        {
            { Kind: "string", Value: { } text } => [text],
            { Kind: "interpolated", Parts: { } values } => values.Select((x, i) => x.Literal is { Kind: "string", Value: { } text } ? text : $"{HoleStart}{i}{HoleEnd}").ToList(),
            _ => (List<string>?)null
        };
        if (parts is null || parts.All(x => x.StartsWith(HoleStart)))
        {
            return null;
        }

        var path = string.Concat(parts.SkipWhile(x => x.StartsWith(HoleStart) && parts.Count > 1));
        path = Regex.Replace(path, @"^[a-zA-Z][a-zA-Z0-9+.-]*://[^/]*", "").Split('?', '#')[0];
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        var shown = Regex.Replace(string.Join("/", segments), $@"{HoleStart}(\d+){HoleEnd}", m => $"{{{((LiteralNode)uri!).Parts![int.Parse(m.Groups[1].Value)].Text}}}");
        return (shown, segments);
    }

    /// <summary>
    /// The code's route against the model's, segment by segment, as ASP.NET Core matches one: a literal without case, a model's
    /// parameter against a value the code computes, and an optional parameter at the end (<c>{id?}</c>) may be left out. A query
    /// string the model gives the service's route (<c>?api-version={version}</c>) isn't part of the path.
    /// </summary>
    internal static bool RouteMatches(IReadOnlyList<string> code, string expected)
    {
        var model = Regex.Replace(expected, @"\?(?!\})[^/]*", "").Split('/', StringSplitOptions.RemoveEmptyEntries).Where(x => x != "~").ToList();
        while (model.Count > code.Count && model[^1] is ['{', .., '?', '}'])
        {
            model.RemoveAt(model.Count - 1);
        }

        return model.Count == code.Count && model.Zip(code).All(x => x.First.StartsWith('{')
            ? x.Second.Contains(HoleStart)
            : !x.Second.Contains(HoleStart) && string.Equals(x.First, x.Second, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>A URI as text, each value in it <c>{value}</c>, for reading its query string.</summary>
    private static string? Text(LiteralNode? literal) => literal switch
    {
        { Kind: "string", Value: { } text } => text,
        { Kind: "interpolated", Parts: { } parts } => string.Concat(parts.Select(x => x.Literal is { Kind: "string", Value: { } text } ? text : $"{{{x.Text}}}")),
        _ => null
    };

    /// <summary>The API version an <c>AddHttpClient&lt;Proxy&gt;(...)</c> registration adds with <c>AddApiVersion</c>.</summary>
    private static (string Version, Span At)? Registered(CodeGraph code, string proxy)
    {
        foreach (var call in code.Types.SelectMany(x => x.Members).SelectMany(x => x.Calls).Concat(code.Files.Select(x => x.TopLevel).OfType<FunctionNode>().SelectMany(x => x.Calls)))
        {
            if (call.Name != "AddApiVersion" || call.Arguments.Count == 0)
            {
                continue;
            }

            for (var on = call.On; on is not null; on = on.On)
            {
                if (on.Name == "AddHttpClient" && on.TypeArguments.Any(x => x.Name == proxy))
                {
                    return (call.Arguments[0].Literal?.Value ?? call.Arguments[0].Text, call.At);
                }
            }
        }

        return null;
    }

    private static string TrimAsync(string name) => name.EndsWith("Async", StringComparison.Ordinal) ? name[..^"Async".Length] : name;
}
