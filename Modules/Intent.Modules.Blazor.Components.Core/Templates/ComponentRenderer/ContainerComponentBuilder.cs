using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class ContainerComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public ContainerComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var button = new ContainerModel(component);
        var htmlElement = new HtmlElement("div", _componentTemplate.RazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        parentNode.AddChildNode(htmlElement);
    }
}