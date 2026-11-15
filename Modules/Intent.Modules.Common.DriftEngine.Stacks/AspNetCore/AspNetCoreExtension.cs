using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// How ASP.NET Core code states the HTTP claims (<see cref="Http.HttpExtension"/>): a controller's route, verb, binding and
/// authorization attributes, and a Minimal API router's endpoint mappings (<see cref="MinimalApiReader"/>). It adds no
/// claims of its own, so a Spring MVC extension reads the same claims from its annotations.
/// </summary>
public sealed class AspNetCoreExtension : IDriftExtension
{
    public string Id => "intent.aspnetcore";

    public IEnumerable<ICodeReader> Readers { get; } = [new MinimalApiReader(), new AspNetCoreDetector()];

    // The Minimal API reader comes first (the later one does) and passes on anything that isn't an endpoint.
    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new AspNetCoreClaims(), new MinimalApiClaims()];

    public IEnumerable<IDriftRule> Rules { get; } = [new ConventionalRoutes()];
}

/// <summary>
/// An Operation with no Route of its own is at its service's Route, and a Web API 2 action with no route of its own isn't
/// at its controller's: Web API 2 serves it on a conventional route (<see cref="Binding.ConventionalRoute"/>). An empty Route
/// makes no <c>operation.route</c> claim (it is stated only when set), so this rule reads that case: the diagram puts
/// the endpoint at the service's Route, and the code serves it somewhere else.
/// </summary>
internal sealed class ConventionalRoutes : IDriftRule
{
    private const string Designer = "Services";

    public IEnumerable<Finding> Check(RuleContext context)
    {
        if (context.Code.Get<AspNetCoreCode>() is null || !context.Model.Designers.Contains(Designer))
        {
            yield break;
        }

        foreach (var operation in context.Model.Elements(Designer, "Operation"))
        {
            if (!context.Model.HasStereotype(operation, "Http Settings")
                || !string.IsNullOrWhiteSpace(context.Model.Stereotype(operation, "Http Settings", "Route"))
                || context.Model.Owner(operation) is not { } service
                || context.Model.Stereotype(service, "Http Service Settings", "Route") is not { Length: > 0 } serviceRoute
                || context.TypeOf(Designer, service) is not { } owner
                || context.MemberOf(Designer, operation) is not { } member
                || !context.Scope.Checks(member.At.File)
                || !Binding.WebApi2(context.Code, owner)
                || Binding.ConventionalRoute(context.Code, owner, member) is not { } conventional
                || Binding.Normal(conventional[2..]) == Binding.Normal(serviceRoute))
            {
                continue;
            }

            yield return new Finding("aspnetcore/conventional.route", Designer, "ModelCode", "error",
                $"`{owner.Name}.{member.Name}` ({member.At.File}:{member.At.Line}) has no route of its own, so Web API 2 doesn't serve it under its controller's route but on the conventional route `{conventional[2..]}`. The diagram puts Operation `{operation.Name}` at its service's Route, `{serviceRoute}`. Give the action a `[Route]`, or set the Operation's Route to `{conventional}`.",
                new Anchor(File: member.At.File, Line: member.At.Line),
                Against: [new Anchor(Element: operation.Id)]);
        }
    }
}

/// <summary>
/// ASP.NET Core is in the code: a controller, an attribute-routed action, or a Minimal API endpoint. ASP.NET Web API 2
/// (<c>System.Web.Http</c>) states the same claims with the same attributes, and its <c>[RoutePrefix]</c>, so it is read
/// alike. A Razor component's <c>[Route]</c> (its <c>@page</c>) is a page's route, not an endpoint's.
/// </summary>
internal sealed class AspNetCoreCode;

internal sealed class AspNetCoreDetector : ICodeReader
{
    // [Authorize] and [AllowAnonymous] are ASP.NET Core's on any endpoint, a gRPC service's methods included.
    private static readonly HashSet<string> Attributes =
        ["ApiController", "Route", "RoutePrefix", "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "AcceptVerbs", "Authorize", "AllowAnonymous"];

    public void Read(CodeGraph graph)
    {
        if (graph.Get<MinimalApiModel>() is { Endpoints.Count: > 0 }
            || graph.Types.Any(x => x.Kind != "component" && x.Attributes.Concat(x.Members.SelectMany(m => m.Attributes)).Any(a => Attributes.Contains(a.Name))
                                    || graph.Supertypes(x).Any(s => s is "ControllerBase" or "Controller" or "ApiController")))
        {
            graph.Set(new AspNetCoreCode());
        }
    }
}

internal sealed class AspNetCoreClaims : IClaimReader
{
    private static readonly HashSet<string> HttpVerbs = ["HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "HttpHead", "HttpOptions"];

    /// <summary>The name prefixes Web API 2 takes an unattributed action's verb from.</summary>
    private static readonly string[] ConventionVerbs = ["Get", "Post", "Put", "Delete", "Patch", "Head", "Options"];

    private static readonly Dictionary<string, string> BindingSources = new()
    {
        ["FromBody"] = "From Body", ["FromRoute"] = "From Route", ["FromQuery"] = "From Query", ["FromHeader"] = "From Header", ["FromForm"] = "From Form",
        // Injected from the container, not bound from the request: a modelled request Parameter there is wrong.
        ["FromServices"] = "From Services"
    };

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "service.route", "operation.verb", "operation.route", "parameter.source", "parameter.name", "service.secured", "service.roles", "operation.secured",
        "operation.roles", "operation.anonymous", "operation.status"
    };

    /// <summary>The attributes whose <c>Name</c> is the name a parameter is bound by: Web API 2's <c>[FromUri]</c> too.</summary>
    private static readonly HashSet<string> NamedBindings = ["FromHeader", "FromQuery", "FromRoute", "FromForm", "FromUri", "ModelBinder", "BindProperty"];

    public ClaimReading? Read(ClaimContext context)
    {
        // No ASP.NET Core in the code: the claim is another framework's to read, or can't be verified.
        if (context.Code.Get<AspNetCoreCode>() is null)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "service.route":
            {
                // Web API 2's [RoutePrefix] is the controller's route, as ASP.NET Core's [Route] on the controller is.
                var type = context.Type!;
                var route = type.Attributes.FirstOrDefault(x => x.Name is "Route" or "RoutePrefix" && x.Arguments.Count > 0);
                return Value(route is null ? null : Tokens(route.Arguments[0], type.Name, null), route?.At ?? type.At);
            }
            case "operation.verb":
            {
                var member = context.Member!;
                var verb = member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name));
                var accept = member.Attributes.FirstOrDefault(x => x.Name == "AcceptVerbs" && x.Arguments.Count > 0);
                var actual = verb is not null ? verb.Name[4..].ToUpperInvariant() : accept?.Arguments[0].ToUpperInvariant();
                if (actual is null && context.Code.Types.FirstOrDefault(x => x.Members.Contains(member)) is { } owner && Binding.WebApi2(context.Code, owner))
                {
                    // Web API 2 takes an unattributed action's verb from its name (GetOrders is GET), and POST for any other name.
                    return ClaimReading.Stated(ConventionVerbs.FirstOrDefault(x => member.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase))?.ToUpperInvariant() ?? "POST", member.At);
                }

                return Value(actual, (verb ?? accept)?.At ?? member.At);
            }
            case "operation.route":
            {
                // The action's own template, from [Route] or the verb attribute; the controller's route is service.route.
                var member = context.Member!;
                var owner = context.TypeOf(context.Owner(context.Element)!)!;
                var route = member.Attributes.FirstOrDefault(x => x.Name == "Route" && x.Arguments.Count > 0)
                            ?? member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name) && x.Arguments.Count > 0);
                if (route is null && Binding.WebApi2(context.Code, owner) && Binding.ConventionalRoute(context.Code, owner, member) is { } conventional)
                {
                    // Web API 2 serves an action with no route of its own on a conventional route, not under its controller's.
                    return ClaimReading.Stated(conventional, member.At);
                }

                return Value(route is null ? null : Tokens(route.Arguments[0], owner.Name, member.Name), route?.At ?? member.At);
            }
            case "parameter.source":
            {
                // A binding attribute states the source; without one, it is where ASP.NET binds it (Binding).
                var member = context.Member!;
                if (member.Parameters.FirstOrDefault(x => x.Name == context.Element.Name) is not { } parameter)
                {
                    return ClaimReading.Absent(member.At);
                }

                var binding = parameter.Attributes.FirstOrDefault(x => BindingSources.ContainsKey(x.Name));
                return binding is not null ? ClaimReading.Stated(BindingSources[binding.Name], binding.At) : Binding.Infer(context, member, parameter);
            }
            case "parameter.name":
                return BoundName(context);
            case "service.secured" or "service.roles":
                return Secured(context.Code, context.Kind, context.Type!, context.Type!.Attributes, context.Type.At);
            case "operation.status":
                return Http.SuccessStatus.Read(context, context.Member!);
            default:
            {
                var member = context.Member!;
                var owner = context.Code.Types.FirstOrDefault(x => x.Members.Contains(member)) ?? context.TypeOf(context.Owner(context.Element)!)!;
                return Secured(context.Code, context.Kind, owner, member.Attributes, member.At);
            }
        }
    }

    private static ClaimReading Value(string? value, Span at) => value is null ? ClaimReading.Absent(at) : ClaimReading.Stated(value, at);

    /// <summary>
    /// The name a binding attribute gives (<c>[FromHeader(Name = "x-requestid")]</c>), a constant's included. Without one the
    /// code doesn't state it, although ASP.NET would bind by the parameter's own name (decision 4).
    /// </summary>
    private static ClaimReading BoundName(ClaimContext context)
    {
        var member = context.Member!;
        if (member.Parameters.FirstOrDefault(x => x.Name == context.Element.Name) is not { } parameter)
        {
            return ClaimReading.Absent(member.At);
        }

        var binding = parameter.Attributes.FirstOrDefault(x => NamedBindings.Contains(x.Name) && x.Named.ContainsKey("Name"))
                      ?? parameter.Attributes.FirstOrDefault(x => NamedBindings.Contains(x.Name));
        if (binding?.Values.FirstOrDefault(x => x.Name == "Name") is not { } name)
        {
            return ClaimReading.Absent(binding?.At ?? parameter.At);
        }

        var owner = context.Code.Types.FirstOrDefault(x => x.Members.Contains(member));
        var value = name.Literal ?? (owner is null ? null : context.Code.ConstantOf(name.Text, owner));
        return value is not null
            ? ClaimReading.Stated(value, binding.At)
            : ClaimReading.CantVerify("unreadable", $"The code can't read `{name.Text}`, the name `[{binding.Name}]` gives: it isn't a string constant of the in-scope code.", binding.At);
    }

    /// <summary>
    /// <c>[Authorize]</c> and <c>[AllowAnonymous]</c>, and the roles <c>[Authorize(Roles = "...")]</c> names. An attribute
    /// of the code's own that derives from <c>AuthorizeAttribute</c> is one, as ASP.NET Core applies it; roles given as a
    /// constant are its value (<c>Roles = AuthorizationRoles.Reader</c>). An attribute that authorizes some other way is a
    /// house extension's to read.
    /// </summary>
    private static ClaimReading Secured(CodeGraph code, string kind, TypeNode from, IReadOnlyList<AttributeNode> attributes, Span at)
    {
        if (kind == "operation.anonymous")
        {
            var anonymous = attributes.FirstOrDefault(x => x.Name == "AllowAnonymous");
            return anonymous is null ? ClaimReading.Absent(at, "false") : ClaimReading.Stated("true", anonymous.At);
        }

        var authorize = attributes.Where(x => x.Name == "Authorize" || Authorizes(code, x, from)).ToList();
        if (kind.EndsWith(".secured", StringComparison.Ordinal))
        {
            return authorize.Count > 0 ? ClaimReading.Stated("true", authorize[0].At) : ClaimReading.Absent(at, "false");
        }

        var named = authorize.FirstOrDefault(x => x.Named.ContainsKey("Roles"));
        if (named is null)
        {
            return ClaimReading.Absent(authorize.FirstOrDefault()?.At ?? at);
        }

        var roles = named.Values.FirstOrDefault(x => x.Name == "Roles");
        return ClaimReading.Stated(roles is { Literal: null } ? code.ConstantOf(roles.Text, from) ?? named.Named["Roles"] : named.Named["Roles"], named.At);
    }

    /// <summary>The attribute is a class of the code's own (<c>X</c> or <c>XAttribute</c>) that derives from <c>AuthorizeAttribute</c>.</summary>
    private static bool Authorizes(CodeGraph code, AttributeNode attribute, TypeNode from) =>
        new[] { $"{attribute.Name}Attribute", attribute.Name }
            .Select(x => code.TypeFor(new TypeRefNode { Text = x, Name = x, QualifiedName = x }, from))
            .FirstOrDefault(x => x is not null) is { } type
        && code.Supertypes(type).Any(x => x is "AuthorizeAttribute" or "Authorize");

    /// <summary>Route tokens: <c>[controller]</c> is the class name less <c>Controller</c>, <c>[action]</c> the method's.</summary>
    private static string Tokens(string template, string controller, string? action)
    {
        var name = controller.EndsWith("Controller", StringComparison.Ordinal) ? controller[..^"Controller".Length] : controller;
        var replaced = template.Replace("[controller]", name, StringComparison.OrdinalIgnoreCase);
        return action is null ? replaced : replaced.Replace("[action]", action, StringComparison.OrdinalIgnoreCase);
    }
}
