using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// Where ASP.NET binds a parameter that no binding attribute places (user decision, 2026-11-14: the Services model is a
/// logical projection of the endpoint, so a parameter the route names is a route parameter, attribute or not, and the model
/// must say so). The rules are the framework's own, alike for a Minimal API endpoint, an <c>[ApiController]</c> controller
/// and a Web API 2 controller:
/// <list type="bullet">
/// <item>A name in the endpoint's route template, the router's or controller's part included, is the route.</item>
/// <item>Web API 2's <c>[FromUri]</c> is the route, else the query string.</item>
/// <item>A form file is the form; a service (<see cref="MinimalApiClaims.IsInjected"/>) is the container's.</item>
/// <item>An <c>[AsParameters]</c> type is where its members are, when they all agree.</item>
/// <item>
/// A simple type (a primitive, string, Guid, a date or time, an enum, or a type of the code's own with a <c>TryParse</c> or
/// a <c>[TypeConverter]</c>) is the query string, and so is a Minimal API's array or list of them.
/// </item>
/// <item>Any other type is the body.</item>
/// </list>
/// An MVC controller without <c>[ApiController]</c> binds a complex type from the form, the route or the query string,
/// whichever has its fields, so the code doesn't say which.
/// </summary>
internal static class Binding
{
    private static readonly HashSet<string> Simple = new(StringComparer.Ordinal)
    {
        "string", "String", "char", "Char", "bool", "Boolean", "byte", "Byte", "sbyte", "SByte", "short", "Int16", "ushort", "UInt16", "int", "Int32", "uint",
        "UInt32", "long", "Int64", "ulong", "UInt64", "float", "Single", "double", "Double", "decimal", "Decimal", "Guid", "DateTime", "DateTimeOffset", "DateOnly",
        "TimeOnly", "TimeSpan", "Uri", "Version"
    };

    private static readonly HashSet<string> FormFiles = new(StringComparer.Ordinal) { "IFormFile", "IFormFileCollection", "IFormCollection" };

    private static readonly HashSet<string> Lists = new(StringComparer.Ordinal)
    {
        "IEnumerable", "List", "IList", "ICollection", "IReadOnlyList", "IReadOnlyCollection", "HashSet", "ISet", "StringValues"
    };

    private static readonly HashSet<string> Verbs = new(StringComparer.Ordinal) { "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "HttpHead", "HttpOptions" };

    /// <summary>The calls that map a conventional route, for a controller whose actions have no route of their own.</summary>
    private static readonly HashSet<string> Conventional =
        new(StringComparer.Ordinal) { "MapHttpRoute", "MapRoute", "MapControllerRoute", "MapAreaControllerRoute", "MapDefaultControllerRoute" };

    /// <summary>The calls an ASP.NET Core application maps a controller's conventional route with; an area's needs its controller's <c>[Area]</c>, so isn't one.</summary>
    private static readonly HashSet<string> CoreConventional = new(StringComparer.Ordinal) { "MapControllerRoute", "MapRoute", "MapDefaultControllerRoute" };

    /// <summary>The route <c>MapDefaultControllerRoute()</c> maps.</summary>
    private const string DefaultControllerRoute = "{controller=Home}/{action=Index}/{id?}";

    private static readonly Regex ControllerToken = new(@"\{controller(=[^}]*)?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ActionToken = new(@"\{action(=[^}]*)?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex RouteNames = new(@"\{\**([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    /// <summary>A template as the route comparer reads it: no slashes at its ends, no constraints, case aside.</summary>
    internal static string Normal(string template) => Regex.Replace(template.Trim().Trim('/'), @"(?::[^}]*?)?(\??)\}", "$1}").ToLowerInvariant();

    public static ClaimReading Infer(ClaimContext context, MemberNode member, ParameterNode parameter)
    {
        var code = context.Code;
        var owner = code.Types.FirstOrDefault(x => x.Members.Contains(member))
                    ?? (context.Owner(context.Element) is { } operation && context.Owner(operation) is { } service ? context.TypeOf(service) : null);
        if (owner is null)
        {
            return ClaimReading.Absent(parameter.At);
        }

        var endpoint = code.Get<MinimalApiModel>()?.Endpoints
            .FirstOrDefault(x => x.Router == owner.Name && (x.Lambda is { } lambda ? lambda.At == member.At : x.Handler == member.Name));
        var modelled = context.Owner(context.Element) is { } modelledOperation ? context.Model.Stereotype(modelledOperation, "Http Settings", "Route") : null;
        var route = (endpoint is not null ? [endpoint.GroupIn(code) ?? endpoint.Group ?? "", endpoint.TemplateIn(code) ?? endpoint.Template] : Templates(code, owner, member, modelled))
            .SelectMany(x => RouteNames.Matches(x).Select(m => m.Groups[1].Value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var body = endpoint is not null
                   || new[] { owner }.Concat(code.Bases(owner)).Any(x => x.Attributes.Any(a => a.Name == "ApiController"))
                   || WebApi2(code, owner);
        // What ASP.NET infers is its convention, not a value the code states, so unmodelled.value doesn't report it.
        if (Source(code, owner, parameter, route, body, endpoint is not null, nested: false) is { } source)
        {
            return ClaimReading.Defaulted(source, parameter.At, "no binding attribute");
        }

        return ClaimReading.CantVerify("inferred",
            $"`{owner.Name}` is an MVC controller without `[ApiController]`, which binds `{parameter.Name}` from the form, the route or the query string, whichever has its fields, so the code doesn't say which. Give it a binding attribute.",
            parameter.At);
    }

    /// <summary>A Web API 2 controller: one deriving from <c>System.Web.Http</c>'s <c>ApiController</c>.</summary>
    public static bool WebApi2(CodeGraph code, TypeNode owner) => code.Supertypes(owner).Contains("ApiController");

    /// <summary>An OData controller, Web API 2's or ASP.NET Core's, which OData routes by convention (<see cref="ODataRoute"/>).</summary>
    public static bool OData(CodeGraph code, TypeNode owner) => code.Supertypes(owner).Contains("ODataController");

    /// <summary>The action has a route template of its own: a <c>[Route]</c>, or a verb attribute that names one.</summary>
    public static bool HasOwnRoute(MemberNode member) => member.Attributes.Any(x => (x.Name == "Route" || Verbs.Contains(x.Name)) && x.Arguments.Count > 0);

    /// <summary>
    /// The route Web API 2 serves a controller action on when it has no route of its own: attribute routing covers only
    /// actions with a <c>[Route]</c>, so a controller's <c>[RoutePrefix]</c> doesn't route the others, and they are
    /// reached through the first conventional route <c>MapHttpRoute</c> maps with a <c>{controller}</c>
    /// (<c>api/{controller}/{id}</c> by default). Its <c>{controller}</c> and <c>{action}</c> are the action's, and a value
    /// the route's defaults make optional (<c>id = RouteParameter.Optional</c>) is left out when the action doesn't take
    /// it, so <c>PolicyController.PostAsync(request)</c> is <c>~/api/Policy</c>: absolute, as it isn't under the
    /// controller's route. Null for an action with its own route, or when no conventional route names a controller.
    /// <para>
    /// ASP.NET Core routes an action conventionally only when neither it nor its controller has an attribute route: a
    /// controller's <c>[Route]</c> routes all its actions, and <c>[ApiController]</c> requires one. Its conventional routes
    /// are <c>MapControllerRoute</c>'s pattern and <c>MapDefaultControllerRoute()</c>'s
    /// <c>{controller=Home}/{action=Index}/{id?}</c>, the first with a <c>{controller}</c> and an <c>{action}</c>, so
    /// <c>AccountController.Login()</c> is <c>~/Account/Login</c>: a token's default is still the action's own name in the
    /// full route.
    /// </para>
    /// </summary>
    public static string? ConventionalRoute(CodeGraph code, TypeNode owner, MemberNode member)
    {
        if (HasOwnRoute(member))
        {
            return null;
        }

        if (OData(code, owner))
        {
            return ODataRoute(code, owner, member);
        }

        var webApi2 = WebApi2(code, owner);
        var controllers = new[] { owner }.Concat(code.Bases(owner)).ToList();
        if (!webApi2 && controllers.Any(x => x.Attributes.Any(a => a.Name == "ApiController" || a.Name == "Route" && a.Arguments.Count > 0)))
        {
            return null;
        }

        foreach (var call in MinimalApiClaims.AllCalls(code).Where(x => webApi2 ? x.Name == "MapHttpRoute" : CoreConventional.Contains(x.Name)))
        {
            var quoted = call.Name == "MapDefaultControllerRoute"
                ? $"\"{DefaultControllerRoute}\""
                : call.Arguments.Select(x => x.Text).FirstOrDefault(x => x.StartsWith('"') && ControllerToken.IsMatch(x) && (webApi2 || ActionToken.IsMatch(x)));
            if (quoted is null)
            {
                continue;
            }

            var optional = call.Arguments.SelectMany(x => OptionalValues.Matches(x.Text).Select(m => m.Groups[1].Value)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var controller = owner.Name.EndsWith("Controller", StringComparison.Ordinal) ? owner.Name[..^"Controller".Length] : owner.Name;
            var action = member.Attributes.FirstOrDefault(x => x.Name == "ActionName" && x.Arguments.Count > 0)?.Arguments[0].Trim('"') ?? member.Name;
            var segments = new List<string>();
            foreach (var segment in quoted.Trim('"').Trim('/').Split('/'))
            {
                // A token's default (`{action=Index}`) makes it optional; its name is before the `=`.
                var token = segment.Trim('{', '}');
                var name = token.Split('=')[0].TrimEnd('?');
                if (segment.StartsWith('{') && token.Contains('='))
                {
                    optional.Add(name);
                }

                if (segment.StartsWith('{') && name.Equals("controller", StringComparison.OrdinalIgnoreCase))
                {
                    segments.Add(controller);
                }
                else if (segment.StartsWith('{') && name.Equals("action", StringComparison.OrdinalIgnoreCase))
                {
                    segments.Add(action);
                }
                else if (!segment.StartsWith('{')
                         || !(optional.Contains(name) || segment.EndsWith("?}", StringComparison.Ordinal))
                         || member.Parameters.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    segments.Add(segment);
                }
            }

            return "~/" + string.Join("/", segments);
        }

        return null;
    }

    /// <summary>
    /// The route OData's conventional routing serves an <c>ODataController</c>'s action on: the prefix its route is mapped
    /// under (Web API 2's <c>MapODataServiceRoute("odata", "odata", model)</c> or <c>MapODataRoute</c>, ASP.NET Core's
    /// <c>AddRouteComponents("odata", model)</c>), then the entity set or singleton the EDM model names after the controller
    /// (<c>EntitySet&lt;RequestPack&gt;("RequestPacks")</c> for <c>RequestPacksController</c>), with <c>({key})</c> for an action
    /// that takes the key. So <c>RequestPacksController.GetRequestPacks()</c> is <c>~/odata/RequestPacks</c>, which a migrated
    /// Web API 2 application's v1 recipe read by convention. Null for an attribute-routed action (<c>[ODataRoute]</c>), or a
    /// controller no entity set or singleton names, which OData doesn't route.
    /// </summary>
    private static string? ODataRoute(CodeGraph code, TypeNode owner, MemberNode member)
    {
        if (member.Attributes.Any(x => x.Name == "ODataRoute"))
        {
            return null;
        }

        var calls = MinimalApiClaims.AllCalls(code).ToList();
        string? Text(CallNode call, int index) => call.Arguments.Count > index && code.StringOf(call.Arguments[index]) is { Kind: "string" } literal ? literal.Value : null;
        var name = owner.Name.EndsWith("Controller", StringComparison.Ordinal) ? owner.Name[..^"Controller".Length] : owner.Name;
        if (!calls.Any(x => x.Name is "EntitySet" or "Singleton" && Text(x, 0) == name))
        {
            return null;
        }

        var prefix = calls.Select(x => x.Name switch
            {
                "MapODataServiceRoute" or "MapODataRoute" => Text(x, 1),
                "AddRouteComponents" when x.Arguments.Count > 1 => Text(x, 0),
                _ => null
            })
            .FirstOrDefault(x => x is not null) ?? "";
        var key = member.Parameters.Any(x => x.Name == "key" || x.Attributes.Any(a => a.Name == "FromODataUri")) ? "({key})" : "";
        return "~/" + string.Join("/", new[] { prefix.Trim('/'), name + key }.Where(x => x.Length > 0));
    }

    /// <summary>A route value the defaults make optional: <c>id = RouteParameter.Optional</c>.</summary>
    private static readonly Regex OptionalValues = new(@"([A-Za-z_][A-Za-z0-9_]*)\s*=\s*RouteParameter\.Optional", RegexOptions.Compiled);

    private static string? Source(CodeGraph code, TypeNode from, ParameterNode parameter, ISet<string> route, bool body, bool minimal, bool nested)
    {
        if (route.Contains(parameter.Name))
        {
            return "From Route";
        }

        if (parameter.Type is not { } type)
        {
            return null;
        }

        if (parameter.Attributes.Any(x => x.Name == "FromUri"))
        {
            return "From Query";
        }

        if (FormFiles.Contains(type.Name))
        {
            return "From Form";
        }

        if (!nested && MinimalApiClaims.IsInjected(parameter, code))
        {
            return "From Services";
        }

        if (!nested && parameter.Attributes.Any(x => x.Name == "AsParameters"))
        {
            var sources = Members(code, from, type).Select(x => Source(code, from, x, route, body, minimal, nested: true)).Distinct().ToList();
            return sources is [{ } only] ? only : null;
        }

        // An array's element is its one type argument, as a list's is.
        if (IsSimple(code, from, type) || minimal && (type.IsArray || Lists.Contains(type.Name)) && type.Arguments is [var item] && IsSimple(code, from, item))
        {
            return "From Query";
        }

        return body ? "From Body" : null;
    }

    private static bool IsSimple(CodeGraph code, TypeNode from, TypeRefNode type)
    {
        if (type.IsArray)
        {
            return false;
        }

        if (type is { Name: "Nullable", Arguments: [var underlying] })
        {
            return IsSimple(code, from, underlying);
        }

        if (Simple.Contains(type.Name) && type.Arguments.Count == 0)
        {
            return true;
        }

        return code.TypeFor(type, from) is { } declared
               && (declared.Kind == "enum"
                   || declared.Members.Any(x => x is { Kind: "method", Name: "TryParse", IsStatic: true })
                   || declared.Attributes.Any(x => x.Name == "TypeConverter"));
    }

    /// <summary>An <c>[AsParameters]</c> type's members, as parameters: its properties, or a record's positional ones.</summary>
    private static IEnumerable<ParameterNode> Members(CodeGraph code, TypeNode from, TypeRefNode type)
    {
        if (code.TypeFor(type, from) is not { } declared)
        {
            return [];
        }

        var properties = code.MembersOf(declared).Where(x => x is { Kind: "property", IsStatic: false }).ToList();
        return properties.Count > 0
            ? properties.Select(x => new ParameterNode { Name = x.Name, Type = x.Type, Attributes = x.Attributes, At = x.At })
            : declared.Members.FirstOrDefault(x => x.IsPrimary)?.Parameters ?? [];
    }

    /// <summary>
    /// A controller action's route templates: its controller's <c>[Route]</c> or <c>[RoutePrefix]</c>, inherited ones
    /// included, and its own <c>[Route]</c> or verb attribute's; with none of its own, an ASP.NET Core controller's route,
    /// else the conventional routes mapped (a Web API 2 action with no route of its own isn't under its controller's
    /// <c>[RoutePrefix]</c>, <see cref="ConventionalRoute"/>). An action with several routes binds a parameter from whichever
    /// one a request comes in by, and the diagram shows one: the route the model states, else the first, which
    /// <c>operation.route</c> reads.
    /// </summary>
    private static IEnumerable<string> Templates(CodeGraph code, TypeNode owner, MemberNode member, string? modelled)
    {
        var own = member.Attributes.Where(x => (x.Name == "Route" || Verbs.Contains(x.Name)) && x.Arguments.Count > 0).Select(x => x.Arguments[0]).ToList();
        if (own.Count > 1)
        {
            own = [own.FirstOrDefault(x => modelled is not null && Normal(x) == Normal(modelled)) ?? own[0]];
        }

        var controller = new[] { owner }.Concat(code.Bases(owner))
            .SelectMany(x => x.Attributes.Where(a => a.Name is "Route" or "RoutePrefix" && a.Arguments.Count > 0).Select(a => a.Arguments[0]));
        if (own.Count > 0 || controller.Any() && !WebApi2(code, owner))
        {
            return controller.Concat(own);
        }

        return MinimalApiClaims.AllCalls(code)
            .Where(x => Conventional.Contains(x.Name))
            .SelectMany(x => x.Name == "MapDefaultControllerRoute" ? new[] { DefaultControllerRoute } : x.Arguments.Where(a => a.Text.StartsWith('"')).Select(a => a.Text));
    }
}
