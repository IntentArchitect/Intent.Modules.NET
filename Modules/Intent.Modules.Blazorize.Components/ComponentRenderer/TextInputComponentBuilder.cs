using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class TextInputComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public TextInputComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
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
                    textEdit.AddAttribute("@bind-Text", textInput.Value.Trim('{', '}'))
                        .AddAttributeIfNotEmpty("Placeholder", textInput.GetLabelAddon()?.Label().TrimEnd(':'));
                });
            });
        node.AddChildNode(htmlElement);
    }
}
