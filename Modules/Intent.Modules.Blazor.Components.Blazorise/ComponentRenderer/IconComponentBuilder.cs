using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var iconModel = new IconModel(component);
        var htmlElement = new HtmlElement("Icon", _componentTemplate.RazorFile)
            .AddAttribute("Name", $"IconName.{iconModel.GetIconAppearance().Name().Source.ToPascalCase()}");
        ;
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }

        var onClickMapping = _bindingManager.GetMappedEndFor(iconModel, "On Click");
        if (onClickMapping != null)
        {
            if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            {
                var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode)}");
            }
            else
            {
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode)}");
            }
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        parentNode.AddChildNode(htmlElement);
    }
}