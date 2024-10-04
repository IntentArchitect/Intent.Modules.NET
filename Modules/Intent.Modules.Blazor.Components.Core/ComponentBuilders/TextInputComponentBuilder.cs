using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

public class TextInputComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;

    public TextInputComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("label", _componentTemplate.RazorFile)
            .WithText(textInput.GetLabelAddon()?.Label())
            .AddHtmlElement("InputText", inputText =>
            {
                inputText.AddAttribute("@bind-Value", textInput.Value.Trim('{', '}'));
            });
        parentNode.AddChildNode(htmlElement);
    }
}
