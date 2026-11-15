using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Http;

/// <summary>
/// What a model says about an HTTP API, whatever framework serves it: routes, verbs, parameter sources and security, from
/// the Services designer's HTTP and security stereotypes. It reads no code; a framework's extension (ASP.NET Core,
/// Spring MVC) reads these claims from its own code.
/// </summary>
public sealed class HttpExtension : IDriftExtension
{
    // A constraint goes; an optional parameter's ? stays ({brandId:int?} is {brandId?}).
    private static readonly Regex Constraint = new(@"(?::[^}]*?)?(\??)\}", RegexOptions.CultureInvariant);

    public string Id => "intent.http";

    public IEnumerable<Pack> Packs { get; } = [StockExtensions.LoadPack("intent.http")];

    public IEnumerable<IValueComparer> Comparers { get; } =
    [
        // Segments compare case-insensitively, without leading or trailing slashes or parameter constraints ({id:int} is {id}).
        // A query string isn't part of a route: a model's api/orders?api-version={version} maps api/orders.
        new ValueComparer("route", (expected, actual) => string.Equals(Route(actual), Route(expected), StringComparison.OrdinalIgnoreCase),
            route => Constraint.Replace(route.Trim().Trim('/'), "$1}"))
    ];

    private static string Route(string route) => Constraint.Replace(WithoutQuery(route).Trim().Trim('/'), "$1}");

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
}
