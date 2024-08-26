using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var button = new ButtonModel(component);
        var htmlElement = new HtmlElement("button", _componentTemplate.RazorFile)
            .AddAttribute("type", "button")
            .WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        ;
        var onClickMapping = _bindingManager.GetMappedEndFor(button, "On Click");
        if (onClickMapping != null)
        {
            if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            {
                var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
                htmlElement.AddAttribute("@onclick", $"() => {_bindingManager.GetBinding(onClickMapping)}()");
            }
            else
            {
                htmlElement.AddAttribute("@onclick", $"{_bindingManager.GetEventEmitterBinding(onClickMapping)}");
            }
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        parentNode.AddChildNode(htmlElement);
    }
}