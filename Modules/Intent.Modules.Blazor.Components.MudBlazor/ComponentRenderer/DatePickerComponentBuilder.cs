using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class DatePickerComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public DatePickerComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var datePicker = new DatePickerModel(component);
        var htmlElement = new HtmlElement("MudDatePicker", _componentTemplate.RazorFile);
        var valueMapping = _bindingManager.GetMappedEndFor(datePicker);
        var valueBinding = _bindingManager.GetBinding(valueMapping, parentNode)?.ToString();
        htmlElement.AddAttributeIfNotEmpty("@bind-Date", valueBinding)
            .AddAttributeIfNotEmpty("Label", datePicker.GetLabelAddon()?.Label().TrimEnd(':') ?? datePicker.Name);
        parentNode.AddChildNode(htmlElement);
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