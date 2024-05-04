using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var formModel = new FormModel(component);
        var codeBlock = new RazorCodeDirective(new CSharpStatement($"if ({formModel.GetContent()?.Model().Trim('{', '}')} is not null)"), _componentTemplate.RazorFile);
        var htmlElement = new HtmlElement("EditForm", _componentTemplate.RazorFile);
        
        codeBlock.AddHtmlElement(htmlElement);
        htmlElement.AddAttributeIfNotEmpty("Model", _bindingManager.GetStereotypePropertyBinding(formModel, "Model"));
        htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Valid Submit")}");
        htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Invalid Submit")}");
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        parentNode.AddChildNode(codeBlock);

    }
}