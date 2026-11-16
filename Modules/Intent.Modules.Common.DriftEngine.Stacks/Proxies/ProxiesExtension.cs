using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Proxies;

/// <summary>
/// Reads what a front end's service proxy sends (the <c>proxy.*</c> claims) from .NET's own clients: the request an
/// <c>HttpClient</c> method makes (its verb, the route its URI is, its headers and API version), and the rpc a generated gRPC
/// client is called with, and the service that client is for. A proxy Operation is its method; the methods of its own type
/// that method calls are read with it. It answers only for a method that makes such a call, so a proxy written another way is
/// left to a house's rule.
/// </summary>
public sealed class ProxiesExtension : IDriftExtension
{
    public string Id => "intent.proxies";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new ProxyRequests()];
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
        var element = context.Element;
        if (element.ParentElement is not { } proxy || (context.TypeOf(proxy) ?? ClassNamed(context.Code, proxy.Name)) is not { } type
            || (context.MemberOf(element) ?? MethodOf(context.Code, type, element)) is not { Kind: "method" } method)
        {
            return null;
        }

        var file = context.Code.Files.FirstOrDefault(x => x.Path == type.At.File);
        var calls = Reached(context.Code, type, method).SelectMany(x => x.Calls).ToList();
        var http = calls.Where(x => IsHttpClient(x) && Verbs.ContainsKey(x.Name)).ToList();
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
                var call = http.Concat(grpc).First();
                return ClaimReading.Compared("present", call.At, context.Expected == "present", $"{method.Name} sends a request ({call.Text}) at {call.At.File}:{call.At.Line}.");
            }
            case "proxy.rpc":
                return grpc.FirstOrDefault() is { } called
                    ? ClaimReading.Compared(TrimAsync(called.Name), called.At, TrimAsync(called.Name) == context.Expected)
                    : ClaimReading.Absent(http[0].At);
            case "proxy.grpcService":
            {
                if (grpc.FirstOrDefault() is not { } rpc)
                {
                    return ClaimReading.Absent(http[0].At);
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
        var (verb, uri) = Request(context.Code, request);
        switch (context.Kind)
        {
            case "proxy.verb":
                return verb is null
                    ? ClaimReading.CantVerify("unreadable", "The request's method is computed.", request.At)
                    : ClaimReading.Compared(verb, request.At, string.Equals(verb, context.Expected, StringComparison.OrdinalIgnoreCase));
            case "proxy.route":
                return Route(uri) is { } route
                    ? ClaimReading.Stated(route, request.At)
                    : ClaimReading.CantVerify("unreadable", "The request's URI is computed.", request.At);
            case "proxy.header":
            {
                var headers = calls.Where(x => x.Name is "Add" or "TryAddWithoutValidation" && x.Receiver?.EndsWith("Headers", StringComparison.Ordinal) == true && x.Arguments.Count > 0)
                    .Select(x => (Name: context.Code.StringOf(x.Arguments[0]) is { Kind: "string", Value: { } name } ? name : null, x.At))
                    .Where(x => x.Name is not null)
                    .ToList();
                return headers.Count == 0
                    ? ClaimReading.Absent(request.At)
                    : ClaimReading.Stated(string.Join(", ", headers.Select(x => x.Name).Order(StringComparer.Ordinal)), headers[0].At);
            }
            default:
            {
                // An api-version in the request's query string, else the one the proxy's AddHttpClient registration adds to every request.
                if (Text(uri) is { } text && Regex.Match(text, @"[?&]api-version=([^&#]+)") is { Success: true } query)
                {
                    return ClaimReading.Stated(query.Groups[1].Value, request.At);
                }

                return Registered(context.Code, proxy.Name) is { } registered
                    ? ClaimReading.Stated(registered.Version, registered.At)
                    : ClaimReading.Absent(request.At);
            }
        }
    }

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
    private static (string? Verb, LiteralNode? Uri) Request(CodeGraph code, CallNode call)
    {
        if (Verbs[call.Name] is { Length: > 0 } verb)
        {
            return (verb, call.Arguments.Count > 0 ? code.StringOf(call.Arguments[0]) : null);
        }

        // SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)), through a local.
        if (call.Arguments.FirstOrDefault()?.Creations.FirstOrDefault(x => x.Type.Name == "HttpRequestMessage") is { } message && message.Arguments.Count > 0)
        {
            var method = Regex.Match(message.Arguments[0].Text, @"HttpMethod\.(\w+)") is { Success: true } named ? named.Groups[1].Value.ToUpperInvariant() : null;
            return (method, message.Arguments.Count > 1 ? code.StringOf(message.Arguments[1]) : null);
        }

        return (null, null);
    }

    /// <summary>A URI's path as a route: each value in it a <c>{hole}</c>, its query string and leading <c>/</c> dropped.</summary>
    private static string? Route(LiteralNode? uri) => Text(uri) is { } text ? text.Split('?', '#')[0].Trim('/') : null;

    private static string? Text(LiteralNode? literal) => literal switch
    {
        { Kind: "string", Value: { } text } => text,
        { Kind: "interpolated", Parts: [{ Literal: { Kind: "string" } }, ..] parts } => string.Concat(parts.Select(x => x.Literal is { Kind: "string", Value: { } text } ? text : $"{{{x.Text}}}")),
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
