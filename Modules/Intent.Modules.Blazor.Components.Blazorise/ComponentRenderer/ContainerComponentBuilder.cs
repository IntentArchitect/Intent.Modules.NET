using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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
        var htmlElement = new HtmlElement("Row", _componentTemplate.RazorFile)
            .AddHtmlElement("Column", column =>
            {
                foreach (var child in component.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, column);
                }
            });
        parentNode.AddChildNode(htmlElement);
    }
}