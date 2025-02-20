using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class LinkComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public LinkComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new LinkModel(component);
        IHtmlElement htmlElement = new HtmlElement("MudLink", _componentTemplate.RazorFile);

        var mappingEnds = _bindingManager.GetMappedEndsFor(model, "Link To");
        htmlElement.AddAttributeIfNotEmpty("Href", _bindingManager.GetHrefRoute(mappingEnds));

        //htmlElement.AddAttributeIfNotEmpty("Class", string.IsNullOrWhiteSpace(model.GetAppearance().Class()) ? "my-2 mr-2" : model.GetAppearance().Class());

        if (component.ChildElements.Any())
        {
            foreach (var child in component.ChildElements)
            {
                _componentResolver.BuildComponent(child, htmlElement);
            }
        }
        else
        {
            htmlElement.WithText(model.Name);
        }

        var onClickMapping = _bindingManager.GetMappedEndFor(model, "On Click");
        if (onClickMapping != null)
        {
            htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode)!.ToLambda()}");
        }

        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}