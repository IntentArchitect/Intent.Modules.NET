using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var layoutModel = new LayoutModel(component);
        IHtmlElement layoutHtml = new HtmlElement("Layout", _componentTemplate.RazorFile);
        parentNode.AddChildNode(layoutHtml);

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
            layoutHtml.AddHtmlElement("Layout", layout => layoutHtml = layout);
            layoutHtml.AddAttribute("Sider");
            layoutHtml.AddHtmlElement("LayoutSider", layoutSider =>
            {
                layoutSider.AddHtmlElement("LayoutSiderContent", layoutSiderContent =>
                {
                    foreach (var child in layoutModel.Sider.InternalElement.ChildElements)
                    {
                        _componentResolver.ResolveFor(child).BuildComponent(child, layoutSiderContent);
                    }
                });
            });
        }
        layoutHtml.AddHtmlElement("LayoutContent", layoutContent =>
        {
            foreach (var child in layoutModel.Body.InternalElement.ChildElements)
            {
                _componentResolver.ResolveFor(child).BuildComponent(child, layoutContent);
            }
            layoutContent.AddAttribute("Padding", "Padding.Is4.OnX.Is4.FromTop");
            layoutContent.WithText("@Body");
        });
        //});

    }
}