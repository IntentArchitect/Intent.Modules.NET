using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var textInput = new TextModel(component);
        var valueMapping = _bindingManager.GetMappedEndFor(textInput);
        var htmlElement = new HtmlElement("label", _componentTemplate.RazorFile)
            .WithText(valueMapping != null ? _bindingManager.GetCodeDirective(valueMapping) : textInput.Value);
        node.AddChildNode(htmlElement);
    }
}