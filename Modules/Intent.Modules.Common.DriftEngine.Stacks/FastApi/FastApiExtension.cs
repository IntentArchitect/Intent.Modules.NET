using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Python;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.FastApi;

/// <summary>
/// How FastAPI code states the HTTP claims (<see cref="Http.HttpExtension"/>), as <c>intent.springmvc</c> reads Spring's: a
/// router module (<c>router = APIRouter(prefix="/orders")</c>) is a Service's code, its decorated functions the Operations'
/// path operations, with the prefixes the routers including it add. Where a parameter binds is FastAPI's rule: a path
/// parameter, a <c>Query</c>/<c>Form</c>/<c>Body</c>/<c>Header</c> marker, a Pydantic model as the body, else the query; what
/// FastAPI injects (a <c>Depends</c>, the request) isn't a parameter of the operation. It answers only for FastAPI's code.
/// </summary>
public sealed class FastApiExtension : IDriftExtension
{
    public string Id => "intent.fastapi";

    public IEnumerable<ICodeReader> Readers { get; } = [new FastApiReader()];

    public IEnumerable<IMemberRefiner> Refiners { get; } = [new FastApiParameters()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new FastApiClaims()];
}

/// <summary>Reads <see cref="FastApiSite"/> once the Python language has read the code.</summary>
internal sealed class FastApiReader : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        if (FastApiSite.Read(graph) is { } site)
        {
            graph.Set(site);
        }
    }
}

/// <summary>
/// A path operation's parameters as its HTTP contract states them: what FastAPI injects (a <c>Depends</c> or <c>Security</c>
/// dependency, <c>Request</c>, <c>Response</c>, <c>BackgroundTasks</c>) isn't one of them, as C#'s <c>CancellationToken</c> isn't.
/// </summary>
internal sealed class FastApiParameters : IMemberRefiner
{
    public IReadOnlyCollection<string> Kinds { get; } = ["operation.parameters", "parameter.type", "parameter.nullable", "parameter.default"];

    public MemberNode? Refine(ClaimContext context)
    {
        var member = context.Member;
        if (FastApiSite.Of(context.Code) is not { } site || site.EndpointOf(member) is null || !member!.Parameters.Any(FastApiSite.IsInjected))
        {
            return null;
        }

        return member with { Parameters = member.Parameters.Where(x => !FastApiSite.IsInjected(x)).ToList() };
    }
}

/// <summary>
/// What a FastAPI path operation states over HTTP: that it is one, its verb and route (under every prefix its routers and the
/// <c>include_router</c> calls add), where each parameter binds and by what name, its status, whether OpenAPI shows it, its
/// security and what it returns (its <c>response_model</c>; a 204's <c>-&gt; Response</c> is nothing); and that a router module
/// is a Service's code.
/// </summary>
internal sealed class FastApiClaims : IClaimReader
{
    /// <summary>FastAPI's and Starlette's responses: a path operation annotated with one returns no data of its own.</summary>
    private static readonly HashSet<string> Responses =
        ["Response", "JSONResponse", "HTMLResponse", "PlainTextResponse", "RedirectResponse", "StreamingResponse", "FileResponse", "ORJSONResponse", "UJSONResponse"];

    public IReadOnlyCollection<string> Kinds { get; } =
    [
        "type.kind", "endpoint.exists", "operation.verb", "operation.route", "service.route", "parameter.source", "parameter.name",
        "operation.status", "operation.openApi.ignore", "security", "operation.returns"
    ];

    public IReadOnlyCollection<string> KindsWithoutMember { get; } = ["endpoint.exists"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (FastApiSite.Of(context.Code) is not { } site)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "type.kind":
                return Kind(context, site);
            case "service.route":
                return ServiceRoute(context, site);
            case "endpoint.exists":
                return Endpoint(context, site);
            case "parameter.source" or "parameter.name":
                return Parameter(context, site);
        }

        if (site.EndpointOf(context.Member) is not { } endpoint)
        {
            return null;
        }

        var at = endpoint.Decorator.At;
        switch (context.Kind)
        {
            case "operation.verb":
                return endpoint.Verbs.Count > 1
                    ? ClaimReading.Compared(string.Join(", ", endpoint.Verbs), at, endpoint.Verbs.Contains(context.Expected, StringComparer.OrdinalIgnoreCase))
                    : ClaimReading.Stated(endpoint.Verb, at);
            case "operation.route":
            {
                // The model states the operation's route under its service's; the code's is under every prefix.
                var service = context.Owner(context.Element) is { } owner ? context.Model.Stereotype(owner, "Http Service Settings", "Route") : null;
                var absolute = context.Expected.StartsWith('/') || context.Expected.StartsWith("~/", StringComparison.Ordinal);
                var expected = absolute ? FastApiSite.Route(context.Expected.TrimStart('~')) : FastApiSite.Route(service, context.Expected);
                var prefix = absolute ? "" : FastApiSite.Route(service);
                var shown = prefix.Length > 0 && (endpoint.Route + "/").StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase)
                    ? endpoint.Route[prefix.Length..].Trim('/')
                    : endpoint.Route;
                var same = Same(endpoint.Route, expected);
                return ClaimReading.Compared(shown, at, same,
                    same ? null : $"`{endpoint.Function.Name}` answers `{endpoint.Verb} {endpoint.Url}` (its path `{endpoint.Path}` under its routers' prefixes), not `/{expected}`. The model states it in the operation's Http Settings Route, under its service's Http Service Settings Route.");
            }
            case "operation.status":
                return endpoint.Status is { } status
                    ? ClaimReading.Stated(status.ToString(), at)
                    : ClaimReading.Defaulted("200", at, "status_code");
            case "operation.openApi.ignore":
                return ClaimReading.Stated(endpoint.Hidden ? "true" : "false", endpoint.Named("include_in_schema") is null && endpoint.Router.Hidden ? endpoint.Router.At : at);
            case "security":
                return ClaimReading.Stated(site.Secured(context.Code, endpoint) ? "Secured" : "None", at);
            default:
                return Returns(context, endpoint);
        }
    }

    /// <summary>Routes compare without their parameters' names or constraints, case aside: <c>{id}</c> is <c>{project_id:int}</c>.</summary>
    private static bool Same(string code, string model)
    {
        static string Shape(string route) => string.Join("/", route.Split('/').Select(x => x.StartsWith('{') ? "{}" : x.ToLowerInvariant()));
        return Shape(code) == Shape(model);
    }

    /// <summary>A router module (<c>router = APIRouter(...)</c>) is FastAPI's controller: a Service's code, though it is no class.</summary>
    private static ClaimReading? Kind(ClaimContext context, FastApiSite site)
    {
        if (context.Element.SpecializationType != "Service" || context.Type is not { Kind: "module" } module || site.RouterOf(module) is not { } router)
        {
            return null;
        }

        return ClaimReading.Compared("module", router.At, true);
    }

    /// <summary>
    /// A router's routes are under a chain of prefixes, those of the routers including it and its own (<c>api</c>, then
    /// <c>projects</c>), and the model may split that path between the service's route and its operations' routes at any prefix:
    /// <c>api/projects</c> with <c>{id}</c>, <c>api</c> with <c>projects/{id}</c>, or none with <c>api/projects/{id}</c>. The service's
    /// route is one of those cuts; <c>operation.route</c> checks the whole path.
    /// </summary>
    private static ClaimReading? ServiceRoute(ClaimContext context, FastApiSite site)
    {
        if (site.RouterOf(context.Type) is not { } router)
        {
            return null;
        }

        var prefixes = router.OuterPrefixes.Append(router.Prefix).Select(x => FastApiSite.Route(x)).Where(x => x.Length > 0).ToList();
        var cuts = Enumerable.Range(0, prefixes.Count + 1).Select(x => FastApiSite.Route([.. prefixes.Take(x)])).ToList();
        var expected = FastApiSite.Route(context.Expected);
        var cut = cuts.FirstOrDefault(x => Same(x, expected));

        // A router with no prefix of its own states no route: the routers including it state theirs.
        if (router.Prefix is null)
        {
            return ClaimReading.Defaulted(cut ?? router.FullPrefix, router.IncludedAt.LastOrDefault() ?? router.At, "prefix");
        }

        return ClaimReading.Compared(router.FullPrefix, router.At, cut is not null,
            cut is not null ? null : $"`{context.Type!.Name}` routes under `/{router.FullPrefix}` (its routers' prefixes: {string.Join(", ", prefixes.Select(x => $"`/{x}`"))}), so its route is `/{router.FullPrefix}` or a leading part of it at a prefix, not `/{expected}`. The model states it in the service's Http Service Settings Route.");
    }

    /// <summary>
    /// An Operation's path operation (<c>endpoint.exists</c>): its router module's function of its name, decorated with a verb.
    /// A function without one isn't an endpoint, as an Operation without Http Settings states.
    /// </summary>
    private static ClaimReading? Endpoint(ClaimContext context, FastApiSite site)
    {
        var function = context.Member;
        if (function is null && context.Owner(context.Element) is { } service && context.TypeOf(service) is { } module && site.RouterOf(module) is not null)
        {
            function = module.Members.FirstOrDefault(x => x.Kind == "method" && x.Name == context.Element.Name);
            if (function is null)
            {
                return ClaimReading.Missing(module.Files.FirstOrDefault() ?? module.Name, $"a function `{context.Element.Name}` with a path operation decorator");
            }
        }

        if (function is null || !FastApiSite.IsPython(function))
        {
            return null;
        }

        var modelled = context.Expected != "absent";
        if (site.EndpointOf(function) is { } endpoint)
        {
            return modelled
                ? ClaimReading.Compared("present", endpoint.Decorator.At, true)
                : ClaimReading.Compared("present", endpoint.Decorator.At, false,
                    $"`{function.Name}` is a path operation (`@{endpoint.Router.Field.Name}.{endpoint.Decorator.Name}`), and the Operation has no Http Settings, so the model says it isn't one. Give it Http Settings, or remove its decorator.");
        }

        // A function of a router module with no path operation decorator isn't an endpoint.
        return site.RouterOf(context.Code.Types.FirstOrDefault(x => x.Members.Contains(function))) is { } router
            ? ClaimReading.Compared("absent", function.At, !modelled,
                $"`{function.Name}` isn't in the code as an endpoint: no `@{router.Field.Name}.get`, `.post` or other path operation decorator maps it. The model states it in the operation's Http Settings. Map it, or remove its Http Settings.")
            : null;
    }

    /// <summary>
    /// Where a parameter binds, as FastAPI decides: a name in the route's path is a path parameter; a <c>Form</c>/<c>File</c>,
    /// <c>Body</c>, <c>Header</c>, <c>Cookie</c> or <c>Query</c> marker says it; a Pydantic model is the body; anything else is the
    /// query. The name it binds by is its marker's <c>alias</c>, else its own (a header's with <c>_</c> as <c>-</c>).
    /// </summary>
    private static ClaimReading? Parameter(ClaimContext context, FastApiSite site)
    {
        var operation = context.Owner(context.Element);
        var function = context.Member is { Kind: "method" } member ? member : operation is null ? null : context.MemberOf(operation);
        if (site.EndpointOf(function) is not { } endpoint || function!.Parameters.FirstOrDefault(x => x.Name == context.Element.Name) is not { } parameter)
        {
            return null;
        }

        var marker = parameter.Attributes.FirstOrDefault(x => x.Name is "Query" or "Form" or "File" or "Body" or "Header" or "Path" or "Cookie");
        var source = endpoint.PathParameters.Contains(parameter.Name) ? "From Route"
            : marker?.Name switch
            {
                "Form" or "File" => "From Form",
                "Body" => "From Body",
                "Header" => "From Header",
                "Cookie" => "From Cookie",
                "Query" => "From Query",
                "Path" => "From Route",
                _ => parameter.Type is { } type && IsModel(context.Code, type) ? "From Body" : "From Query"
            };

        if (context.Kind == "parameter.source")
        {
            return ClaimReading.Stated(source, marker?.At ?? parameter.At);
        }

        if (marker?.Named.TryGetValue("alias", out var alias) == true)
        {
            return ClaimReading.Stated(alias, marker.At);
        }

        return ClaimReading.Defaulted(source == "From Header" && marker?.Named.GetValueOrDefault("convert_underscores") != "false" ? parameter.Name.Replace('_', '-') : parameter.Name,
            parameter.At, "alias");
    }

    /// <summary>A Pydantic model, which FastAPI reads from the request's body: a class deriving from <c>BaseModel</c>.</summary>
    private static bool IsModel(CodeGraph graph, TypeRefNode type)
    {
        var names = new[] { type }.Concat(type.Union).Concat(type.Arguments).Select(x => x.Name);
        return graph.TypesNamed(names).Any(x => graph.Supertypes(x).Contains("BaseModel"));
    }

    /// <summary>
    /// What a path operation returns: its <c>response_model</c> when the decorator gives one, which FastAPI sends whatever the
    /// function's annotation; nothing for a 204 annotated with a response class. Otherwise its annotation, the engine's rule.
    /// </summary>
    private static ClaimReading? Returns(ClaimContext context, FastApiEndpoint endpoint)
    {
        var function = endpoint.Function;
        if (endpoint.Value("response_model") is { } given && given.Text != "None" && PythonParser.Expression(given.Text) is { } written)
        {
            var model = PythonSyntax.TypeOf(written);
            var same = context.ExpectedValue is ITypeReference expected && context.TypeMatches(expected, model);
            return ClaimReading.Compared(model.Text, endpoint.Decorator.At, same,
                same ? null : $"`{function.Name}` returns `{model.Text}` (its `response_model`), not `{context.Expected}`. The model states it in the operation's return type.");
        }

        if (endpoint.Status == 204 && (function.Type is null || Responses.Contains(function.Type.Name)))
        {
            var same = context.Expected == "void";
            return ClaimReading.Compared("void", function.At, same,
                same ? null : $"`{function.Name}` answers 204 No Content, so it returns nothing, not `{context.Expected}`. The model states it in the operation's return type.");
        }

        // With no annotation, what the operation sends is its response_class: an HTML page's HTMLResponse.
        if (function.Type is null && endpoint.Value("response_class") is { } responseClass && PythonParser.Expression(responseClass.Text) is { } classWritten)
        {
            var sent = PythonSyntax.TypeOf(classWritten);
            var same = context.ExpectedValue is ITypeReference expected && context.TypeMatches(expected, sent);
            return ClaimReading.Compared(sent.Text, endpoint.Decorator.At, same,
                same ? null : $"`{function.Name}` sends a `{sent.Text}` (its `response_class`), not `{context.Expected}`. The model states it in the operation's return type.");
        }

        return null;
    }
}
