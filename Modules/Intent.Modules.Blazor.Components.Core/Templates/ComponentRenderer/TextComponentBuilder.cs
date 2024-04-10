using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class TextComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;
    private readonly BindingManager _bindingManager;

    public TextComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var textInput = new TextModel(component);
        var valueMapping = _bindingManager.GetMappedEndFor(textInput);
        var htmlElement = new HtmlElement("label", _template.BlazorFile)
            .WithText(valueMapping != null ? _bindingManager.GetCodeDirective(valueMapping) : textInput.Value);
        node.AddNode(htmlElement);
    }
}