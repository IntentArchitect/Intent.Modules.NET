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
        var htmlElement = new HtmlElement(_template.GetTypeName(component.TypeReference), _template.BlazorFile);

        foreach (var modelProperty in model.Properties.Where(x => x.HasBindable()))
        {
            var mappedEnd = _template.GetMappedEndFor(modelProperty);
            if (mappedEnd is null)
            {
                continue;
            }

            htmlElement.AddAttributeIfNotEmpty(modelProperty.Name.ToPropertyName(), _template.GetBinding(mappedEnd));
        }

        foreach (var eventEmitter in model.EventEmitters.Where(x => x.HasBindable()))
        {
            var mappedEnd = _template.GetMappedEndFor(eventEmitter);
            if (mappedEnd is null)
            {
                continue;
            }

            var operation = mappedEnd.SourceElement.AsOperationModel();
            if (operation?.Parameters.Count == 1 && operation.Parameters.Single().TypeReference.Element.Id == eventEmitter.TypeReference.Element.Id)
            {
                htmlElement.AddAttributeIfNotEmpty(eventEmitter.Name.ToPropertyName(), $"{operation.Parameters.Single().Name.ToParameterName()} => {_template.GetBinding(mappedEnd)}({operation.Parameters.Single().Name.ToParameterName()})");
            }
            else
            {
                htmlElement.AddAttributeIfNotEmpty(eventEmitter.Name.ToPropertyName(), _template.GetBinding(mappedEnd));
            }

        }

        //foreach (var stereotypeProperty in model.Stereotypes.SelectMany(x => x.Properties))
        //{
        //    var mappedEnd = _template.GetMappedEndFor(model, stereotypeProperty.Key);
        //    if (mappedEnd is null)
        //    {
        //        continue;
        //    }
        //    htmlElement.AddAttributeIfNotEmpty(stereotypeProperty.Key.ToPropertyName(), _template.GetBinding(mappedEnd));
        //}

        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).Render(child, htmlElement);
        }
        node.AddNode(htmlElement);
    }
}