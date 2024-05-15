using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

public class ButtonComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public ButtonComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var button = new ButtonModel(component); 
        var htmlElement = new HtmlElement("Button", _componentTemplate.RazorFile)
            .AddAttribute("Type", button.GetInteraction().Type().IsSubmit() ? "ButtonType.Submit" : "ButtonType.Button")
            .AddAttribute("Color", button.GetInteraction().Type().IsSubmit() ? "Color.Primary" : "Color.Secondary")
            .WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        ;
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }

        var onClickMapping = _bindingManager.GetMappedEndFor(button, "On Click");
        if (onClickMapping != null)
        {
            if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            {
                var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");
            }
            else
            {
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");
            }
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        parentNode.AddChildNode(htmlElement);
    }
}