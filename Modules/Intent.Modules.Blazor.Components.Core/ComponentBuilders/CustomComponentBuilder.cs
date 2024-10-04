using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

public class CustomComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public CustomComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new ComponentModel((IElement)component.TypeReference.Element);
        var htmlElement = new HtmlElement(_componentTemplate.GetTypeName(component.TypeReference), _componentTemplate.RazorFile);

        foreach (var modelProperty in model.Properties.Where(x => x.HasBindable()))
        {
            var mappedEnd = _bindingManager.GetMappedEndFor(modelProperty);
            if (mappedEnd is null)
            {
                continue;
            }

            htmlElement.AddAttributeIfNotEmpty(modelProperty.Name.ToPropertyName(), _bindingManager.GetBinding(mappedEnd)?.ToString());
        }

        foreach (var eventEmitter in model.EventEmitters.Where(x => x.HasBindable()))
        {
            var mappedEnd = _bindingManager.GetMappedEndFor(eventEmitter);
            if (mappedEnd is null)
            {
                continue;
            }

            var operation = mappedEnd.SourceElement.AsComponentOperationModel();
            if (operation?.Parameters.Count == 1 && operation.Parameters.Single().TypeReference.Element.Id == eventEmitter.TypeReference.Element.Id)
            {
                htmlElement.AddAttributeIfNotEmpty(eventEmitter.Name.ToPropertyName(), $"{operation.Parameters.Single().Name.ToParameterName()} => {_bindingManager.GetBinding(mappedEnd)}({operation.Parameters.Single().Name.ToParameterName()})");
            }
            else
            {
                htmlElement.AddAttributeIfNotEmpty(eventEmitter.Name.ToPropertyName(), _bindingManager.GetBinding(mappedEnd)?.ToString());
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
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        parentNode.AddChildNode(htmlElement);
    }
}