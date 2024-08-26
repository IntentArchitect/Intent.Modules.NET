using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public class EmptyElementRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public EmptyElementRenderer(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        //var htmlElement = new HtmlElement(component.Name, _template.BlazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.BuildComponent(child, parentNode);
        }

        return [];
        //node.AddNode(htmlElement);
    }
}