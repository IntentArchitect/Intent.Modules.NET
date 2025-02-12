﻿using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class SelectComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public SelectComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var selectModel = new SelectModel(component);
        var htmlElement = new HtmlElement("MudSelect", _componentTemplate.RazorFile);
        var valueMapping = _bindingManager.GetMappedEndFor(selectModel);
        var valueBinding = _bindingManager.GetBinding(valueMapping, parentNode)?.ToString();

        var onSelectedMapping = _bindingManager.GetMappedEndFor(selectModel, "919d2201-db56-448c-bdc8-7f2fd353cfd8"); // On Selected
        var onSelectedBinding = _bindingManager.GetBinding(onSelectedMapping, parentNode);

        htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetBinding(valueMapping, parentNode)?.ToString());
        htmlElement.AddAttribute("Label", selectModel.TryGetLabelAddon(out var label) ? label.Label() : selectModel.Name);
        htmlElement.AddAttributeIfNotEmpty("@bind-Value:after", onSelectedBinding?.ToLambda());

        parentNode.AddChildNode(htmlElement);
        var mappedEnd = _bindingManager.GetMappedEndFor(selectModel, "Options");
        if (mappedEnd == null)
        {
            return [htmlElement];
        }
        htmlElement.AddCodeBlock($"@foreach (var item in {_bindingManager.GetBinding(selectModel, "Options")})", code =>
        {
            htmlElement.AddMappingReplacement(mappedEnd.SourceElement, "item");
            code.AddHtmlElement("MudSelectItem", selectItem =>
            {
                var selectItemMapping = _bindingManager.GetMappedEndFor(selectModel, "Value");
                selectItem.AddAttribute("T", _componentTemplate.GetTypeName(valueMapping.SourceElement.TypeReference));
                // TODO: Use bindings:
                selectItem.AddAttribute("Value", _bindingManager.GetBinding(selectModel, "Value", htmlElement)?.ToString())
                    .WithText(_bindingManager.GetBinding(selectModel, "Text", htmlElement)?.ToString());
            });
        });
        
        if (parentNode.GetAllNodesInHierarchy().OfType<HtmlElement>().Any(x => x.Name == "MudForm"))
        {
            if (valueMapping != null)
            {
                htmlElement.AddAttribute("For", $"@(() => {valueBinding})");
            }
        }
        return [htmlElement];
    }
}
