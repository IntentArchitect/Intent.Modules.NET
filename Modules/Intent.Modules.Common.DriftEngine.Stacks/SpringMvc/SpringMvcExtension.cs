using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.SpringMvc;

/// <summary>
/// How Spring MVC code states the HTTP claims (<see cref="Http.HttpExtension"/>): a controller's mapping, binding and
/// security annotations. A Service with HTTP settings is its controller, <c>{Name|trimEnd:Service}Controller</c>, so its
/// Operations are the controller's handler methods; and a handler's <c>ResponseEntity&lt;T&gt;</c> returns <c>T</c>.
/// </summary>
public sealed class SpringMvcExtension : IDriftExtension
{
    public string Id => "intent.springmvc";

    public IEnumerable<ICodeReader> Readers { get; } = [new SpringMvcDetector()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new SpringMvcClaims()];
}

/// <summary>Spring MVC is in the code: its controllers, the classes annotated <c>@RestController</c> or <c>@Controller</c>.</summary>
internal sealed class SpringMvcCode(IReadOnlyList<TypeNode> controllers)
{
    public IReadOnlyList<TypeNode> Controllers { get; } = controllers;
}

internal sealed class SpringMvcDetector : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        // A controller annotation in a file that imports Spring: another library's @Controller isn't Spring's.
        var files = graph.Files.Where(x => x.Imports.Any(i => i.Target.StartsWith("org.springframework.", StringComparison.Ordinal))).Select(x => x.Path).ToHashSet(StringComparer.Ordinal);
        var controllers = graph.Types.Where(x => x.Attributes.Any(a => a.Name is "RestController" or "Controller") && x.Files.Any(files.Contains)).ToList();
        if (controllers.Count > 0)
        {
            graph.Set(new SpringMvcCode(controllers));
        }
    }
}

internal sealed class SpringMvcClaims : IClaimReader
{
    private static readonly Dictionary<string, string> Verbs = new()
    {
        ["GetMapping"] = "GET", ["PostMapping"] = "POST", ["PutMapping"] = "PUT", ["DeleteMapping"] = "DELETE", ["PatchMapping"] = "PATCH"
    };

    private static readonly Dictionary<string, string> BindingSources = new()
    {
        ["PathVariable"] = "From Route", ["RequestParam"] = "From Query", ["RequestBody"] = "From Body", ["RequestHeader"] = "From Header"
    };

    /// <summary>What Spring injects into a handler, rather than binding from the request.</summary>
    private static readonly HashSet<string> Injected =
        ["HttpServletRequest", "HttpServletResponse", "HttpSession", "Principal", "Authentication", "BindingResult", "Errors", "Model", "Locale", "WebRequest"];

    /// <summary><c>hasRole('Admin')</c> or <c>hasAnyRole('Admin', 'Support')</c>.</summary>
    private static readonly Regex HasRole = new(@"^has(?:Any)?Role\s*\((?<roles>[^)]*)\)$", RegexOptions.CultureInvariant);

    private static readonly Regex Quoted = new(@"['""](?<value>[^'""]*)['""]", RegexOptions.CultureInvariant);

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "type.exists", "operation.returns", "operation.parameters", "service.route", "operation.verb", "operation.route", "parameter.source", "service.secured", "service.roles",
        "operation.secured", "operation.roles", "operation.anonymous"
    };

    public ClaimReading? Read(ClaimContext context)
    {
        // No Spring MVC in the code: the claim is another framework's to read, or can't be verified.
        if (context.Code.Get<SpringMvcCode>() is not { } spring)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "type.exists":
                return Controller(context, spring);
            case "operation.returns":
                return Returns(context);
            case "service.route":
            {
                var type = context.Type!;
                var mapping = type.Attributes.FirstOrDefault(x => x.Name == "RequestMapping");
                return Value(mapping is null ? null : Path(mapping), mapping?.At ?? type.At);
            }
            case "operation.verb":
            {
                var member = context.Member!;
                if (member.Attributes.FirstOrDefault(x => Verbs.ContainsKey(x.Name)) is { } verb)
                {
                    return ClaimReading.Stated(Verbs[verb.Name], verb.At);
                }

                var request = member.Attributes.FirstOrDefault(x => x.Name == "RequestMapping");
                var methods = request?.Values.FirstOrDefault(x => x.Name == "method") is { } method
                    ? (method.Items.Count > 0 ? method.Items : [method]).Select(x => x.Text.Split('.')[^1]).ToList()
                    : [];
                return methods.Count switch
                {
                    0 => ClaimReading.Absent(request?.At ?? member.At),
                    1 => ClaimReading.Stated(methods[0], request!.At),
                    _ => ClaimReading.CantVerify("unreadable", $"`{member.Name}` is mapped to {string.Join(", ", methods)}, not one verb.", request!.At)
                };
            }
            case "operation.route":
            {
                // The handler's own path; the controller's @RequestMapping is service.route.
                var member = context.Member!;
                var mapping = member.Attributes.FirstOrDefault(x => Verbs.ContainsKey(x.Name) || x.Name == "RequestMapping");
                return Value(mapping is null ? null : Path(mapping), mapping?.At ?? member.At);
            }
            case "parameter.source":
            {
                // Only a binding annotation states a source; Spring's inference for an unannotated parameter isn't read. A
                // request body is one per handler, whatever its Java name.
                var member = context.Member!;
                var parameter = member.Parameters.FirstOrDefault(x => string.Equals(x.Name, context.Element.Name, StringComparison.OrdinalIgnoreCase))
                                ?? (context.Expected == "From Body" ? member.Parameters.FirstOrDefault(x => Binding(x) == "RequestBody") : null);
                var binding = parameter?.Attributes.FirstOrDefault(x => BindingSources.ContainsKey(x.Name));
                return Value(binding is null ? null : BindingSources[binding.Name], binding?.At ?? parameter?.At ?? member.At);
            }
            case "operation.parameters":
                return Parameters(context, spring);
            case "service.secured" or "service.roles":
                return Secured(context.Kind, context.Type!.Attributes, context.Type.At);
            default:
                return Secured(context.Kind, context.Member!.Attributes, context.Member.At);
        }
    }

    private static ClaimReading Value(string? value, Span at) => value is null ? ClaimReading.Absent(at) : ClaimReading.Stated(value, at);

    /// <summary>
    /// A Service with HTTP settings is its controller: the <c>@RestController</c> or <c>@Controller</c> named for it, with
    /// <c>Service</c> replaced by <c>Controller</c>, or named as the Service is. Any other element, or a Service with no
    /// HTTP settings, is left to the engine's rule.
    /// </summary>
    private static ClaimReading? Controller(ClaimContext context, SpringMvcCode spring)
    {
        var element = context.Element;
        if (element.SpecializationType != "Service" || context.Claim.Designer != "Services"
            || !Has(element, "Http Service Settings") && !element.ChildElements.Any(x => Has(x, "Http Settings")))
        {
            return null;
        }

        var name = (element.Name.EndsWith("Service", StringComparison.Ordinal) ? element.Name[..^"Service".Length] : element.Name) + "Controller";
        var candidates = spring.Controllers.Where(x => x.Name == name).ToList();
        if (candidates.Count == 0)
        {
            candidates = spring.Controllers.Where(x => x.Name == element.Name).ToList();
        }

        return candidates.Count switch
        {
            1 => ClaimReading.Found(candidates[0]),
            0 => ClaimReading.Missing($"{context.Code.Files.Count} in-scope files", $"a `@RestController` class named `{name}`"),
            _ => ClaimReading.CantVerify("ambiguous", $"{candidates.Count} controllers are named `{name}`.", candidates[0].At, candidates.Select(x => x.At).ToList())
        };
    }

    /// <summary>
    /// A handler returning <c>ResponseEntity&lt;T&gt;</c> or <c>HttpEntity&lt;T&gt;</c> returns <c>T</c>, and
    /// <c>ResponseEntity&lt;Void&gt;</c> nothing. One returning <c>ResponseEntity&lt;Object&gt;</c> or <c>&lt;?&gt;</c> returns
    /// what it builds its successful responses with (<see cref="Built"/>). Any other return type is left to the engine's rule.
    /// </summary>
    private static ClaimReading? Returns(ClaimContext context)
    {
        if (context.Member is not { Type: { Name: "ResponseEntity" or "HttpEntity" } returns } member)
        {
            return null;
        }

        var body = returns.Arguments.FirstOrDefault();
        if (body is null or { Name: "Object" or "?" })
        {
            return Built(context, member, returns);
        }

        var same = context.ExpectedValue is ITypeReference model ? body.Name != "Void" && context.TypeMatches(model, body) : body.Name == "Void";
        return ClaimReading.Compared(returns.Text, member.At, same);
    }

    /// <summary>The <c>ResponseEntity</c> factories that build a successful response; the others build an error, which isn't modelled.</summary>
    private static readonly HashSet<string> Successes = ["ok", "of", "created", "accepted", "noContent"];

    /// <summary>The <c>HttpStatus</c> constants of a successful response, for <c>ResponseEntity.status(..)</c>.</summary>
    private static readonly HashSet<string> SuccessStatuses = ["OK", "CREATED", "ACCEPTED", "NO_CONTENT"];

    /// <summary>
    /// What a handler builds its successful responses with, each a chain from a <c>ResponseEntity</c> factory: the body
    /// <c>ok(x)</c> and <c>.body(x)</c> are given, <c>of(x)</c>'s <c>Optional</c> holds, or none for <c>.build()</c>. A response
    /// built by another method, such as a house helper, is a house extension's to read.
    /// </summary>
    private static ClaimReading Built(ClaimContext context, MemberNode member, TypeRefNode returns)
    {
        var chained = member.Calls.Select(x => x.On).OfType<CallNode>().ToHashSet();
        var bodies = new List<(CallNode Call, ValueNode? Body)>();
        foreach (var call in member.Calls.Where(x => !chained.Contains(x)))
        {
            var factory = call;
            while (factory.On is { } on)
            {
                factory = on;
            }

            if (factory.Receiver?.Split('.')[^1] != "ResponseEntity" || !IsSuccess(factory))
            {
                continue;
            }

            if (factory is { Name: "ok" or "of", Arguments: [var given] } && call == factory)
            {
                bodies.Add((call, given));
            }
            else if (call is { Name: "body", Arguments: [var body] })
            {
                bodies.Add((call, body.Text == "null" ? null : body));
            }
            else if (call.Name == "build")
            {
                bodies.Add((call, null));
            }
            else
            {
                return ClaimReading.CantVerify("unreadable", $"`{call.Text}` builds a response this extension doesn't read.", call.At);
            }
        }

        if (bodies.Count == 0)
        {
            return ClaimReading.CantVerify("unreadable", $"`{member.Name}` returns `{returns.Text}`, which doesn't state the type of its body.", member.At);
        }

        var types = new List<TypeRefNode?>();
        foreach (var (call, body) in bodies)
        {
            if (body is null)
            {
                types.Add(null);
                continue;
            }

            var type = body.IsKnown && body.Types is [var only] ? only : null;
            if (call is { Name: "of" } && type is { Name: "Optional", Arguments: [var held] })
            {
                type = held;
            }
            else if (call is { Name: "of" })
            {
                type = null;
            }

            if (type is null)
            {
                return ClaimReading.CantVerify("unresolved", $"The code doesn't say what `{body.Text}` is, so what `{member.Name}` returns isn't known.", call.At);
            }

            types.Add(type);
        }

        var same = types.All(x => context.ExpectedValue is ITypeReference model ? x is not null && context.TypeMatches(model, x) : x is null);
        var actual = string.Join(" or ", types.Select(x => x?.Text ?? "no body").Distinct());
        return ClaimReading.Compared($"{returns.Text} with {actual}", bodies[0].Call.At, same);
    }

    /// <summary>A factory of a successful response: <c>ok</c>, <c>created</c>, ..., or <c>status(..)</c> of a 2xx status.</summary>
    private static bool IsSuccess(CallNode factory) =>
        Successes.Contains(factory.Name)
        || factory is { Name: "status", Arguments: [var status] }
        && (SuccessStatuses.Contains(status.Text.Split('.')[^1]) || int.TryParse(status.Text, out var code) && code is >= 200 and < 300);

    /// <summary>
    /// A controller handler's parameters as HTTP binds them, which is what an Operation's parameters state: the request body
    /// by its type, whatever its Java name, and the rest by the name they bind (<c>@RequestHeader("x-requestid")</c>, else
    /// the parameter's), in any order. What Spring injects (the servlet request, the principal) isn't part of the contract.
    /// A method of any other type is left to the engine's rule.
    /// </summary>
    private static ClaimReading? Parameters(ClaimContext context, SpringMvcCode spring)
    {
        if (context.Member is not { } member || context.Owner(context.Element) is not { } owner || context.TypeOf(owner) is not { } type
            || !spring.Controllers.Contains(type))
        {
            return null;
        }

        var code = member.Parameters.Where(x => Binding(x) is not null || x.Attributes.Count == 0 && !Injected.Contains(x.Type?.Name ?? "")).ToList();
        var matched = new HashSet<ParameterNode>();
        var same = true;
        foreach (var parameter in context.Element.ChildElements.Where(x => x.SpecializationType == "Parameter"))
        {
            var body = context.ClaimOn(parameter, "parameter.source")?.Expected is { } source
                ? source == "From Body"
                : parameter.TypeReference is { IsCollection: false, Element: IElement { SpecializationType: "DTO" or "Command" } };
            var bound = body
                ? code.FirstOrDefault(x => Binding(x) == "RequestBody")
                : code.FirstOrDefault(x => Binding(x) != "RequestBody" && string.Equals(BoundName(x), parameter.Name, StringComparison.OrdinalIgnoreCase));
            same &= bound is { Type: { } boundType } && matched.Add(bound) && parameter.TypeReference is { Element: not null } model && context.TypeMatches(model, boundType);
        }

        same &= matched.Count == code.Count;
        return ClaimReading.Compared(string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, same);
    }

    /// <summary>The binding annotation's name, less the package: <c>PathVariable</c>, <c>RequestBody</c>; null when none.</summary>
    private static string? Binding(ParameterNode parameter) => parameter.Attributes.FirstOrDefault(x => BindingSources.ContainsKey(x.Name))?.Name;

    /// <summary>The name a parameter binds: its annotation's <c>name</c>, <c>value</c> or first argument, else its own.</summary>
    private static string BoundName(ParameterNode parameter)
    {
        var binding = parameter.Attributes.FirstOrDefault(x => BindingSources.ContainsKey(x.Name));
        return binding?.Named.GetValueOrDefault("name") ?? binding?.Named.GetValueOrDefault("value") ?? binding?.Arguments.FirstOrDefault() ?? parameter.Name;
    }

    /// <summary>
    /// <c>@PreAuthorize</c>, <c>@Secured</c>, <c>@RolesAllowed</c> and <c>@PermitAll</c>. <c>permitAll()</c> is anonymous,
    /// <c>isAuthenticated()</c> any signed-in caller, <c>hasRole</c> and <c>hasAnyRole</c> their roles; Spring's
    /// <c>ROLE_</c> prefix isn't part of a role's name. Another expression is a house extension's to read.
    /// </summary>
    private static ClaimReading Secured(string kind, IReadOnlyList<AttributeNode> attributes, Span at)
    {
        var preAuthorize = attributes.FirstOrDefault(x => x.Name == "PreAuthorize" && x.Arguments.Count > 0);
        var expression = preAuthorize?.Arguments[0].Trim();
        var permitAll = attributes.FirstOrDefault(x => x.Name == "PermitAll") ?? (expression == "permitAll()" ? preAuthorize : null);
        if (kind == "operation.anonymous")
        {
            return permitAll is null ? ClaimReading.Absent(at, "false") : ClaimReading.Stated("true", permitAll.At);
        }

        var secured = (expression is not null && expression != "permitAll()" ? preAuthorize : null)
                      ?? attributes.FirstOrDefault(x => x.Name is "Secured" or "RolesAllowed");
        if (kind.EndsWith(".secured", StringComparison.Ordinal))
        {
            return secured is null ? ClaimReading.Absent(at, "false") : ClaimReading.Stated("true", secured.At);
        }

        if (secured is null || expression == "isAuthenticated()")
        {
            return ClaimReading.Absent(secured?.At ?? at);
        }

        IEnumerable<string> roles;
        if (secured.Name == "PreAuthorize")
        {
            if (HasRole.Match(expression!) is not { Success: true } match)
            {
                return ClaimReading.CantVerify("unreadable", $"`@PreAuthorize(\"{expression}\")` isn't a role check this extension reads.", secured.At);
            }

            roles = Quoted.Matches(match.Groups["roles"].Value).Select(x => x.Groups["value"].Value);
        }
        else
        {
            roles = secured.Values.SelectMany(x => x.Items.Count > 0 ? x.Items : [x]).Select(x => x.Literal ?? x.Text);
        }

        return ClaimReading.Stated(string.Join(", ", roles.Select(x => x.StartsWith("ROLE_", StringComparison.Ordinal) ? x["ROLE_".Length..] : x)), secured.At);
    }

    /// <summary>
    /// A mapping's path: its first positional argument, or its <c>value</c> or <c>path</c>; the first of a list. Read without
    /// its leading <c>/</c>, as a route template is written: <c>/api/orders</c> is <c>api/orders</c>.
    /// </summary>
    private static string? Path(AttributeNode mapping)
    {
        var value = mapping.Values.FirstOrDefault(x => x.Name is null or "value" or "path");
        var path = value is null ? null : value.Items.Count > 0 ? value.Items[0].Literal ?? value.Items[0].Text : value.Literal ?? value.Text;
        return path?.TrimStart('/');
    }

    private static bool Has(IElement element, string stereotype) => element.Stereotypes.Any(x => x.Name == stereotype);
}
