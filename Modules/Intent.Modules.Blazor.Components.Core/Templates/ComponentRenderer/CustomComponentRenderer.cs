using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class CustomComponentRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public CustomComponentRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var model = new ComponentModel((IElement)component.TypeReference.Element);
        var htmlElement = new HtmlElement(_template.GetTypeName(component.TypeReference), _template.RazorFile);

        var mapping = _template.Model.View.InternalElement.Mappings.FirstOrDefault();
        foreach (var modelProperty in model.Properties.Where(x => x.HasBindable()))
        {
            var mappedEnd = mapping.MappedEnds.FirstOrDefault(x => x.TargetElement.Id == modelProperty.Id);
            if (mappedEnd is null)
            {
                continue;
            }

            htmlElement.AddAttributeIfNotEmpty(modelProperty.Name.ToPropertyName(), _template.GetBinding(mappedEnd));
        }
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).Render(child, htmlElement);
        }
        node.AddNode(htmlElement);
    }
}