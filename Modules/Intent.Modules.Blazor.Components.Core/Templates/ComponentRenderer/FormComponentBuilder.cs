using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class FormComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;
    private readonly BindingManager _bindingManager;

    public FormComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var formModel = new FormModel(component);
        var codeBlock = new RazorCodeDirective(new CSharpStatement($"if ({formModel.GetContent()?.Model().Trim('{', '}')} is not null)"), _template.BlazorFile);
        var htmlElement = new HtmlElement("EditForm", _template.BlazorFile);
        
        codeBlock.AddHtmlElement(htmlElement);
        htmlElement.AddAttributeIfNotEmpty("Model", _bindingManager.GetStereotypePropertyBinding(formModel, "Model"));
        htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Valid Submit")}");
        htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Invalid Submit")}");
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        node.AddNode(codeBlock);

    }
}