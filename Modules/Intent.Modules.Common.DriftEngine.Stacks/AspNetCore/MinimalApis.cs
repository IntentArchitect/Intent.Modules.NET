using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// One Minimal API endpoint: <c>group.MapPut("/cancel", CancelOrderAsync)</c> in a router type's mapping method.
/// <paramref name="Security"/> is what is chained on the endpoint itself (<c>RequireAuthorization</c> or <c>AllowAnonymous</c>).
/// </summary>
/// <param name="Handler">The handler method's name; for a lambda, the name <c>.WithName("...")</c> gives the endpoint, if any.</param>
/// <param name="Lambda">A lambda handler as a method: its parameters, its return type where written, and its body.</param>
/// <param name="Template">The pattern: a string literal's value, otherwise the argument as written (<paramref name="TemplateWritten"/>).</param>
/// <param name="Group">Its groups' prefixes joined; one given as anything but a literal is as written, and is <paramref name="GroupWritten"/>.</param>
internal sealed record MinimalEndpoint(string Router, string? Handler, string Verb, string Template, Span At, string? Group, Span? GroupAt, string? Security, Span? SecurityAt,
    MemberNode? Lambda = null, bool TemplateWritten = false, string? GroupWritten = null)
{
    /// <summary>The pattern, a string constant's value where the code names one (<c>OrderRoutes.ById</c>); null when the code doesn't give it.</summary>
    public string? TemplateIn(CodeGraph code) => TemplateWritten ? Constant(code, Template) : Template;

    /// <summary>The groups' prefixes, as <see cref="TemplateIn"/> reads a pattern; null when one can't be read.</summary>
    public string? GroupIn(CodeGraph code) =>
        GroupWritten is null ? Group : Constant(code, GroupWritten) is { } prefix ? Group!.Replace(GroupWritten, prefix.Trim('/')) : null;

    private string? Constant(CodeGraph code, string name) => code.Types.FirstOrDefault(x => x.Name == Router) is { } router ? code.ConstantOf(name, router) : null;
}

/// <summary>What <see cref="MinimalApiReader"/> found in the in-scope code.</summary>
internal sealed class MinimalApiModel
{
    public List<MinimalEndpoint> Endpoints { get; } = [];

    /// <summary>
    /// A router's security: chained on its group (<c>app.MapGroup("x").RequireAuthorization()</c>), or on a call of its
    /// mapping method anywhere in scope (<c>orders.MapOrdersApiV1().RequireAuthorization()</c> in Program.cs).
    /// </summary>
    public Dictionary<string, (string Security, Span At)> RouterSecurity { get; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Reads ASP.NET Core Minimal API endpoints from syntax: <c>MapGet</c>, <c>MapPost</c>, <c>MapPut</c>, <c>MapDelete</c> and
/// <c>MapPatch</c> of a method group or a lambda, the <c>MapGroup</c> prefixes of the builder they're called on, and the
/// <c>RequireAuthorization</c> or <c>AllowAnonymous</c> chained on the endpoint, its group or its router's mapping call. A
/// lambda is its operation's method: named by <c>.WithName("...")</c>, or else matched by its verb and route.
/// </summary>
internal sealed class MinimalApiReader : ICodeReader
{
    private static readonly Dictionary<string, string> Verbs = new(StringComparer.Ordinal)
    {
        ["MapGet"] = "GET", ["MapPost"] = "POST", ["MapPut"] = "PUT", ["MapDelete"] = "DELETE", ["MapPatch"] = "PATCH"
    };

    public void Read(CodeGraph graph)
    {
        if (CSharpSyntax.Of(graph) is not { } csharp)
        {
            return;
        }

        var model = new MinimalApiModel();
        var mappers = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (file, tree) in csharp.Trees)
        {
            foreach (var call in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (call.Expression is not MemberAccessExpressionSyntax access || call.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>() is not { } router)
                {
                    continue;
                }

                // MapMethods("/x", ["PUT"], handler) maps the verbs it lists; several are joined, which no one verb matches.
                string? verb;
                ExpressionSyntax? template;
                ExpressionSyntax? handler;
                if (access.Name.Identifier.Text == "MapMethods"
                    && call.ArgumentList.Arguments is [{ Expression: var mapped }, { Expression: var methods }, { Expression: var mappedHandler }, ..])
                {
                    var listed = methods.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().Select(x => x.Token.ValueText.ToUpperInvariant()).Distinct().ToList();
                    (verb, template, handler) = (listed.Count == 0 ? null : string.Join(",", listed), mapped, mappedHandler);
                }
                else if (Verbs.TryGetValue(access.Name.Identifier.Text, out var named)
                         && call.ArgumentList.Arguments is [{ Expression: var plain }, { Expression: var plainHandler }, ..])
                {
                    (verb, template, handler) = (named, plain, plainHandler);
                }
                else
                {
                    continue;
                }

                if (verb is null)
                {
                    continue;
                }

                var lambda = handler as AnonymousFunctionExpressionSyntax;
                var name = lambda is null ? Handler(handler) : Named(call);
                if (lambda is null && name is null)
                {
                    continue;
                }

                var group = Group(access.Expression, file);
                var (security, securityAt) = Chained(call, file);
                var (pattern, written) = template is LiteralExpressionSyntax literal ? (literal.Token.ValueText, false) : (template.ToString(), true);
                model.Endpoints.Add(new MinimalEndpoint(router.Identifier.Text, name, verb, pattern, SpanOf(call, file),
                    group.Prefix, group.At, security, securityAt, lambda is null ? null : Method(lambda, name ?? $"{verb} {pattern}", file, graph), written, group.Written));
                if (group.Security is { } groupSecurity)
                {
                    model.RouterSecurity[router.Identifier.Text] = (groupSecurity, group.SecurityAt!);
                }

                if (call.FirstAncestorOrSelf<MethodDeclarationSyntax>() is { } mapper)
                {
                    mappers[mapper.Identifier.Text] = router.Identifier.Text;
                }
            }
        }

        foreach (var (file, tree) in csharp.Trees)
        {
            foreach (var call in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (call.Expression is MemberAccessExpressionSyntax access && mappers.TryGetValue(access.Name.Identifier.Text, out var router)
                    && Chained(call, file) is ({ } security, { } at))
                {
                    model.RouterSecurity[router] = (security, at);
                }
            }
        }

        graph.Set(model);
    }

    private static string? Handler(ExpressionSyntax handler) => handler switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.Text,
        MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
        _ => null
    };

    /// <summary>The name <c>.WithName("...")</c> chained on the endpoint gives it.</summary>
    private static string? Named(InvocationExpressionSyntax call)
    {
        for (SyntaxNode node = call; node.Parent is MemberAccessExpressionSyntax access && access.Parent is InvocationExpressionSyntax next; node = next)
        {
            if (access.Name.Identifier.Text == "WithName" && next.ArgumentList.Arguments is [{ Expression: LiteralExpressionSyntax name }])
            {
                return name.Token.ValueText;
            }
        }

        return null;
    }

    /// <summary>
    /// A lambda handler as the operation's method: its parameters as written, its return type where it is written
    /// (<c>Task&lt;Ok&lt;T&gt;&gt; (...) =&gt;</c>), and its body, the unit the C# reader read for it.
    /// </summary>
    private static MemberNode Method(AnonymousFunctionExpressionSyntax lambda, string name, string file, CodeGraph graph)
    {
        var at = SpanOf(lambda, file);
        var parameters = lambda switch
        {
            ParenthesizedLambdaExpressionSyntax x => x.ParameterList.Parameters,
            SimpleLambdaExpressionSyntax x => [x.Parameter],
            _ => new SeparatedSyntaxList<ParameterSyntax>()
        };
        return new MemberNode
        {
            Name = name,
            Kind = "method",
            Type = lambda is ParenthesizedLambdaExpressionSyntax { ReturnType: { } returns } ? CSharpReader.TypeRef(returns) : null,
            Parameters = CSharpReader.ParametersOf(parameters, file),
            At = at,
            Body = graph.Types.SelectMany(x => x.Members).SelectMany(x => x.Calls).SelectMany(x => x.Arguments)
                .Select(x => x.Function).FirstOrDefault(x => x is not null && x.At.File == at.File && x.At.Line == at.Line && x.At.Column == at.Column)
        };
    }

    /// <summary>
    /// The <c>MapGroup</c> prefixes and security of the builder an endpoint is mapped on, following a local variable to its
    /// initializer: <c>var api = app.MapGroup("api/orders").HasApiVersion(1.0);</c>.
    /// </summary>
    private static (string? Prefix, Span? At, string? Security, Span? SecurityAt, string? Written) Group(ExpressionSyntax builder, string file)
    {
        var prefixes = new List<string>();
        string? written = null;
        Span? at = null;
        string? security = null;
        Span? securityAt = null;
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = builder;
        while (true)
        {
            if (current is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } call)
            {
                switch (access.Name.Identifier.Text)
                {
                    case "MapGroup" when call.ArgumentList.Arguments is [{ Expression: LiteralExpressionSyntax prefix }, ..]:
                        prefixes.Insert(0, prefix.Token.ValueText);
                        at ??= SpanOf(call, file);
                        break;
                    // A constant, or an interpolation: as written, for the claim reader to read or report it can't.
                    case "MapGroup" when call.ArgumentList.Arguments is [{ Expression: var named }, ..]:
                        written ??= named.ToString();
                        prefixes.Insert(0, named.ToString());
                        at ??= SpanOf(call, file);
                        break;
                    case "RequireAuthorization" or "AllowAnonymous" when security is null:
                        security = access.Name.Identifier.Text;
                        securityAt = SpanOf(access.Name, file);
                        break;
                }

                current = access.Expression;
            }
            else if (current is IdentifierNameSyntax variable && seen.Add(variable.Identifier.Text)
                     && current.FirstAncestorOrSelf<MemberDeclarationSyntax>()?.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                         .FirstOrDefault(x => x.Identifier.Text == variable.Identifier.Text)?.Initializer?.Value is { } initializer)
            {
                current = initializer;
            }
            else
            {
                return (prefixes.Count == 0 ? null : string.Join("/", prefixes.Select(x => x.Trim('/'))), at, security, securityAt, written);
            }
        }
    }

    /// <summary>The <c>RequireAuthorization</c> or <c>AllowAnonymous</c> chained after a call.</summary>
    private static (string? Security, Span? At) Chained(InvocationExpressionSyntax call, string file)
    {
        for (SyntaxNode node = call; node.Parent is MemberAccessExpressionSyntax access && access.Parent is InvocationExpressionSyntax next; node = next)
        {
            if (access.Name.Identifier.Text is "RequireAuthorization" or "AllowAnonymous")
            {
                return (access.Name.Identifier.Text, SpanOf(access.Name, file));
            }
        }

        return (null, null);
    }

    private static Span SpanOf(SyntaxNode node, string file)
    {
        var lines = node.GetLocation().GetLineSpan();
        return new Span(file, lines.StartLinePosition.Line + 1, lines.StartLinePosition.Character + 1, lines.EndLinePosition.Line + 1, lines.EndLinePosition.Character + 1);
    }
}

/// <summary>
/// The HTTP claims, and an operation's return type and parameters, of a Service that is a Minimal API router: its
/// endpoints' handler methods are its operations. A Service or operation that isn't one is passed on, to the controller
/// rules or the engine's own.
/// </summary>
internal sealed class MinimalApiClaims : IClaimReader
{
    /// <summary>The results a handler's success is: <c>Ok&lt;T&gt;</c> is T, <c>Ok</c> nothing.</summary>
    private static readonly HashSet<string> Success = ["Ok", "Created", "CreatedAtRoute", "Accepted", "AcceptedAtRoute", "NoContent"];

    /// <summary>The results that send a file, and what they return: its stream, or its content.</summary>
    private static readonly Dictionary<string, TypeRefNode> Files = new(StringComparer.Ordinal)
    {
        ["FileStreamHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["PushStreamHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["PhysicalFileHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["VirtualFileHttpResult"] = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" },
        ["FileContentHttpResult"] = new() { Text = "byte[]", Name = "byte", QualifiedName = "byte", IsArray = true }
    };

    /// <summary>Parameters the framework supplies; they're never part of the modelled signature.</summary>
    private static readonly HashSet<string> Injected = ["CancellationToken", "ILogger", "HttpContext", "HttpRequest", "HttpResponse", "ClaimsPrincipal", "IServiceProvider"];

    /// <summary>Interfaces that are request data, not services.</summary>
    private static readonly HashSet<string> RequestInterfaces = ["IFormFile", "IFormFileCollection", "IEnumerable", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection"];

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "member.exists", "service.route", "service.secured", "operation.verb", "operation.route", "operation.secured", "operation.anonymous", "operation.returns",
        "operation.parameters", "operation.status"
    };

    public ClaimReading? Read(ClaimContext context)
    {
        // A handler that returns ASP.NET Core's typed results returns what its success carries, wherever it's mapped.
        if (context.Kind == "operation.returns" && context.Member is { Type: { } returned } handler
            && (returned is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : returned) is { } result
            && (result.Name == "Results" || Success.Contains(result.Name) && result.Arguments.Count == 1 || Files.ContainsKey(result.Name)))
        {
            return Returns(context, handler);
        }

        if (context.Code.Get<MinimalApiModel>() is not { Endpoints.Count: > 0 } model)
        {
            return null;
        }

        if (context.Kind == "member.exists")
        {
            return Lambda(context, model);
        }

        if (context.Kind.StartsWith("service.", StringComparison.Ordinal))
        {
            var type = context.Type!;
            var endpoints = model.Endpoints.Where(x => x.Router == type.Name).ToList();
            if (endpoints.Count == 0)
            {
                return null;
            }

            if (context.Kind == "service.route")
            {
                if (endpoints.FirstOrDefault(x => x.GroupWritten is not null && x.GroupIn(context.Code) is null) is { } unread)
                {
                    return Unreadable($"The group's prefix is `{unread.GroupWritten}`", unread.GroupAt!);
                }

                var groups = endpoints.Select(x => x.GroupIn(context.Code)).Distinct().ToList();
                return groups is [{ } group]
                    ? ClaimReading.Stated(group, endpoints[0].GroupAt!)
                    : groups is [null] ? ClaimReading.Absent(endpoints[0].At) : ClaimReading.CantVerify("ambiguous", $"its endpoints are mapped on {groups.Count} different groups.");
            }

            return model.RouterSecurity.TryGetValue(type.Name, out var security)
                ? ClaimReading.Stated(security.Security == "RequireAuthorization" ? "true" : "false", security.At)
                : ClaimReading.Absent(endpoints[0].GroupAt ?? endpoints[0].At, "false");
        }

        if (context.Member is not { } member || context.Owner(context.Element) is not { } service || context.TypeOf(service) is not { } router
            || model.Endpoints.FirstOrDefault(x => x.Router == router.Name && (x.Lambda is { } lambda ? lambda.At == member.At : x.Handler == member.Name)) is not { } endpoint)
        {
            return null;
        }

        return context.Kind switch
        {
            "operation.verb" when endpoint.Verb.Contains(',') =>
                ClaimReading.CantVerify("unreadable", $"The code can't read as one verb: `MapMethods` maps {endpoint.Template} with {endpoint.Verb.Replace(",", ", ")}.", endpoint.At),
            "operation.verb" => ClaimReading.Stated(endpoint.Verb, endpoint.At),
            "operation.route" => endpoint.TemplateIn(context.Code) is { } template
                ? ClaimReading.Stated(template, endpoint.At)
                : Unreadable($"The pattern is `{endpoint.Template}`", endpoint.At),
            "operation.secured" => endpoint.Security == "RequireAuthorization" ? ClaimReading.Stated("true", endpoint.SecurityAt!) : ClaimReading.Absent(endpoint.At, "false"),
            "operation.anonymous" => endpoint.Security == "AllowAnonymous" ? ClaimReading.Stated("true", endpoint.SecurityAt!) : ClaimReading.Absent(endpoint.At, "false"),
            "operation.returns" when endpoint.Lambda is not null && member.Type is null => LambdaReturns(context, endpoint, member),
            "operation.returns" => Returns(context, member),
            "operation.status" => Http.SuccessStatus.Read(context, member),
            _ => Parameters(context, member)
        };
    }

    /// <summary>
    /// An operation whose endpoint's handler is a lambda, not a method of the router: the lambda <c>.WithName</c> names
    /// after it, or else the only lambda mapped with the operation's verb and route.
    /// </summary>
    private static ClaimReading? Lambda(ClaimContext context, MinimalApiModel model)
    {
        if (context.Element.SpecializationType != "Operation" || context.Type is not { } router || router.Members.Any(x => x.Name == context.Element.Name))
        {
            return null;
        }

        var lambdas = model.Endpoints.Where(x => x.Router == router.Name && x.Lambda is not null).ToList();
        var verb = context.Model.Stereotype(context.Element, "Http Settings", "Verb");
        var route = context.Model.Stereotype(context.Element, "Http Settings", "Route");
        var match = lambdas.FirstOrDefault(x => x.Handler == context.Element.Name)
                    ?? (lambdas.Where(x => x.Handler is null && string.Equals(x.Verb, verb, StringComparison.OrdinalIgnoreCase) && Route(x.TemplateIn(context.Code) ?? x.Template) == Route(route ?? "")).ToList() is [var only] ? only : null);
        return match is null ? null : ClaimReading.Found(match.Lambda! with { Name = context.Element.Name });
    }

    private static ClaimReading Unreadable(string what, Span at) =>
        ClaimReading.CantVerify("unreadable", $"{what}, which the rule can't read as a route: it isn't a literal or a string constant the code declares.", at);

    /// <summary>A template as the route comparer reads it: no slashes at its ends, no constraints, case aside.</summary>
    private static string Route(string template) =>
        System.Text.RegularExpressions.Regex.Replace(template.Trim().Trim('/'), @"(?::[^}]*?)?(\??)\}", "$1}").ToLowerInvariant();

    /// <summary>
    /// The handler's success result: <c>Task</c> unwrapped, then of <c>Results&lt;...&gt;</c> the success result, whose
    /// type argument is what it returns (<c>Ok&lt;Order&gt;</c> is Order, <c>Ok</c> nothing), or the file a file result
    /// sends. The error results aren't modelled.
    /// </summary>
    private static ClaimReading Returns(ClaimContext context, MemberNode member)
    {
        var type = member.Type;
        if (type is { Name: "Task" or "ValueTask" })
        {
            type = type.Arguments.FirstOrDefault();
        }

        // A model may state the result as the code writes it (Ok<PaginatedItems<CatalogItem>>), as one drawn from the method
        // does, rather than what it carries.
        var written = type;
        if (type is { Name: "Results" })
        {
            type = type.Arguments.FirstOrDefault(x => Success.Contains(x.Name) || Files.ContainsKey(x.Name));
        }

        if (type is not null && Files.TryGetValue(type.Name, out var file))
        {
            type = file;
        }
        else if (type is not null && Success.Contains(type.Name))
        {
            type = type.Arguments.FirstOrDefault();
        }

        return ClaimReading.Compared(member.Type?.Text ?? "void", member.At, context.ExpectedValue is ITypeReference model
            ? type is not null && type.Text != "void" && context.TypeMatches(model, type) || written is not null && context.TypeMatches(model, written)
            : type is null || type.Text == "void");
    }

    /// <summary>
    /// What a lambda that doesn't write its return type returns, read from its body: nothing, when it returns no value; for
    /// each value it does, a success result's payload (<c>TypedResults.Ok(data)</c> is data's type; the error results aren't
    /// modelled), or the value's own type, as the code gives it (a creation, a local given one, an in-scope method's result)
    /// or as a query gives it (<see cref="Sequences"/>). A value that can't be read, or values that disagree, are can't
    /// verify, never a guess.
    /// </summary>
    private static ClaimReading LambdaReturns(ClaimContext context, MinimalEndpoint endpoint, MemberNode member)
    {
        if (member.Body is not { } body)
        {
            return ClaimReading.CantVerify("unresolved", $"The lambda that handles {endpoint.Verb} {endpoint.Template} doesn't write its return type, and its body isn't read.", endpoint.At);
        }

        var returned = new List<TypeRefNode?>();
        foreach (var value in body.Returns)
        {
            var outcome = Returned(body, value);
            if (!outcome.Read)
            {
                return ClaimReading.CantVerify("unresolved",
                    $"The lambda that handles {endpoint.Verb} {endpoint.Template} doesn't write its return type, and what it returns at {value.At.File}:{value.At.Line} (`{value.Text}`) isn't read.",
                    value.At);
            }

            if (!outcome.Error)
            {
                returned.Add(outcome.Type);
            }
        }

        if (returned.Select(x => x?.Text ?? "void").Distinct(StringComparer.Ordinal).Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous",
                $"The lambda that handles {endpoint.Verb} {endpoint.Template} returns {string.Join(" or ", returned.Select(x => $"`{x?.Text ?? "nothing"}`").Distinct())}.", endpoint.At);
        }

        var type = returned.FirstOrDefault();
        return ClaimReading.Compared(type?.Text ?? "void", body.Returns.FirstOrDefault()?.At ?? member.At, context.ExpectedValue is ITypeReference model
            ? type is not null && context.TypeMatches(model, type)
            : type is null);
    }

    /// <param name="Read">Whether what it returns is known.</param>
    /// <param name="Error">An error result, which the model doesn't state.</param>
    /// <param name="Type">The success body's type; null for none.</param>
    private readonly record struct Outcome(bool Read, bool Error, TypeRefNode? Type);

    private static Outcome Returned(FunctionNode body, ValueNode value)
    {
        if ((value.Call ?? Outermost(body, value.At)) is { Receiver: "Results" or "TypedResults" } result)
        {
            if (!Success.Contains(result.Name))
            {
                return new Outcome(true, true, null);
            }

            // Created(uri, value) and CreatedAtRoute(name, values, value) put the body last.
            var payload = result.Name switch
            {
                "Ok" => result.Arguments.FirstOrDefault(),
                "Created" or "Accepted" => result.Arguments.Count > 1 ? result.Arguments[1] : null,
                "CreatedAtRoute" or "AcceptedAtRoute" => result.Arguments.Count > 2 ? result.Arguments[2] : null,
                _ => null
            };
            return payload is null ? new Outcome(true, false, null) : TypeOf(body, payload, 0) is { } type ? new Outcome(true, false, type) : default;
        }

        return TypeOf(body, value, 0) is { } plain ? new Outcome(true, false, plain) : default;
    }

    /// <summary>A value's type: as the code gives it, through what a local is given, or as the query it is gives it.</summary>
    private static TypeRefNode? TypeOf(FunctionNode body, ValueNode value, int depth)
    {
        if (value.IsKnown && value.Types is [var known])
        {
            return known;
        }

        if (depth < 3 && value.Given.Count > 0)
        {
            var given = value.Given.Select(x => TypeOf(body, x, depth + 1)).ToList();
            return given.All(x => x is not null) && given.Select(x => x!.Text).Distinct(StringComparer.Ordinal).Count() == 1 ? given[0] : null;
        }

        return (value.Call ?? Outermost(body, value.At)) is { } call ? Sequences.Result(call) : null;
    }

    /// <summary>The call a value is, under an <c>await</c>: the outermost call within it, the last of a chain.</summary>
    private static CallNode? Outermost(FunctionNode body, Span span) =>
        body.Calls.Where(x => !x.IsReference && span.Contains(x.At))
            .OrderBy(x => (x.At.Line, x.At.Column ?? 0))
            .ThenByDescending(x => (x.At.EndLine ?? x.At.Line, x.At.EndColumn ?? 0))
            .FirstOrDefault();

    /// <summary>The handler's parameters in order, each its name and type, without the ones the framework injects.</summary>
    private static ClaimReading Parameters(ClaimContext context, MemberNode member)
    {
        var code = member.Parameters.Where(x => !IsInjected(x, context.Code)).ToList();
        var model = context.Element.ChildElements.Where(x => x.SpecializationType == "Parameter").ToList();
        // What the request binds; a model that also lists what is injected, as one drawn from the method does, states the
        // method's whole signature, which is as true.
        bool Same(IReadOnlyList<ParameterNode> parameters) => parameters.Count == model.Count && parameters.Zip(model).All(x =>
            x.First.Name.TrimStart('@') == x.Second.Name.TrimStart('@') && x.First.Type is { } type && x.Second.TypeReference is { Element: not null } modelType && context.TypeMatches(modelType, type));
        return ClaimReading.Compared(string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, Same(code) || Same(member.Parameters));
    }

    /// <summary>
    /// <c>[FromServices]</c>, the framework's own types, an interface (a service), a DbContext, and an <c>[AsParameters]</c>
    /// holder of services. An <c>[AsParameters]</c> type of request data (only simple properties) is a parameter.
    /// </summary>
    internal static bool IsInjected(ParameterNode parameter, CodeGraph code)
    {
        if (parameter.Attributes.Any(x => x.Name == "FromServices") || parameter.Type is not { } type)
        {
            return parameter.Type is not null;
        }

        var declared = code.Types.FirstOrDefault(x => x.Name == type.Name);
        if (parameter.Attributes.Any(x => x.Name == "AsParameters"))
        {
            return declared is null || code.MembersOf(declared).Any(x => x.Kind == "property" && x.Type is { } property && IsService(property, code));
        }

        return IsService(type, code)
               || declared is not null && code.Bases(declared).Any(x => x.Name.EndsWith("DbContext", StringComparison.Ordinal))
               || declared?.BaseTypes.Any(x => x.Name.EndsWith("DbContext", StringComparison.Ordinal)) == true;
    }

    private static bool IsService(TypeRefNode type, CodeGraph code) =>
        Injected.Contains(type.Name)
        || Registered(code).Contains(type.Name)
        || !RequestInterfaces.Contains(type.Name) && (code.Types.FirstOrDefault(x => x.Name == type.Name) is { } declared
            ? declared.Kind == "interface"
            : type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]));

    /// <summary>
    /// The types the code registers with the container, which ASP.NET resolves a parameter of from it: the type arguments of
    /// <c>AddSingleton&lt;HooksRepository&gt;()</c>, <c>AddScoped&lt;IOrders, Orders&gt;()</c> and the like, anywhere in scope.
    /// </summary>
    private static IReadOnlySet<string> Registered(CodeGraph code)
    {
        if (code.Get<RegisteredServices>() is not { } registered)
        {
            code.Set(registered = new RegisteredServices(AllCalls(code)
                .Where(x => Registrations.Contains(x.Name))
                .SelectMany(x => x.TypeArguments)
                .Select(x => x.Name)
                .ToHashSet(StringComparer.Ordinal)));
        }

        return registered.Names;
    }

    /// <summary>Every call the code makes: its types' members' and its files' top-level statements' (Program.cs).</summary>
    internal static IEnumerable<CallNode> AllCalls(CodeGraph code) =>
        code.Types.SelectMany(x => x.Members).SelectMany(x => x.Calls).Concat(code.Files.Select(x => x.TopLevel).OfType<FunctionNode>().SelectMany(x => x.Calls));

    private static readonly HashSet<string> Registrations = new(StringComparer.Ordinal)
    {
        "AddSingleton", "AddScoped", "AddTransient", "TryAddSingleton", "TryAddScoped", "TryAddTransient", "AddKeyedSingleton", "AddKeyedScoped",
        "AddKeyedTransient", "AddHttpClient", "AddDbContext", "AddDbContextPool", "AddHostedService"
    };
}

/// <summary>The type names the code registers with the container (<see cref="MinimalApiClaims.IsInjected"/>), found once per run.</summary>
internal sealed record RegisteredServices(IReadOnlySet<string> Names);
