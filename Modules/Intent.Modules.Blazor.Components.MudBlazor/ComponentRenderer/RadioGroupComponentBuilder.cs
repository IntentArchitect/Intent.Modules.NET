using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modelers.UI.Core.Api.RadioGroupModelStereotypeExtensions;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;
public class RadioGroupComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public RadioGroupComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var radioModel = new RadioGroupModel(component);
        var htmlElement = new HtmlElement("MudRadioGroup", _componentTemplate.RazorFile);
        var valueMapping = _bindingManager.GetMappedEndFor(radioModel);
        var valueBinding = _bindingManager.GetBinding(valueMapping, parentNode)?.ToString();

        var onSelectedMapping = _bindingManager.GetMappedEndFor(radioModel, "25a22edc-62f5-4daa-bb78-6e7a59855d6f"); // On Selected
        var onSelectedBinding = _bindingManager.GetBinding(onSelectedMapping, parentNode);

        htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetBinding(valueMapping, parentNode)?.ToString());
        htmlElement.AddAttribute("Label", radioModel.TryGetLabelAddon(out var label) ? label.Label() : radioModel.Name);
        htmlElement.AddAttributeIfNotEmpty("@bind-Value:after", onSelectedBinding?.ToLambda());

        parentNode.AddChildNode(htmlElement);
        var mappedEnd = _bindingManager.GetMappedEndFor(radioModel, "Options");
        if (mappedEnd == null)
        {
            return [htmlElement];
        }

        htmlElement.AddCodeBlock($"@foreach (var item in {_bindingManager.GetBinding(radioModel, "Options")})", code =>
        {
            htmlElement.AddMappingReplacement(mappedEnd.SourceElement, "item");

            if(radioModel.TryGetDisplayOptions(out var displayOptions) && displayOptions.Alignment().AsEnum() == DisplayOptions.AlignmentOptionsEnum.Vertical)
            {
                code.AddHtmlElement("MudItem", item =>
                {
                    item.AddHtmlElement("MudRadio", selectItem =>
                    {
                        AddBindingAttributes(selectItem, radioModel, htmlElement, valueMapping);
                    });
                });
            }
            else
            {
                code.AddHtmlElement("MudRadio", selectItem =>
                {
                    AddBindingAttributes(selectItem, radioModel, htmlElement, valueMapping);
                });
            }
            
            //code.AddHtmlElement("MudRadio", selectItem =>
            //{
            //    selectItem.AddHtmlElement("MudItem");
            //    var selectItemMapping = _bindingManager.GetMappedEndFor(radioModel, "Value");
            //    selectItem.AddAttribute("T", _componentTemplate.GetTypeName(valueMapping.SourceElement.TypeReference));
            //    // TODO: Use bindings:
            //    selectItem.AddAttribute("Value", _bindingManager.GetBinding(radioModel, "Value", htmlElement)?.ToString())
            //        .WithText(_bindingManager.GetBinding(radioModel, "Text", htmlElement)?.ToString());
            //});
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

    private void AddBindingAttributes(IHtmlElement selectItem, RadioGroupModel radioModel, HtmlElement htmlElement, IElementToElementMappedEnd valueMapping)
    {
        var selectItemMapping = _bindingManager.GetMappedEndFor(radioModel, "Value");
        selectItem.AddAttribute("T", _componentTemplate.GetTypeName(valueMapping.SourceElement.TypeReference));
        // TODO: Use bindings:
        selectItem.AddAttribute("Value", _bindingManager.GetBinding(radioModel, "Value", htmlElement)?.ToString())
            .WithText(_bindingManager.GetBinding(radioModel, "Text", htmlElement)?.ToString());
    }
}
