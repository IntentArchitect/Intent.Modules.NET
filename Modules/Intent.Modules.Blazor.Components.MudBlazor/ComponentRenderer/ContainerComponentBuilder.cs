using System.Collections.Generic;
using System.Data.Common;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class ContainerComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public ContainerComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var results = new List<IRazorFileNode>();
        foreach (var child in component.ChildElements)
        {
            results.AddRange(_componentResolver.BuildComponent(child, parentNode));
        }

        return results;
        var htmlElement = parentNode.AddHtmlElement("MudContainer", htmlElement =>
        {
            foreach (var child in component.ChildElements)
            {
                _componentResolver.BuildComponent(child, htmlElement);
            }
        });
        return [htmlElement];
    }
}