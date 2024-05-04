using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

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
        var htmlElement = new HtmlElement("Validation", _componentTemplate.RazorFile)
            .AddHtmlElement("Field", field =>
            {
                field.AddHtmlElement("FieldLabel", label =>
                {
                    label.WithText(textInput.GetLabelAddon()?.Label());
                });
                field.AddHtmlElement("TextEdit", textEdit =>
                {
                    textEdit.AddAttribute("@bind-Text", _bindingManager.GetElementBinding(textInput))
                        .AddAttributeIfNotEmpty("Placeholder", textInput.GetLabelAddon()?.Label().TrimEnd(':'));
                });
            });
        parentNode.AddChildNode(htmlElement);
    }
}
