using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var textInput = new TextModel(component);
        var textValue = _bindingManager.GetElementBinding(textInput)?.ToString() ?? textInput.Name;
        if (textValue.StartsWith("#"))
        {
            var size = 0;
            while (textValue[0] == '#')
            {
                textValue = textValue.Substring(1);
                size++;
            }

            var htmlElement = new HtmlElement($"h{size}", _componentTemplate.RazorFile)
                .WithText(textValue);
            parentNode.AddChildNode(htmlElement);
        }
        else
        {
            var htmlElement = new HtmlElement("p", _componentTemplate.RazorFile)
                .WithText(textValue);
            parentNode.AddChildNode(htmlElement);
        }

    }
}