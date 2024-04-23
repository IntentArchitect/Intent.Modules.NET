using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class ContainerComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;

    public ContainerComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var button = new ContainerModel(component);
        var htmlElement = new HtmlElement("Row", _template.BlazorFile)
            .AddHtmlElement("Column", column =>
            {
                foreach (var child in component.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, column);
                }
            });
        node.AddChildNode(htmlElement);
    }
}