using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Http;

/// <summary>
/// What a model says about an HTTP API, whatever framework serves it: endpoints, routes, verbs, parameters, security and
/// versioning, from the Services designer's HTTP, security and versioning stereotypes. It reads no code; a framework's
/// extension (ASP.NET Core, Spring MVC) reads these claims from its own code.
/// </summary>
public sealed class HttpExtension : IDriftExtension
{
    // A constraint goes; an optional parameter's ? stays ({brandId:int?} is {brandId?}).
    private static readonly Regex Constraint = new(@"(?::[^}]*?)?(\??)\}", RegexOptions.CultureInvariant);

    public string Id => "intent.http";

    public IEnumerable<Pack> Packs { get; } = [StockExtensions.LoadPack("intent.http")];

    public IEnumerable<IClaimSource> ClaimSources { get; } = [new Endpoints()];

    public IEnumerable<IValueComparer> Comparers { get; } =
    [
        // Segments compare case-insensitively, without leading or trailing slashes or parameter constraints ({id:int} is {id}).
        // A query string isn't part of a route: a model's api/orders?api-version={version} maps api/orders. A parameter written
        // :id, as vue-router, Express and Angular write one, is {id}.
        new ValueComparer("route", (expected, actual) => string.Equals(Route(actual), Route(expected), StringComparison.OrdinalIgnoreCase),
            route => Constraint.Replace(route.Trim().Trim('/'), "$1}")),
        // A set of API versions, each compared as a version: 1, v1 and 1.0 are one item.
        new ValueComparer("versions", (expected, actual) => Versions(expected).SetEquals(Versions(actual)))
    ];

    private static string Route(string route) => Constraint.Replace(Colon.Replace(WithoutQuery(route).Trim().Trim('/'), "$1{$2$3}"), "$1}");

    private static readonly Regex Colon = new(@"(^|/):(\w+)(\?)?(?=/|$)", RegexOptions.CultureInvariant);

    /// <summary>The route before its query string: the first <c>?</c> outside a parameter, which an optional one ({id?}) has inside.</summary>
    private static string WithoutQuery(string route)
    {
        var depth = 0;
        for (var i = 0; i < route.Length; i++)
        {
            depth += route[i] switch { '{' => 1, '}' => -1, _ => 0 };
            if (route[i] == '?' && depth == 0)
            {
                return route[..i];
            }
        }

        return route;
    }

    /// <summary>A version without a leading <c>v</c> or trailing <c>.0</c> parts: <c>v1</c>, <c>1</c> and <c>1.0</c> are <c>1</c>.</summary>
    internal static string Version(string text)
    {
        var trimmed = text.Trim().TrimStart('v', 'V');
        while (trimmed.EndsWith(".0", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^2];
        }

        return trimmed;
    }

    private static HashSet<string> Versions(string text) =>
        text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Where(x => x != "none").Select(Version).ToHashSet(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Whether an Operation is an endpoint (<c>endpoint.exists</c>): one with Http Settings states it is; one without them, in a
/// service that serves HTTP (its Http Service Settings, or another of its Operations' Http Settings), states it isn't. A
/// service with no HTTP at all, such as a gRPC service or a business class, states nothing about endpoints. The claim waits
/// on the service's type and the operation's method, so a missing router or method is one finding, its own.
/// </summary>
internal sealed class Endpoints : IClaimSource
{
    private const string Designer = "Services";

    public IEnumerable<ClaimSpec> Claims(ModelContext model)
    {
        if (!model.Designers.Contains(Designer))
        {
            yield break;
        }

        foreach (var service in model.Elements(Designer, "Service"))
        {
            var operations = service.ChildElements.Where(x => x.SpecializationType == "Operation").ToList();
            var http = model.HasStereotype(service, "Http Service Settings") || operations.Any(x => model.HasStereotype(x, "Http Settings"));
            if (!http)
            {
                continue;
            }

            foreach (var operation in operations)
            {
                var endpoint = model.HasStereotype(operation, "Http Settings");
                yield return new ClaimSpec
                {
                    Designer = Designer,
                    Element = operation,
                    Kind = "endpoint.exists",
                    Expected = endpoint ? "present" : "absent",
                    DependsOn = new ClaimRef(Designer, service.Id, "type.exists"),
                    WaitsOn = [new ClaimRef(Designer, operation.Id, "member.exists")],
                    Source = endpoint
                        ? "the operation's Http Settings"
                        : "its service's HTTP settings, where an Operation without Http Settings isn't an endpoint"
                };
            }
        }
    }
}
