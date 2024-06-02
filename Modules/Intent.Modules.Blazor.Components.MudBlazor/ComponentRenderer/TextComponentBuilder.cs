using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class TextComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public TextComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var textInput = new TextModel(component);
        var textValue = _bindingManager.GetElementBinding(textInput)?.ToString() ?? (string.IsNullOrWhiteSpace(textInput.Value) ? textInput.Name : textInput.Value);
        if (textValue.StartsWith("#"))
        {
            var size = 0;
            while (textValue[0] == '#')
            {
                textValue = textValue.Substring(1);
                size++;
            }
            var htmlElement = new HtmlElement("MudText", _componentTemplate.RazorFile)
                .WithText(textValue)
                .AddAttribute("Typo", $"Typo.h{size}");
            parentNode.AddChildNode(htmlElement);
        }
        else
        {
            var htmlElement = new HtmlElement("MudText", _componentTemplate.RazorFile)
                .WithText(textValue);
            parentNode.AddChildNode(htmlElement);
        }
    }
}