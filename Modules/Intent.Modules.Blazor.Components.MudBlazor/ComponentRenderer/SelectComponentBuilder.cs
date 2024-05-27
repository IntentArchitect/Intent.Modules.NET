using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.TypeResolution;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var selectModel = new SelectModel(component);
        var htmlElement = new HtmlElement("MudSelect", _componentTemplate.RazorFile);
        var modelMapping = _bindingManager.GetMappedEndFor(selectModel);
        htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetBinding(modelMapping)?.ToString());
        htmlElement.AddAttribute("Label", selectModel.TryGetLabelAddon(out var label) ? label.Label() : selectModel.Name);

        var mappedEnd = _bindingManager.GetMappedEndFor(selectModel, "Options");
        if (mappedEnd == null)
        {
            return;
        }
        htmlElement.AddCodeBlock($"foreach (var item in {_bindingManager.GetBinding(selectModel, "Options")})", code =>
        {
            htmlElement.AddMappingReplacement(mappedEnd.SourceElement, "item");
            code.AddHtmlElement("MudSelectItem", selectItem =>
            {
                var selectItemMapping = _bindingManager.GetMappedEndFor(selectModel, "Value");
                selectItem.AddAttributeIfNotEmpty("T", !selectItemMapping.SourceElement.TypeReference.Equals(modelMapping.SourceElement.TypeReference)
                    ? _componentTemplate.GetTypeName(modelMapping.SourceElement.TypeReference)
                    : null);
                // TODO: Use bindings:
                selectItem.AddAttribute("Value", _bindingManager.GetBinding(selectModel, "Value", htmlElement)?.ToString())
                    .WithText(_bindingManager.GetBinding(selectModel, "Text", htmlElement)?.ToString());
            });
        });
        parentNode.AddChildNode(htmlElement);
    }
}
