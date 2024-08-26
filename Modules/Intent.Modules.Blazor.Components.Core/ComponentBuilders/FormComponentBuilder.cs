using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

public class FormComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public FormComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var formModel = new FormModel(component);
        var codeBlock = IRazorCodeDirective.Create(new CSharpStatement($"if ({formModel.GetContent()?.Model().Trim('{', '}')} is not null)"), _componentTemplate.RazorFile);
        var htmlElement = new HtmlElement("EditForm", _componentTemplate.RazorFile);

        codeBlock.AddHtmlElement(htmlElement);
        htmlElement.AddAttributeIfNotEmpty("Model", _bindingManager.GetBinding(formModel, "Model")?.ToString());
        htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"{_bindingManager.GetBinding(formModel, "On Valid Submit")?.ToLambda()}");
        htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"{_bindingManager.GetBinding(formModel, "On Invalid Submit")?.ToLambda()}");

        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        parentNode.AddChildNode(codeBlock);

    }
}