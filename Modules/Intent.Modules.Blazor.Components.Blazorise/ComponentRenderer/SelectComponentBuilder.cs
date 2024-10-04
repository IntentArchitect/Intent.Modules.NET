using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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
        var htmlElement = new HtmlElement("Validation", _componentTemplate.RazorFile)
            .AddHtmlElement("Field", field =>
            {
                field.AddHtmlElement("FieldLabel", label =>
                {
                    label.WithText(selectModel.GetLabelAddon()?.Label());
                });
                field.AddHtmlElement("Select", select =>
                {
                    select.AddAttributeIfNotEmpty("@bind-SelectedValue", _bindingManager.GetElementBinding(selectModel)?.ToString())
                        .AddAttributeIfNotEmpty("Placeholder", selectModel.GetLabelAddon()?.Label().TrimEnd(':'));
                    var mappedEnd = _bindingManager.GetMappedEndFor(selectModel, "Options");
                    if (mappedEnd == null)
                    {
                        return;
                    }
                    select.AddCodeBlock($"@foreach (var option in {_bindingManager.GetBinding(selectModel, "Options")})", code =>
                    {
                        select.AddMappingReplacement(mappedEnd.SourceElement, "option");
                        code.AddHtmlElement("SelectItem", selectItem =>
                        {
                            // TODO: Use bindings:
                            selectItem.AddAttribute("Value", _bindingManager.GetBinding(selectModel, "Value", select)?.ToString())
                                .WithText(_bindingManager.GetBinding(selectModel, "Text", select)?.ToString());
                        });
                    });
                });
            });
        parentNode.AddChildNode(htmlElement);
    }
}
