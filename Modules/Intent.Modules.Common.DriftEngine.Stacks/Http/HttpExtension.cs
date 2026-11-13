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
    private static readonly Regex Constraint = new(@"(?::[^}]*)?\}", RegexOptions.CultureInvariant);

    public string Id => "intent.http";

    public IEnumerable<Pack> Packs { get; } = [StockExtensions.LoadPack("intent.http")];

    public IEnumerable<IValueComparer> Comparers { get; } =
    [
        // Segments compare case-insensitively, without leading or trailing slashes or parameter constraints ({id:int} is {id}).
        new ValueComparer("route", (expected, actual) => string.Equals(Route(actual), Route(expected), StringComparison.OrdinalIgnoreCase))
    ];

    private static string Route(string route) => Constraint.Replace(route.Trim().Trim('/'), "}");
}
