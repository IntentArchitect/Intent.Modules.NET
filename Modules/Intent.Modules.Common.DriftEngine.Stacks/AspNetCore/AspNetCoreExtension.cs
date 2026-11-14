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
}

/// <summary>
/// ASP.NET Core is in the code: a controller, an attribute-routed action, or a Minimal API endpoint. A Razor component's
/// <c>[Route]</c> (its <c>@page</c>) is a page's route, not an endpoint's.
/// </summary>
internal sealed class AspNetCoreCode;

internal sealed class AspNetCoreDetector : ICodeReader
{
    private static readonly HashSet<string> Attributes = ["ApiController", "Route", "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "AcceptVerbs"];

    public void Read(CodeGraph graph)
    {
        if (graph.Get<MinimalApiModel>() is { Endpoints.Count: > 0 }
            || graph.Types.Any(x => x.Kind != "component" && x.Attributes.Concat(x.Members.SelectMany(m => m.Attributes)).Any(a => Attributes.Contains(a.Name))
                                    || graph.Supertypes(x).Any(s => s is "ControllerBase" or "Controller")))
        {
            graph.Set(new AspNetCoreCode());
        }
    }
}

internal sealed class AspNetCoreClaims : IClaimReader
{
    private static readonly HashSet<string> HttpVerbs = ["HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "HttpHead", "HttpOptions"];

    private static readonly Dictionary<string, string> BindingSources = new()
    {
        ["FromBody"] = "From Body", ["FromRoute"] = "From Route", ["FromQuery"] = "From Query", ["FromHeader"] = "From Header", ["FromForm"] = "From Form",
        // Injected from the container, not bound from the request: a modelled request Parameter there is wrong.
        ["FromServices"] = "From Services"
    };

    public IReadOnlyCollection<string> Kinds { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "service.route", "operation.verb", "operation.route", "parameter.source", "service.secured", "service.roles", "operation.secured", "operation.roles",
        "operation.anonymous"
    };

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
                var type = context.Type!;
                var route = type.Attributes.FirstOrDefault(x => x.Name == "Route" && x.Arguments.Count > 0);
                return Value(route is null ? null : Tokens(route.Arguments[0], type.Name, null), route?.At ?? type.At);
            }
            case "operation.verb":
            {
                var member = context.Member!;
                var verb = member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name));
                var accept = member.Attributes.FirstOrDefault(x => x.Name == "AcceptVerbs" && x.Arguments.Count > 0);
                var actual = verb is not null ? verb.Name[4..].ToUpperInvariant() : accept?.Arguments[0].ToUpperInvariant();
                return Value(actual, (verb ?? accept)?.At ?? member.At);
            }
            case "operation.route":
            {
                // The action's own template, from [Route] or the verb attribute; the controller's route is service.route.
                var member = context.Member!;
                var owner = context.TypeOf(context.Owner(context.Element)!)!;
                var route = member.Attributes.FirstOrDefault(x => x.Name == "Route" && x.Arguments.Count > 0)
                            ?? member.Attributes.FirstOrDefault(x => HttpVerbs.Contains(x.Name) && x.Arguments.Count > 0);
                return Value(route is null ? null : Tokens(route.Arguments[0], owner.Name, member.Name), route?.At ?? member.At);
            }
            case "parameter.source":
            {
                // Only an explicit binding attribute states a source; ASP.NET's inference from the route or the type isn't read.
                var member = context.Member!;
                var parameter = member.Parameters.FirstOrDefault(x => x.Name == context.Element.Name);
                var binding = parameter?.Attributes.FirstOrDefault(x => BindingSources.ContainsKey(x.Name));
                return Value(binding is null ? null : BindingSources[binding.Name], binding?.At ?? parameter?.At ?? member.At);
            }
            case "service.secured" or "service.roles":
                return Secured(context.Kind, context.Type!.Attributes, context.Type.At);
            default:
                return Secured(context.Kind, context.Member!.Attributes, context.Member.At);
        }
    }

    private static ClaimReading Value(string? value, Span at) => value is null ? ClaimReading.Absent(at) : ClaimReading.Stated(value, at);

    /// <summary>
    /// <c>[Authorize]</c> and <c>[AllowAnonymous]</c>, and the roles <c>[Authorize(Roles = "...")]</c> names. A custom
    /// authorization attribute is a house extension's to read.
    /// </summary>
    private static ClaimReading Secured(string kind, IReadOnlyList<AttributeNode> attributes, Span at)
    {
        if (kind == "operation.anonymous")
        {
            var anonymous = attributes.FirstOrDefault(x => x.Name == "AllowAnonymous");
            return anonymous is null ? ClaimReading.Absent(at, "false") : ClaimReading.Stated("true", anonymous.At);
        }

        var authorize = attributes.Where(x => x.Name == "Authorize").ToList();
        if (kind.EndsWith(".secured", StringComparison.Ordinal))
        {
            return authorize.Count > 0 ? ClaimReading.Stated("true", authorize[0].At) : ClaimReading.Absent(at, "false");
        }

        var named = authorize.FirstOrDefault(x => x.Named.ContainsKey("Roles"));
        return named is null ? ClaimReading.Absent(authorize.FirstOrDefault()?.At ?? at) : ClaimReading.Stated(named.Named["Roles"], named.At);
    }

    /// <summary>Route tokens: <c>[controller]</c> is the class name less <c>Controller</c>, <c>[action]</c> the method's.</summary>
    private static string Tokens(string template, string controller, string? action)
    {
        var name = controller.EndsWith("Controller", StringComparison.Ordinal) ? controller[..^"Controller".Length] : controller;
        var replaced = template.Replace("[controller]", name, StringComparison.OrdinalIgnoreCase);
        return action is null ? replaced : replaced.Replace("[action]", action, StringComparison.OrdinalIgnoreCase);
    }
}
