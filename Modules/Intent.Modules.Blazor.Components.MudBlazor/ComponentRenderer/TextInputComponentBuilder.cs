using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class TextInputComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public TextInputComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("MudTextField ", _componentTemplate.RazorFile);
        htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetElementBinding(textInput)?.ToString())
                        .AddAttributeIfNotEmpty("Label", textInput.GetLabelAddon()?.Label().TrimEnd(':'));
        parentNode.AddChildNode(htmlElement);
    }
}