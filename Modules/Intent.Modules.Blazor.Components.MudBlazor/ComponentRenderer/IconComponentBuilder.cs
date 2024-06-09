using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Razor;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class IconComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    static IconComponentBuilder()
    {
        DefaultRazorComponentBuilderProvider.Register(IconModel.SpecializationTypeId, (provider, componentTemplate) => new IconComponentBuilder(provider, componentTemplate));
    }

    public IconComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var iconModel = new IconModel(component);
        var htmlElement = new HtmlElement("MudIcon ", _componentTemplate.RazorFile)
            .AddAttribute("Icon", $"fab fa-{iconModel.GetIconAppearance().Name().Source}");
        ;
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }

        var onClickMapping = _bindingManager.GetMappedEndFor(iconModel, "On Click");
        if (onClickMapping != null)
        {
            if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            {
                var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode)}");
            }
            else
            {
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode)}");
            }
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        parentNode.AddChildNode(htmlElement);
    }
}