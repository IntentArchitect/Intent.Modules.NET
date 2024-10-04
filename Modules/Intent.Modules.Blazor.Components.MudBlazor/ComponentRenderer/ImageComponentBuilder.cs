using System.Collections.Generic;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class ImageComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public ImageComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new ImageModel(component);
        var htmlElement = new HtmlElement("MudImage", _componentTemplate.RazorFile)
            .AddAttributeIfNotEmpty("Src", model.Value)
            .AddAttributeIfNotEmpty("Fluid", model.GetAppearance().Fluid() ? "true" : null)
            .AddAttributeIfNotEmpty("Width", model.GetAppearance().Width()?.ToString())
            .AddAttributeIfNotEmpty("Height", model.GetAppearance().Height()?.ToString())
            .AddAttributeIfNotEmpty("Elevation", model.GetAppearance().Elevation()?.ToString())
            .AddAttributeIfNotEmpty("Class", model.GetAppearance().Class())
            ;
        foreach (var child in component.ChildElements)
        {
            _componentResolver.BuildComponent(child, htmlElement);
        }

        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}