using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class ButtonRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public ButtonRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var button = new ButtonModel(component);
        var htmlElement = new HtmlElement("button", _template.BlazorFile)
            .AddAttribute("type", "submit")
            .WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        ;
        var onClickMapping = _template.GetMappedEndFor(button, "On Click");
        if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
        {
            var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
            htmlElement.AddAttribute("@onclick", $"() => {_template.GetBinding(onClickMapping)}()");
        }
        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        node.AddNode(htmlElement);
    }
}