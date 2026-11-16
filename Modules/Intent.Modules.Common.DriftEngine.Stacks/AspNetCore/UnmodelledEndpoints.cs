using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// <c>unmodelled.endpoint</c>: a Minimal API endpoint the code maps in checked code, a lambda or a method group, that no
/// Operation's <c>endpoint.exists</c> located. It is named by its verb and route. A controller's actions are public methods,
/// which <c>unmodelled.member</c> reports, and a controller no Service describes is <c>unmodelled.type</c>'s.
/// </summary>
internal sealed class UnmodelledEndpoints : IOptInRule
{
    public string Id => "unmodelled.endpoint";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        foreach (var endpoint in context.Code.Get<MinimalApiModel>()?.Endpoints ?? [])
        {
            var router = context.Code.Types.FirstOrDefault(x => x.Name == endpoint.Router && x.Files.Contains(endpoint.At.File));
            var handler = endpoint.Handler is null ? null : router?.Members.FirstOrDefault(x => x.Kind == "method" && x.Name == endpoint.Handler);
            if (!context.Scope.Checks(endpoint.At.File) || context.IsLocated(endpoint.At) || handler is not null && context.IsLocated(handler))
            {
                continue;
            }

            var route = string.Join("/", new[] { endpoint.GroupIn(context.Code) ?? endpoint.Group, endpoint.TemplateIn(context.Code) ?? endpoint.Template }
                .Select(x => x?.Trim('/')).Where(x => !string.IsNullOrEmpty(x)));
            var shown = $"{endpoint.Verb.ToUpperInvariant()} /{route}";
            var owner = router is null ? null : context.ElementOf(router);
            yield return new UnmodelledCode(owner?.Designer ?? "Services", endpoint.At, shown,
                $"The endpoint `{shown}` in `{endpoint.Router}` isn't in the model. Model it as an Operation with Http Settings{(owner is null ? "" : $" on `{owner.Value.Element.Name}`")}, or add an exception with a reason to the Root Folder's Drift Check.",
                owner?.Element.Id, endpoint.Router);
        }
    }
}
