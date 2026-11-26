using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// How ASP.NET Core states an endpoint's OpenAPI operationId (<c>operation.openApi.operationId</c>) and whether it is left out
/// of the API description (<c>operation.openApi.ignore</c>), as the framework's own API explorer and OpenAPI document read them.
/// <para>
/// The operationId is <c>ApiDescription.ActionDescriptor.AttributeRouteInfo.Name</c>, else the last
/// <c>IEndpointNameMetadata</c>'s name (Microsoft.AspNetCore.OpenApi's <c>OpenApiDocumentService.GetOperationId</c>, and
/// Swashbuckle's default operationId selector). So a Minimal API endpoint's is its last <c>.WithName(...)</c>, else its
/// handler's <c>[EndpointName]</c>, unless an OpenAPI operation callback chained on it (<c>.WithOpenApi(op => ...)</c>,
/// <c>.AddOpenApiOperationTransformer(...)</c>) sets <c>OperationId</c> itself; a controller action's is its route attribute's
/// <c>Name</c> (<c>[HttpGet("{id}", Name = "GetItem")]</c>, <c>[Route(..., Name = ...)]</c>, the controller's route's when the
/// action's routes give neither a template nor a name), else its <c>[EndpointName]</c>. An endpoint with none states none, so
/// <c>unmodelled.value</c> doesn't read it as a value. Web API 2 has no OpenAPI of its own, so its route names aren't
/// operationIds, and an operationId a library's attribute gives (Swashbuckle's <c>[SwaggerOperation]</c>, NSwag's
/// <c>[OpenApiOperation]</c>) isn't the framework's: both are can't verify.
/// </para>
/// <para>
/// A Minimal API endpoint is left out by <c>ExcludeFromDescription()</c> (or the metadata it adds) on itself, its group or its
/// router's mapping call, or by <c>[ExcludeFromDescription]</c> on its handler: the only metadata Minimal APIs' API description
/// provider reads. A controller action is left out by the first <c>[ApiExplorerSettings]</c> on the action, else on its
/// controller or the controller's bases, as MVC's application model reads it (an action's settings override its
/// controller's); MVC doesn't read <c>[ExcludeFromDescription]</c>. Silence is <c>false</c>: an endpoint is described unless
/// something leaves it out.
/// </para>
/// </summary>
internal static class OpenApi
{
    public const string OperationIdKind = "operation.openApi.operationId";
    public const string IgnoreKind = "operation.openApi.ignore";

    /// <summary>The route attributes whose <c>Name</c> is an action's route name.</summary>
    private static readonly HashSet<string> RouteAttributes = ["Route", "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "HttpHead", "HttpOptions", "AcceptVerbs"];

    /// <summary>A Minimal API endpoint's operationId: its OpenAPI callback's, else its last name, else its handler's <c>[EndpointName]</c>.</summary>
    public static ClaimReading OperationId(ClaimContext context, MinimalEndpoint endpoint, MemberNode member, TypeNode router)
    {
        if ((endpoint.OperationId ?? endpoint.EndpointName) is { } stated)
        {
            return endpoint.TextIn(context.Code, stated.Value) is { } value
                ? ClaimReading.Stated(value, stated.At)
                : Unreadable(stated.Value.Written, stated.At);
        }

        return EndpointName(context, member, router) ?? ClaimReading.Absent(endpoint.At);
    }

    /// <summary>Whether a Minimal API endpoint is left out of the API description: by itself, its handler, its group or its router's mapping call.</summary>
    public static ClaimReading Ignore(MinimalApiModel model, MinimalEndpoint endpoint, MemberNode member, TypeNode router)
    {
        var excluded = endpoint.Excluded
                       ?? member.Attributes.FirstOrDefault(x => x.Name == "ExcludeFromDescription")?.At
                       ?? endpoint.GroupExcluded
                       ?? (model.RouterExcluded.TryGetValue(router.Name, out var mapping) ? mapping : null);
        return excluded is { } at ? ClaimReading.Stated("true", at) : ClaimReading.Absent(endpoint.At, "false");
    }

    /// <summary>A controller action's operationId or API description setting; null for a method that isn't an action of a controller.</summary>
    public static ClaimReading? Action(ClaimContext context)
    {
        var member = context.Member ?? context.MemberOf(context.Element);
        var owner = (member is null ? null : context.Code.Types.FirstOrDefault(x => x.Members.Contains(member))) ?? context.TypeOf(context.Owner(context.Element)!);
        if (member is null || owner is null || !AspNetCoreClaims.IsController(context.Code, owner))
        {
            return null;
        }

        var controller = new[] { owner }.Concat(context.Code.Bases(owner)).ToList();
        if (context.Kind == IgnoreKind)
        {
            var settings = member.Attributes.FirstOrDefault(x => x.Name == "ApiExplorerSettings")
                           ?? controller.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name == "ApiExplorerSettings");
            if (settings is null)
            {
                return ClaimReading.Absent(member.At, "false");
            }

            // [ApiExplorerSettings(GroupName = "v1")] states the default: IgnoreApi is false unless set.
            var ignore = settings.Values.FirstOrDefault(x => x.Name == "IgnoreApi");
            var value = ignore is null ? "false" : ignore.Literal ?? context.Code.LiteralOf(ignore.Text, owner)?.Value;
            return value is "true" or "false" ? ClaimReading.Stated(value, settings.At) : Unreadable(ignore!.Text, settings.At);
        }

        if (Binding.WebApi2(context.Code, owner))
        {
            return ClaimReading.CantVerify("unresolved",
                $"`{owner.Name}` is an ASP.NET Web API 2 controller, which has no OpenAPI of its own: a route's name isn't an operationId, and what an OpenAPI generator names `{member.Name}` isn't in the code.",
                member.At);
        }

        if (member.Attributes.FirstOrDefault(x => x.Name is "SwaggerOperation" or "OpenApiOperation") is { } library)
        {
            return ClaimReading.CantVerify("unresolved",
                $"`[{library.Name}]` on `{owner.Name}.{member.Name}` is a library's attribute: whether it names the operationId depends on the library's options, which aren't in the code read, and ASP.NET Core's own operationId is the route's name.",
                library.At);
        }

        // An action's route attribute's name; with no template or name on any of them, its controller's route's (MVC's ChooseName).
        var routes = member.Attributes.Where(x => RouteAttributes.Contains(x.Name)).ToList();
        var routed = routes.FirstOrDefault(x => Named(x) is not null)
                     ?? (routes.All(x => x.Arguments.Count == 0)
                         ? controller.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name == "Route" && Named(x) is not null)
                         : null);
        if (routed is not null)
        {
            var name = Named(routed)!;
            return name.Literal is { } literal ? ClaimReading.Stated(literal, routed.At)
                : context.Code.ConstantOf(name.Text, owner) is { } constant ? ClaimReading.Stated(constant, routed.At)
                : Unreadable(name.Text, routed.At);
        }

        return EndpointName(context, member, owner) ?? ClaimReading.Absent(member.At);
    }

    /// <summary>The name <c>[EndpointName("...")]</c> on the handler or action gives it; null when it has none.</summary>
    private static ClaimReading? EndpointName(ClaimContext context, MemberNode member, TypeNode owner)
    {
        if (member.Attributes.FirstOrDefault(x => x.Name == "EndpointName") is not { } attribute || attribute.Values.FirstOrDefault(x => x.Name is null) is not { } value)
        {
            return null;
        }

        return value.Literal is { } literal ? ClaimReading.Stated(literal, attribute.At)
            : context.Code.ConstantOf(value.Text, owner) is { } constant ? ClaimReading.Stated(constant, attribute.At)
            : Unreadable(value.Text, attribute.At);
    }

    private static AttributeValueNode? Named(AttributeNode attribute) => attribute.Values.FirstOrDefault(x => x.Name == "Name");

    private static ClaimReading Unreadable(string written, Span at) =>
        ClaimReading.CantVerify("unreadable", $"The code gives it as `{written}`, which the rule can't read without running the code: it isn't a literal or a constant the code declares.", at);
}
