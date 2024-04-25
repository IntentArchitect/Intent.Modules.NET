using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using System.Linq;
using Intent.Modules.Blazor.Api;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class LayoutComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public LayoutComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var layoutModel = new LayoutModel(component);
        var layoutHtml = new HtmlElement("Layout", _componentTemplate.RazorFile);
        if (layoutModel.Header != null)
        {
            layoutHtml.AddHtmlElement("LayoutHeader", layoutHeader =>
            {
                layoutHeader.AddAttribute("Fixed");
                foreach (var child in layoutModel.Header.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, layoutHeader);
                }
            });
        }
        //layoutHtml.AddHtmlElement("Layout", layoutHtml =>
        //{
        if (layoutModel.Sider != null)
        {
            layoutHtml.AddAttribute("Sider");
            layoutHtml.AddHtmlElement("LayoutSider", layoutSider =>
            {
                if (layoutModel.Sider.GetContent().Body())
                {
                    layoutSider.AddHtmlElement("LayoutSiderContent", layoutSiderContent =>
                    {
                        foreach (var child in layoutModel.Sider.InternalElement.ChildElements)
                        {
                            _componentResolver.ResolveFor(child).BuildComponent(child, layoutSiderContent);
                        }
                    });
                }
            });
        }
        layoutHtml.AddHtmlElement("LayoutContent", layoutContent =>
        {
            if (layoutModel.Sider?.GetContent().Body() == true)
            {
                foreach (var child in layoutModel.Body.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, layoutContent);
                }
            }
            layoutContent.AddAttribute("Padding", "Padding.Is4.OnX.Is4.FromTop");
            layoutContent.WithText("@Body");
        });
        //});

        node.AddChildNode(layoutHtml);
    }
}