using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class CheckboxComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public CheckboxComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new CheckboxModel(component);
        var htmlElement = new HtmlElement("MudCheckBox", _componentTemplate.RazorFile);
        htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetElementBinding(model, parentNode)?.ToString())
            .AddAttributeIfNotEmpty("Label", model.GetLabelAddon()?.Label() ?? model.Name);

        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}

public class SpacerComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public SpacerComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var htmlElement = new HtmlElement("MudSpacer", _componentTemplate.RazorFile);
        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}