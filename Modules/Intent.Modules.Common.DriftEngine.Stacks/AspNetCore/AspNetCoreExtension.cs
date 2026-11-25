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

    public IEnumerable<ICodeReader> Readers { get; } = [new MinimalApiReader(), new ApiVersioningReader(), new AspNetCoreDetector()];

    // The Minimal API reader comes first (the later one does) and passes on anything that isn't an endpoint.
    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new AspNetCoreClaims(), new MinimalApiClaims()];

    public IEnumerable<IDriftRule> Rules { get; } = [new ConventionalRoutes()];

    public IEnumerable<IOptInRule> OptInRules { get; } = [new UnmodelledEndpoints()];
}

/// <summary>
/// An Operation with no Route of its own is at its service's Route, and a Web API 2 action with no route of its own isn't
/// at its controller's: Web API 2 serves it on a conventional route (<see cref="Binding.ConventionalRoute"/>), as ASP.NET
/// Core does an action whose controller has no route either. An empty Route
/// makes no <c>operation.route</c> claim (it is stated only when set), so this rule reads that case: the diagram puts
/// the endpoint at the service's Route, and the code serves it somewhere else.
/// </summary>
internal sealed class ConventionalRoutes : IDriftRule
{
    private const string Designer = "Services";

    public string Id => "aspnetcore/conventional.route";

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
                // Only a controller is served on a conventional route: a Minimal API router maps its own (2026-11-25, a migrated
                // eShop's router read as served on another project's MapDefaultControllerRoute).
                || !AspNetCoreClaims.IsController(context.Code, owner)
                || context.MemberOf(Designer, operation) is not { } member
                || !context.Scope.Checks(member.At.File)
                || Binding.ConventionalRoute(context.Code, owner, member) is not { } conventional
                || Binding.Normal(conventional[2..]) == Binding.Normal(serviceRoute))
            {
                continue;
            }

            var framework = Binding.WebApi2(context.Code, owner) ? "Web API 2" : "ASP.NET Core";
            yield return new Finding("aspnetcore/conventional.route", Designer, "ModelCode", "error",
                $"`{owner.Name}.{member.Name}` ({member.At.File}:{member.At.Line}) has no route of its own, so {framework} doesn't serve it under its controller's route but on the conventional route `{conventional[2..]}`. The diagram puts Operation `{operation.Name}` at its service's Route, `{serviceRoute}`. Give the action a `[Route]`, or set the Operation's Route to `{conventional}`.",
                new Anchor(File: member.At.File, Line: member.At.Line),
                Against: [new Anchor(Element: operation.Id)])
            {
                Corrections = [new ModelCorrection(ModelCorrection.SetStereotype, operation.Id) { Stereotype = "Http Settings", Properties = new Dictionary<string, string> { ["Route"] = conventional } }]
            };
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
        // A router that maps a group, but no endpoint yet, is still ASP.NET Core's: an Operation it no longer maps is missing.
        if (graph.Get<MinimalApiModel>() is { Endpoints.Count: > 0 } or { Routers.Count: > 0 }
            || graph.Types.Any(x => x.Kind != "component" && x.Attributes.Concat(x.Members.SelectMany(m => m.Attributes)).Any(a => Attributes.Contains(a.Name))
                                    || graph.Supertypes(x).Any(s => s is "ControllerBase" or "Controller" or "ApiController" or "ODataController")))
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
        "type.exists", "service.route", "operation.verb", "operation.route", "parameter.source", "parameter.name", "operation.status", "endpoint.exists", "security",
        "security.roles", "security.policy", "service.versions", "operation.versions", "api.versionScheme"
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
            case "type.exists":
                return Controller(context);
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
                var owner = context.Code.Types.FirstOrDefault(x => x.Members.Contains(member));
                if (Verb(context.Code, owner, member) is { } stated)
                {
                    return ClaimReading.Stated(stated.Verb, stated.At);
                }

                if (context.Expected is { Length: > 0 } modelled)
                {
                    // An ASP.NET Core action with no verb attribute serves every verb, so it serves the one the model gives. That is the
                    // framework's convention, not a value the code states, so unmodelled.value doesn't report it.
                    return ClaimReading.Defaulted(modelled, member.At, "no verb attribute: it serves every verb");
                }

                return ClaimReading.Absent(member.At);
            }
            case "operation.route":
            {
                // The action's own template, from [Route] or the verb attribute; the controller's route is service.route.
                var member = context.Member!;
                var owner = context.TypeOf(context.Owner(context.Element)!)!;
                var route = Route(context.Code, owner, member);
                // Under a controller with no route of its own, a conventional route's absolute form is its relative one too:
                // OData's ~/odata/RequestPacks is odata/RequestPacks, as a migrated application's model states it.
                if (route.Route is ['~', '/', .. var relative] && !new[] { owner }.Concat(context.Code.Bases(owner)).Any(x => x.Attributes.Any(a => a.Name is "Route" or "RoutePrefix" && a.Arguments.Count > 0))
                    && Binding.Normal(context.Expected) == Binding.Normal(relative))
                {
                    return ClaimReading.Compared(relative, route.At, true);
                }

                return Value(route.Route, route.At);
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
            case "operation.status":
                return Http.SuccessStatus.Read(context, context.Member!);
            case "api.versionScheme":
                return Versioning.Scheme(context);
            case "endpoint.exists":
                return Endpoint(context);
            case "service.versions":
            {
                // A controller's [ApiVersion]s, its bases' and its actions' included.
                var type = context.Type!;
                if (!IsController(context.Code, type))
                {
                    return NotAController(type);
                }

                var declared = new[] { type }.Concat(context.Code.Bases(type)).SelectMany(x => x.Attributes).Concat(type.Members.SelectMany(x => x.Attributes));
                return Versioning.Read(context, Versioning.Attributes(declared, "ApiVersion"), type, type.At);
            }
            default:
            {
                // An action's security and versions: its own attributes, then its controller's.
                var member = context.Member ?? context.MemberOf(context.Element);
                var owner = (member is null ? null : context.Code.Types.FirstOrDefault(x => x.Members.Contains(member))) ?? context.TypeOf(context.Owner(context.Element)!);
                if (member is null || owner is null)
                {
                    return null;
                }

                if (!IsController(context.Code, owner))
                {
                    return NotAController(owner);
                }

                if (context.Kind == "operation.versions")
                {
                    var own = Versioning.Attributes(member.Attributes, "MapToApiVersion", "ApiVersion");
                    return Versioning.Read(context, own.Count > 0 ? own : Versioning.Attributes(new[] { owner }.Concat(context.Code.Bases(owner)).SelectMany(x => x.Attributes), "ApiVersion"),
                        owner, member.At);
                }

                return Security.Read(context, Security.Attributes(context.Code, member.Attributes, owner).Concat(Security.OfType(context.Code, owner)).ToList(), owner, member.At);
            }
        }
    }

    /// <summary>
    /// A controller: one ASP.NET Core or Web API 2 discovers, by <c>[ApiController]</c> or <c>[Controller]</c>, a controller
    /// base, or a name ending <c>Controller</c>.
    /// </summary>
    internal static bool IsController(CodeGraph code, TypeNode type) =>
        type.Kind == "class"
        && (type.Name.EndsWith("Controller", StringComparison.Ordinal)
            || new[] { type }.Concat(code.Bases(type)).Any(x => x.Attributes.Any(a => a.Name is "ApiController" or "Controller"))
            || code.Supertypes(type).Any(x => x is "ControllerBase" or "Controller" or "ApiController" or "ODataController"));

    /// <summary>
    /// A Service that serves HTTP, whose name no type of the code has, is the controller named for it as Spring MVC's is:
    /// <c>OrdersService</c> is <c>OrdersController</c>; with no such controller, the Minimal API router that maps its Route
    /// as a group (<see cref="MinimalApiClaims.Router"/>). A type of the Service's own name (or its interface's) is left to
    /// the engine's rule, so this only finds what it would have reported missing.
    /// </summary>
    private static ClaimReading? Controller(ClaimContext context)
    {
        var element = context.Element;
        if (element.SpecializationType != "Service" || context.Claim.Designer != "Services"
            || !context.Model.HasStereotype(element, "Http Service Settings") && !element.ChildElements.Any(x => context.Model.HasStereotype(x, "Http Settings"))
            || context.Code.Types.Any(x => x.Name == element.Name || x.Name == $"I{element.Name}"))
        {
            return null;
        }

        var name = (element.Name.EndsWith("Service", StringComparison.Ordinal) ? element.Name[..^"Service".Length] : element.Name) + "Controller";
        return context.Code.Types.Where(x => x.Name == name && IsController(context.Code, x)).ToList() switch
        {
            [var controller] => ClaimReading.Found(controller),
            [] => MinimalApiClaims.Router(context),
            var several => ClaimReading.CantVerify("ambiguous", $"{several.Count} controllers are named `{name}`.", several[0].At, several.Select(x => x.At).ToList())
        };
    }

    private static ClaimReading NotAController(TypeNode type) =>
        ClaimReading.CantVerify("unresolved", $"`{type.Name}` is neither a controller nor a Minimal API router, so the code doesn't say how it serves its endpoints.", type.At);

    /// <summary>
    /// An Operation's action (<c>endpoint.exists</c>): every public method of a controller is one unless <c>[NonAction]</c>,
    /// as ASP.NET Core and Web API 2 discover them. One without Http Settings states it isn't.
    /// </summary>
    private static ClaimReading? Endpoint(ClaimContext context)
    {
        if (context.Type is not { } type)
        {
            return null;
        }

        if (!IsController(context.Code, type))
        {
            return NotAController(type);
        }

        if (context.MemberOf(context.Element) is not { } member)
        {
            return ClaimReading.CantVerify("unresolved", $"`{type.Name}` has no one method that is `{context.Element.Name}`, so whether it is an action isn't known.", type.At);
        }

        var modelled = context.Expected != "absent";
        var action = member is { Kind: "method", Access: "public", IsStatic: false } && member.Attributes.All(x => x.Name != "NonAction");
        if (!action)
        {
            return ClaimReading.Compared("absent", member.At, !modelled,
                $"`{type.Name}.{member.Name}` isn't in the code as an endpoint: it isn't a public instance method of the controller{(member.Attributes.Any(x => x.Name == "NonAction") ? ", as `[NonAction]` says" : "")}. The model states it in the operation's Http Settings. Make it an action, or remove its Http Settings.");
        }

        if (modelled)
        {
            return ClaimReading.Found(member);
        }

        var reading = ClaimReading.Compared("present", member.At, false,
            $"`{type.Name}.{member.Name}` is an action of the controller, and the Operation has no Http Settings, so the model says it isn't an endpoint. Give it Http Settings, or mark the method `[NonAction]`.");

        // The Http Settings the code states, when it states the verb: an ASP.NET Core action with no verb attribute serves every
        // verb, and which one the diagram should give it is a judgement.
        if (Verb(context.Code, type, member) is not { } verb)
        {
            return reading;
        }

        return reading.Correcting(new ModelCorrection(ModelCorrection.SetStereotype, context.Element.Id)
        {
            Stereotype = "Http Settings",
            Properties = new Dictionary<string, string> { ["Verb"] = verb.Verb, ["Route"] = Route(context.Code, type, member).Route ?? "" }
        });
    }

    /// <summary>
    /// The verb an action states: its verb attribute's, its <c>[AcceptVerbs]</c>' first, or Web API 2's convention for an
    /// unattributed action, the verb its name starts with (<c>GetOrders</c> is GET), else POST. Null for an ASP.NET Core action
    /// with no verb attribute, which serves every verb.
    /// </summary>
    private static (string Verb, Span At)? Verb(CodeGraph code, TypeNode? owner, MemberNode member)
    {
        var verb = member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name));
        var accept = member.Attributes.FirstOrDefault(x => x.Name == "AcceptVerbs" && x.Arguments.Count > 0);
        if (verb is not null)
        {
            return (verb.Name[4..].ToUpperInvariant(), verb.At);
        }

        if (accept is not null)
        {
            return (accept.Arguments[0].ToUpperInvariant(), accept.At);
        }

        // OData routes an unattributed action by the same convention.
        return owner is not null && (Binding.WebApi2(code, owner) || Binding.OData(code, owner))
            ? (ConventionVerbs.FirstOrDefault(x => member.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase))?.ToUpperInvariant() ?? "POST", member.At)
            : null;
    }

    /// <summary>
    /// The route an action states, relative to its controller's: its <c>[Route]</c> or verb attribute's template; with neither,
    /// the conventional route Web API 2 serves it on, or ASP.NET Core one whose controller has no route either; else none.
    /// </summary>
    private static (string? Route, Span At) Route(CodeGraph code, TypeNode owner, MemberNode member)
    {
        var route = member.Attributes.FirstOrDefault(x => x.Name == "Route" && x.Arguments.Count > 0)
                    ?? member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name) && x.Arguments.Count > 0);
        if (route is null && Binding.ConventionalRoute(code, owner, member) is { } conventional)
        {
            return (conventional, member.At);
        }

        return (route is null ? null : Tokens(route.Arguments[0], owner.Name, member.Name), route?.At ?? member.At);
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

    /// <summary>Route tokens: <c>[controller]</c> is the class name less <c>Controller</c>, <c>[action]</c> the method's.</summary>
    private static string Tokens(string template, string controller, string? action)
    {
        var name = controller.EndsWith("Controller", StringComparison.Ordinal) ? controller[..^"Controller".Length] : controller;
        var replaced = template.Replace("[controller]", name, StringComparison.OrdinalIgnoreCase);
        return action is null ? replaced : replaced.Replace("[action]", action, StringComparison.OrdinalIgnoreCase);
    }
}
