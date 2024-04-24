using Intent.Metadata.Models;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class EmptyElementRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public EmptyElementRenderer(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        //var htmlElement = new HtmlElement(component.Name, _template.BlazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, node);
        }
        //node.AddNode(htmlElement);
    }
}