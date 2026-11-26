using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Python;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.FastApi;

/// <summary>
/// An <c>APIRouter</c> or <c>FastAPI</c> app a Python module declares at its top level, with what the routers that include it
/// add: their prefixes, whether they hide it from OpenAPI, and the <c>include_router</c> calls that order its routes.
/// </summary>
internal sealed class FastApiRouter(TypeNode module, MemberNode field, CreationNode creation)
{
    public TypeNode Module { get; } = module;

    /// <summary>The module variable the router is: <c>router</c>, <c>app</c>.</summary>
    public MemberNode Field { get; } = field;

    public CreationNode Creation { get; } = creation;

    public string? Prefix => Creation.Arguments.FirstOrDefault(x => x.Name == "prefix")?.Literal?.Value;

    public Span At => Field.At;

    /// <summary>The prefixes the routers that include this one, and their include_router calls, add, outermost first.</summary>
    public List<string> OuterPrefixes { get; } = [];

    /// <summary>The include_router calls that bring it into the app, outermost first: they order its routes among the app's.</summary>
    public List<Span> IncludedAt { get; } = [];

    public bool HiddenByInclude { get; set; }

    /// <summary>Every prefix in front of its routes' paths, outermost first.</summary>
    public string FullPrefix => FastApiSite.Route([.. OuterPrefixes, Prefix]);

    public bool Hidden => Creation.Arguments.FirstOrDefault(x => x.Name == "include_in_schema")?.Literal?.Value == "false" || HiddenByInclude;

    /// <summary>What the router's <c>dependencies=[...]</c> runs before each of its routes.</summary>
    public IReadOnlyList<ValueNode> Dependencies => Creation.Arguments.FirstOrDefault(x => x.Name == "dependencies")?.Items ?? [];
}

/// <summary>A path operation: a module function decorated <c>@router.get(...)</c>, <c>@app.post(...)</c> or <c>@router.api_route(..., methods=[...])</c>.</summary>
internal sealed class FastApiEndpoint(FastApiRouter router, MemberNode function, AttributeNode decorator, IReadOnlyList<string> verbs, string path)
{
    public FastApiRouter Router { get; } = router;

    public MemberNode Function { get; } = function;

    public AttributeNode Decorator { get; } = decorator;

    /// <summary>The verbs it answers, upper case: one for <c>@router.get</c>, those <c>methods=</c> lists for <c>api_route</c>.</summary>
    public IReadOnlyList<string> Verbs { get; } = verbs;

    /// <summary>The path its decorator gives, under its router's prefixes.</summary>
    public string Path { get; } = path;

    public string Verb => Verbs.Count > 0 ? Verbs[0] : "GET";

    /// <summary>Its route, without leading or trailing <c>/</c>: every prefix, then its path.</summary>
    public string Route => FastApiSite.Route(Router.FullPrefix, Path);

    /// <summary>Its route as a browser requests it.</summary>
    public string Url => "/" + Route;

    /// <summary>The route's name, which <c>url_for</c> takes: the decorator's <c>name=</c>, else the function's.</summary>
    public string Name => Named("name") ?? Function.Name;

    public bool Hidden => Router.Hidden || Named("include_in_schema") == "false";

    /// <summary>The status its decorator states, or null when it leaves FastAPI's default.</summary>
    public int? Status => Named("status_code") is { } status ? Http.SuccessStatus.StatusOf(status) : null;

    /// <summary>Its place among the app's routes: by the include_router calls that bring it in, then its decorator.</summary>
    public IReadOnlyList<(string File, int Line)> Order => [.. Router.IncludedAt.Select(x => (x.File, x.Line)), (Decorator.At.File, Decorator.At.Line)];

    /// <summary>Its path parameters: <c>project_id</c> of <c>/projects/{project_id}</c>, <c>file</c> of <c>{file:path}</c>.</summary>
    public IEnumerable<string> PathParameters => Route.Split('/').Where(x => x.StartsWith('{') && x.EndsWith('}')).Select(x => x.Trim('{', '}').Split(':')[0]);

    /// <summary>A decorator's keyword argument, as a literal's value or the code's text.</summary>
    public string? Named(string name) => Decorator.Named.TryGetValue(name, out var value) ? value : null;

    public AttributeValueNode? Value(string name) => Decorator.Values.FirstOrDefault(x => x.Name == name);
}

/// <summary>
/// FastAPI as the code states it, read once per run from the Python language's graph and syntax: the routers and apps, the
/// prefixes <c>include_router</c> adds and the order it includes them in, and the path operations.
/// </summary>
internal sealed class FastApiSite
{
    private static readonly string[] VerbDecorators = ["get", "post", "put", "patch", "delete", "head", "options", "trace"];

    /// <summary>The router and app classes: FastAPI's own, and Starlette's under it.</summary>
    private static readonly HashSet<string> RouterClasses = ["APIRouter", "FastAPI"];

    /// <summary>What FastAPI gives a path operation's parameter itself, never from the request's data.</summary>
    public static readonly HashSet<string> Injected = ["Request", "Response", "BackgroundTasks", "WebSocket", "HTTPConnection", "SecurityScopes"];

    /// <summary>FastAPI's security schemes: a dependency on one, at any depth, secures the operation.</summary>
    private static readonly HashSet<string> Schemes =
        ["OAuth2PasswordBearer", "OAuth2AuthorizationCodeBearer", "OAuth2", "HTTPBearer", "HTTPBasic", "HTTPDigest", "APIKeyHeader", "APIKeyQuery", "APIKeyCookie", "OpenIdConnect"];

    public List<FastApiRouter> Routers { get; } = [];

    public List<FastApiEndpoint> Endpoints { get; } = [];

    /// <summary>The run's FastAPI, or null when no in-scope module declares a router or app from <c>fastapi</c>.</summary>
    public static FastApiSite? Of(CodeGraph graph) => graph.Get<FastApiSite>();

    public FastApiRouter? RouterOf(TypeNode? module) => module is null ? null : Routers.FirstOrDefault(x => x.Module == module);

    /// <summary>The path operation a function is; a refined copy of it (its parameters without FastAPI's injected ones) is the same function.</summary>
    public FastApiEndpoint? EndpointOf(MemberNode? function) =>
        function is null ? null : Endpoints.FirstOrDefault(x => ReferenceEquals(x.Function, function) || x.Function.Name == function.Name && x.Function.At == function.At);

    /// <summary>The path operation an Operation describes: its located function, else its service's module's function of its name.</summary>
    public FastApiEndpoint? EndpointFor(ClaimContext context, IElement operation) =>
        EndpointOf(context.LocatedOf(operation, "member.exists", "Services") as MemberNode ?? context.MemberOf(operation))
        ?? Endpoints.FirstOrDefault(x => x.Function.Name == operation.Name && x.Router.Module.Name == operation.ParentElement?.Name);

    /// <summary>
    /// The path operation <c>url_for(name, **path_params)</c> resolves to, as Starlette does: the first of the app's routes whose
    /// name is <paramref name="name"/> and whose path parameters are exactly those given.
    /// </summary>
    public FastApiEndpoint? Resolve(string name, IEnumerable<string> parameters)
    {
        var given = parameters.ToHashSet(StringComparer.Ordinal);
        return Endpoints.Where(x => x.Name == name && given.SetEquals(x.PathParameters)).OrderBy(x => x.Order, OrderComparer.Instance).FirstOrDefault();
    }

    /// <summary>A route without its leading and trailing <c>/</c>, its parts joined.</summary>
    public static string Route(params string?[] parts) =>
        string.Join("/", parts.Where(x => !string.IsNullOrEmpty(x)).SelectMany(x => x!.Split('/', StringSplitOptions.RemoveEmptyEntries)));

    /// <summary>Whether a parameter is FastAPI's to give (a dependency, the request) rather than the request's data.</summary>
    public static bool IsInjected(ParameterNode parameter) =>
        parameter.Attributes.Any(x => x.Name is "Depends" or "Security") || parameter.Type is { } type && Injected.Contains(type.Name);

    /// <summary>
    /// Whether the operation is secured: a <c>Security(...)</c> dependency, or a dependency on one of FastAPI's security schemes
    /// (an <c>OAuth2PasswordBearer</c> instance, or a function depending on one, such as <c>get_current_user</c>), in its
    /// parameters, its decorator's or its router's <c>dependencies</c>.
    /// </summary>
    public bool Secured(CodeGraph graph, FastApiEndpoint endpoint)
    {
        var markers = endpoint.Function.Parameters.SelectMany(x => x.Attributes).Where(x => x.Name is "Depends" or "Security").ToList();
        if (markers.Any(x => x.Name == "Security"))
        {
            return true;
        }

        var dependencies = markers.Select(x => x.Values.FirstOrDefault(v => v.Name is null or "dependency")?.Text)
            .Concat((endpoint.Value("dependencies")?.Items ?? []).Select(x => x.Text))
            .Concat(endpoint.Router.Dependencies.Select(x => x.Text))
            .OfType<string>()
            .ToList();
        return dependencies.Any(x => x.StartsWith("Security(", StringComparison.Ordinal) || IsScheme(graph, Callee(x), 0));
    }

    /// <summary>What a dependency calls: <c>get_current_user</c> of <c>Depends(get_current_user)</c> or of <c>get_current_user</c>.</summary>
    private static string Callee(string text)
    {
        var expression = PythonParser.Expression(text);
        if (expression is PythonCall { Function: PythonName { Id: "Depends" or "Security" }, Arguments: [{ } first, ..] })
        {
            expression = first.Value;
        }

        return expression is null ? text : PythonReader.Dotted(expression).Split('.')[^1];
    }

    private static bool IsScheme(CodeGraph graph, string name, int depth)
    {
        if (depth > 3)
        {
            return false;
        }

        var members = graph.Types.Where(x => x.Kind == "module" && x.Files.Any(IsPython)).SelectMany(x => x.Members).Where(x => x.Name == name).ToList();
        if (members.Any(x => x.Kind == "field" && x.Value?.Creations.Any(c => Schemes.Contains(c.Type.Name)) == true))
        {
            return true;
        }

        return members.Where(x => x.Kind == "method").SelectMany(x => x.Parameters).SelectMany(x => x.Attributes)
            .Where(x => x.Name is "Depends" or "Security")
            .Any(x => x.Name == "Security" || x.Values.FirstOrDefault(v => v.Name is null or "dependency")?.Text is { } callee && IsScheme(graph, Callee(callee), depth + 1));
    }

    /// <summary>
    /// The templates a function renders, through the module's own functions it calls: <c>templates.TemplateResponse(request,
    /// "index.html", ...)</c>, Starlette's older <c>TemplateResponse("index.html", {...})</c>, or its <c>name=</c>.
    /// </summary>
    public static IEnumerable<(string Template, Span At)> Renders(TypeNode module, MemberNode function)
    {
        var seen = new HashSet<MemberNode>();
        var queue = new Queue<MemberNode>([function]);
        while (queue.TryDequeue(out var member))
        {
            if (!seen.Add(member))
            {
                continue;
            }

            foreach (var creation in member.Creations.Where(x => x.Type.Name.EndsWith("TemplateResponse", StringComparison.Ordinal)))
            {
                var name = creation.Arguments.FirstOrDefault(x => x.Name == "name")?.Literal?.Value
                           ?? creation.Arguments.Where(x => x.Name is null).Select(x => x.Literal?.Value).FirstOrDefault(x => x is not null && x.Contains('.'));
                if (name is not null)
                {
                    yield return (name, creation.At);
                }
            }

            foreach (var call in member.Calls)
            {
                var own = call.Target?.Member ?? (call.Receiver is null ? module.Members.FirstOrDefault(x => x.Name == call.Name && x.Kind == "method") : null);
                if (own is not null && module.Members.Contains(own))
                {
                    queue.Enqueue(own);
                }
            }
        }
    }

    public static bool IsPython(string path) => path.EndsWith(".py", StringComparison.OrdinalIgnoreCase);

    public static bool IsPython(MemberNode? member) => member is not null && IsPython(member.At.File);

    internal static FastApiSite? Read(CodeGraph graph)
    {
        if (PythonSyntax.Of(graph) is not { } syntax)
        {
            return null;
        }

        // A router or app is FastAPI's in a file that imports fastapi; another library's APIRouter isn't.
        var fastApiFiles = graph.Files.Where(x => IsPython(x.Path) && x.Imports.Any(i => i.Target == "fastapi" || i.Target.StartsWith("fastapi.", StringComparison.Ordinal)))
            .Select(x => x.Path).ToHashSet(StringComparer.Ordinal);
        var site = new FastApiSite();
        var modules = graph.Types.Where(x => x.Kind == "module" && x.Files.Any(IsPython)).ToList();
        foreach (var module in modules.Where(x => x.Files.Any(fastApiFiles.Contains)))
        {
            foreach (var field in module.Members.Where(x => x.Kind == "field"))
            {
                if (field.Value?.Creations.FirstOrDefault(x => RouterClasses.Contains(x.Type.Name)) is { } creation)
                {
                    site.Routers.Add(new FastApiRouter(module, field, creation));
                }
            }
        }

        if (site.Routers.Count == 0)
        {
            return null;
        }

        Includes(graph, site, modules);
        Operations(syntax, site);
        return site;
    }

    /// <summary>
    /// <c>parent.include_router(child.router, prefix=..., include_in_schema=...)</c>, at a module's top level or in its
    /// functions (an app factory's <c>app.include_router(api_router)</c>): each router's outer prefixes, from its parents' and
    /// the calls'.
    /// </summary>
    private static void Includes(CodeGraph graph, FastApiSite site, IReadOnlyList<TypeNode> modules)
    {
        var includes = new List<(FastApiRouter Child, FastApiRouter? Parent, string? Prefix, bool Hidden, Span At)>();
        foreach (var file in graph.Files.Where(x => IsPython(x.Path)))
        {
            var module = modules.FirstOrDefault(x => x.Files.Contains(file.Path));
            var calls = (file.TopLevel?.Calls ?? []).Concat(module?.Members.SelectMany(x => x.Calls) ?? []);
            foreach (var call in calls.Where(x => x.Name == "include_router"))
            {
                var argument = call.Arguments.FirstOrDefault(x => x.Name is null or "router");
                var child = argument?.Field is { } field ? site.Routers.FirstOrDefault(x => x.Module == field.Type && x.Field == field.Member) : null;
                if (child is null)
                {
                    continue;
                }

                var parent = module is null ? null : site.Routers.FirstOrDefault(x => x.Module == module && x.Field.Name == call.Receiver);
                includes.Add((child, parent, call.Arguments.FirstOrDefault(x => x.Name == "prefix")?.Literal?.Value,
                    call.Arguments.FirstOrDefault(x => x.Name == "include_in_schema")?.Literal?.Value == "false", call.At));
            }
        }

        foreach (var router in site.Routers)
        {
            var current = router;
            var seen = new HashSet<FastApiRouter>();
            while (seen.Add(current) && includes.FirstOrDefault(x => x.Child == current) is { Child: not null } include)
            {
                router.IncludedAt.Insert(0, include.At);
                if (include.Prefix is not null)
                {
                    router.OuterPrefixes.Insert(0, include.Prefix);
                }

                router.HiddenByInclude |= include.Hidden;
                if (include.Parent is null)
                {
                    break;
                }

                if (include.Parent.Prefix is not null)
                {
                    router.OuterPrefixes.Insert(0, include.Parent.Prefix);
                }

                router.HiddenByInclude |= include.Parent.Creation.Arguments.FirstOrDefault(x => x.Name == "include_in_schema")?.Literal?.Value == "false";
                current = include.Parent;
            }
        }
    }

    /// <summary>
    /// Each module function a router's or app's verb decorator maps: the decorator's receiver is read from the syntax, so a
    /// module's two routers, or another library's <c>@cache.get</c>, are told apart.
    /// </summary>
    private static void Operations(PythonSyntax syntax, FastApiSite site)
    {
        foreach (var router in site.Routers)
        {
            var file = router.Module.Files.FirstOrDefault(syntax.Trees.ContainsKey);
            if (file is null)
            {
                continue;
            }

            foreach (var function in syntax.Trees[file].Body.OfType<PythonFunction>())
            {
                var member = router.Module.Members.FirstOrDefault(x => x.Kind == "method" && x.Name == function.Name);
                if (member is null)
                {
                    continue;
                }

                foreach (var decorator in function.Decorators)
                {
                    if (decorator.Expression is not PythonCall { Function: PythonAttribute { Value: PythonName receiver, Name: var name } } call || receiver.Id != router.Field.Name
                        || !(VerbDecorators.Contains(name) || name == "api_route")
                        || member.Attributes.FirstOrDefault(x => x.At.Line == decorator.First.Line && x.Name == name) is not { } attribute)
                    {
                        continue;
                    }

                    var path = call.Arguments.FirstOrDefault(x => x.Name is null or "path")?.Value is PythonString { Value: { } written } ? written : "";
                    var verbs = name == "api_route"
                        ? (call.Arguments.FirstOrDefault(x => x.Name == "methods")?.Value as PythonSequence)?.Items.OfType<PythonString>().Select(x => x.Value?.ToUpperInvariant()).OfType<string>().ToList() ?? ["GET"]
                        : [name.ToUpperInvariant()];
                    site.Endpoints.Add(new FastApiEndpoint(router, member, attribute, verbs, path));
                }
            }
        }
    }

    private sealed class OrderComparer : IComparer<IReadOnlyList<(string File, int Line)>>
    {
        public static readonly OrderComparer Instance = new();

        public int Compare(IReadOnlyList<(string File, int Line)>? x, IReadOnlyList<(string File, int Line)>? y)
        {
            for (var i = 0; i < Math.Min(x!.Count, y!.Count); i++)
            {
                var file = string.CompareOrdinal(x[i].File, y[i].File);
                var order = file != 0 ? file : x[i].Line.CompareTo(y[i].Line);
                if (order != 0)
                {
                    return order;
                }
            }

            return x.Count.CompareTo(y.Count);
        }
    }
}
