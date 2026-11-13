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
internal sealed record MinimalEndpoint(string Router, string Handler, string Verb, string Template, Span At, string? Group, Span? GroupAt, string? Security, Span? SecurityAt);

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
/// <c>MapPatch</c> of a method group, the <c>MapGroup</c> prefixes of the builder they're called on, and the
/// <c>RequireAuthorization</c> or <c>AllowAnonymous</c> chained on the endpoint, its group or its router's mapping call.
/// A lambda handler isn't read yet.
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
                if (call.Expression is not MemberAccessExpressionSyntax access || !Verbs.TryGetValue(access.Name.Identifier.Text, out var verb)
                    || call.ArgumentList.Arguments is not [{ Expression: LiteralExpressionSyntax template }, { Expression: var handler }, ..]
                    || Handler(handler) is not { } name
                    || call.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>() is not { } router)
                {
                    continue;
                }

                var group = Group(access.Expression, file);
                var (security, securityAt) = Chained(call, file);
                model.Endpoints.Add(new MinimalEndpoint(router.Identifier.Text, name, verb, template.Token.ValueText, SpanOf(call, file),
                    group.Prefix, group.At, security, securityAt));
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

    /// <summary>
    /// The <c>MapGroup</c> prefixes and security of the builder an endpoint is mapped on, following a local variable to its
    /// initializer: <c>var api = app.MapGroup("api/orders").HasApiVersion(1.0);</c>.
    /// </summary>
    private static (string? Prefix, Span? At, string? Security, Span? SecurityAt) Group(ExpressionSyntax builder, string file)
    {
        var prefixes = new List<string>();
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
                return (prefixes.Count == 0 ? null : string.Join("/", prefixes.Select(x => x.Trim('/'))), at, security, securityAt);
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

    /// <summary>Parameters the framework supplies; they're never part of the modelled signature.</summary>
    private static readonly HashSet<string> Injected = ["CancellationToken", "ILogger", "HttpContext", "HttpRequest", "HttpResponse", "ClaimsPrincipal", "IServiceProvider"];

    /// <summary>Interfaces that are request data, not services.</summary>
    private static readonly HashSet<string> RequestInterfaces = ["IFormFile", "IFormFileCollection", "IEnumerable", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection"];

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "service.route", "service.secured", "operation.verb", "operation.route", "operation.secured", "operation.anonymous", "operation.returns", "operation.parameters"
    };

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Code.Get<MinimalApiModel>() is not { Endpoints.Count: > 0 } model)
        {
            return null;
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
                var groups = endpoints.Select(x => x.Group).Distinct().ToList();
                return groups is [{ } group]
                    ? ClaimReading.Stated(group, endpoints[0].GroupAt!)
                    : groups is [null] ? ClaimReading.Absent(endpoints[0].At) : ClaimReading.CantVerify("ambiguous", $"its endpoints are mapped on {groups.Count} different groups.");
            }

            return model.RouterSecurity.TryGetValue(type.Name, out var security)
                ? ClaimReading.Stated(security.Security == "RequireAuthorization" ? "true" : "false", security.At)
                : ClaimReading.Absent(endpoints[0].GroupAt ?? endpoints[0].At, "false");
        }

        if (context.Member is not { } member || context.Owner(context.Element) is not { } service || context.TypeOf(service) is not { } router
            || model.Endpoints.FirstOrDefault(x => x.Router == router.Name && x.Handler == member.Name) is not { } endpoint)
        {
            return null;
        }

        return context.Kind switch
        {
            "operation.verb" => ClaimReading.Stated(endpoint.Verb, endpoint.At),
            "operation.route" => ClaimReading.Stated(endpoint.Template, endpoint.At),
            "operation.secured" => endpoint.Security == "RequireAuthorization" ? ClaimReading.Stated("true", endpoint.SecurityAt!) : ClaimReading.Absent(endpoint.At, "false"),
            "operation.anonymous" => endpoint.Security == "AllowAnonymous" ? ClaimReading.Stated("true", endpoint.SecurityAt!) : ClaimReading.Absent(endpoint.At, "false"),
            "operation.returns" => Returns(context, member),
            _ => Parameters(context, member)
        };
    }

    /// <summary>
    /// The handler's success result: <c>Task</c> unwrapped, then of <c>Results&lt;...&gt;</c> the success result, whose
    /// type argument is what it returns (<c>Ok&lt;Order&gt;</c> is Order, <c>Ok</c> nothing). The error results aren't
    /// modelled.
    /// </summary>
    private static ClaimReading Returns(ClaimContext context, MemberNode member)
    {
        var type = member.Type;
        if (type is { Name: "Task" or "ValueTask" })
        {
            type = type.Arguments.FirstOrDefault();
        }

        if (type is { Name: "Results" })
        {
            type = type.Arguments.FirstOrDefault(x => Success.Contains(x.Name));
        }

        if (type is not null && Success.Contains(type.Name))
        {
            type = type.Arguments.FirstOrDefault();
        }

        return ClaimReading.Compared(member.Type?.Text ?? "void", member.At, context.ExpectedValue is ITypeReference model
            ? type is not null && type.Text != "void" && context.TypeMatches(model, type)
            : type is null || type.Text == "void");
    }

    /// <summary>The handler's parameters in order, each its name and type, without the ones the framework injects.</summary>
    private static ClaimReading Parameters(ClaimContext context, MemberNode member)
    {
        var code = member.Parameters.Where(x => !IsInjected(x, context.Code)).ToList();
        var model = context.Element.ChildElements.Where(x => x.SpecializationType == "Parameter").ToList();
        var same = code.Count == model.Count && code.Zip(model).All(x =>
            x.First.Name == x.Second.Name && x.First.Type is { } type && x.Second.TypeReference is { Element: not null } modelType && context.TypeMatches(modelType, type));
        return ClaimReading.Compared(string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, same);
    }

    /// <summary>
    /// <c>[FromServices]</c>, the framework's own types, an interface (a service), a DbContext, and an <c>[AsParameters]</c>
    /// holder of services. An <c>[AsParameters]</c> type of request data (only simple properties) is a parameter.
    /// </summary>
    private static bool IsInjected(ParameterNode parameter, CodeGraph code)
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
        || !RequestInterfaces.Contains(type.Name) && (code.Types.FirstOrDefault(x => x.Name == type.Name) is { } declared
            ? declared.Kind == "interface"
            : type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]));
}
