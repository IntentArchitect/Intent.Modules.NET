using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class TextInputComponent : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TextInputComponent(IRazorComponentBuilderResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("Validation", _template.BlazorFile)
            .AddHtmlElement("Field", field =>
            {
                field.AddHtmlElement("FieldLabel", label =>
                {
                    label.WithText(textInput.GetLabelAddon()?.Label());
                });
                field.AddHtmlElement("TextEdit", textEdit =>
                {
                    textEdit.AddAttribute("@bind-Text", textInput.Value.Trim('{', '}'));
                });
            });
        node.AddNode(htmlElement);
    }
}
