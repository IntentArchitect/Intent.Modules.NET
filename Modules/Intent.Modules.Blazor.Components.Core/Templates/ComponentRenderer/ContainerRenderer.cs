using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class ContainerRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly IRazorComponentTemplate _template;

    public ContainerRenderer(IRazorComponentBuilderResolver componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var button = new ContainerModel(component);
        var htmlElement = new HtmlElement("div", _template.BlazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        node.AddNode(htmlElement);
    }
}