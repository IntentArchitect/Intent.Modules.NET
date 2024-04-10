using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class ButtonComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;
    private BindingManager _bindingManager;

    public ButtonComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var button = new ButtonModel(component);
        var htmlElement = new HtmlElement("Button", _template.BlazorFile)
            .AddAttribute("Type", button.GetInteraction().Type().IsSubmit() ? "ButtonType.Submit" : "ButtonType.Button")
            .AddAttribute("Color", button.GetInteraction().Type().IsSubmit() ? "Color.Primary" : "Color.Secondary")
            .WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        ;
        var onClickMapping = _bindingManager.GetMappedEndFor(button, "On Click");
        if (onClickMapping != null)
        {
            if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            {
                var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
                htmlElement.AddAttribute("Clicked", $"() => {_bindingManager.GetBinding(onClickMapping)}()");
            }
            else
            {
                htmlElement.AddAttribute("Clicked", $"{_bindingManager.GetBinding(onClickMapping)}");
            }
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        node.AddNode(htmlElement);
    }
}