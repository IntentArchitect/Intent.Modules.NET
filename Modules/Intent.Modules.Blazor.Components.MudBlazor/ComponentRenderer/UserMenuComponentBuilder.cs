using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class UserMenuComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public UserMenuComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        // Renders the circuit-free <AppUserMenu/> that the Blazor Authentication module ships into
        // Components/Account/Shared (Profile / My Account / antiforgery-POST Logout). Its namespace is made
        // available to consuming layouts via the app's root Components/_Imports.razor — contributed by the
        // Authentication module's ChangeRenderMode factory extension — so no inline @using is emitted here.
        var htmlElement = new HtmlElement("AppUserMenu", _componentTemplate.RazorFile);
        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}
