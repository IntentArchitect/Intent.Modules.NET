using System.Collections.Generic;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class IconComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    static IconComponentBuilder()
    {
        DefaultRazorComponentBuilderProvider.Register(IconModel.SpecializationTypeId, (provider, componentTemplate) => new IconComponentBuilder(provider, componentTemplate));
    }

    public IconComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new IconModel(component);
        var htmlElement = new HtmlElement("MudIcon", _componentTemplate.RazorFile)
            .AddAttributeIfNotEmpty("Icon", !model.HasIcon() ? (model.Value ?? "Icons.Material.Filled.QuestionMark") : null)
            .AddAttributeIfNotEmpty("Icon", model.HasIcon() ? $"@Icons.Material.{model.GetIcon().Variant().Name}.{model.GetIcon().IconValue().Name}" : null)
            .AddAttributeIfNotEmpty("IconColor", model.GetIcon()?.IconColor() != null ? $"Color.{model.GetIcon()?.IconColor().Name}" : null)
        ;
        foreach (var child in component.ChildElements)
        {
            _componentResolver.BuildComponent(child, htmlElement);
        }

        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}