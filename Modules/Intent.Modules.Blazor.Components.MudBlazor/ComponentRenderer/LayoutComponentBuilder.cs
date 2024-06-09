using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Razor;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

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
        parentNode.AddChildNode(new HtmlElement("MudThemingProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new HtmlElement("MudPopoverProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new HtmlElement("MudDialogProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new HtmlElement("MudSnackbarProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new EmptyLine(_componentTemplate.RazorFile));

        var layoutHtml = new HtmlElement("MudLayout", _componentTemplate.RazorFile);
        parentNode.AddChildNode(layoutHtml);
        var code = _componentTemplate.GetCodeBlock();

        if (layoutModel.Header != null)
        {
            layoutHtml.AddHtmlElement("MudAppBar", appBar =>
            {
                appBar.AddAttribute("Elevation", "1");
                if (layoutModel.Sider != null)
                {
                    appBar.AddHtmlElement("MudIconButton", drawerToggle =>
                    {
                        drawerToggle.AddAttribute("Icon", "@Icons.Material.Filled.Menu");
                        drawerToggle.AddAttribute("Color", "Color.Inherit");
                        drawerToggle.AddAttribute("Edge", "Edge.Start");

                        code.AddField("bool", "_drawerOpen", field => field.WithAssignment(new CSharpStatement("true")));
                        code.AddMethod("void", "DrawerToggle", method =>
                        {
                            method.AddStatement("_drawerOpen = !_drawerOpen;");
                        });

                        drawerToggle.AddAttribute("OnClick", "@((e) => DrawerToggle())");
                    });
                }
                foreach (var child in layoutModel.Header.InternalElement.ChildElements)
                {
                    if (child.IsNavigationMenuModel())
                    {
                        var navMenuModel = child.AsNavigationMenuModel();
                        foreach (var navigationItemModel in navMenuModel.MenuItems)
                        {
                            appBar.AddHtmlElement("MudButton", navLink =>
                            {
                                var mappingEnd = _bindingManager.GetMappedEndFor(navigationItemModel, "Link To");
                                navLink.AddAttribute("Href", mappingEnd != null
                                    ? mappingEnd.SourcePath.Last().Element.AsNavigationTargetEndModel().TypeReference.Element.AsComponentModel().GetPage().Route()
                                    : "/");
                                navLink.AddAttribute("Color", "Color.Inherit");
                                navLink.AddAttribute("Class", "mx-1 px-3");
                                if (!navigationItemModel.InternalElement.ChildElements.Any())
                                {
                                    navLink.WithText(!string.IsNullOrWhiteSpace(navigationItemModel.Value) ? navigationItemModel.Value : navigationItemModel.Name);
                                }

                                foreach (var innerChild in navigationItemModel.InternalElement.ChildElements)
                                {
                                    _componentResolver.ResolveFor(innerChild).BuildComponent(innerChild, navLink);
                                }
                            });
                        }
                        continue;
                    }
                    _componentResolver.ResolveFor(child).BuildComponent(child, appBar);
                }
                appBar.AddHtmlElement("MudSpacer");
                appBar.AddHtmlElement("MudIconButton", icon =>
                {
                    icon.AddAttribute("Icon", "@Icons.Material.Filled.MoreVert");
                    icon.AddAttribute("Color", "Color.Inherit");
                    icon.AddAttribute("Edge", "Edge.End");
                });

            });
        }
        //layoutHtml.AddHtmlElement("Layout", layoutHtml =>
        //{
        if (layoutModel.Sider != null)
        {
            layoutHtml.AddHtmlElement("MudDrawer", mudDrawer =>
            {
                if (layoutModel.Header != null)
                {
                    mudDrawer.AddAttribute("@bind-Open", "_drawerOpen");
                }

                mudDrawer.AddAttribute("ClipMode", "DrawerClipMode.Always");
                mudDrawer.AddAttribute("Elevation", "2");

                foreach (var child in layoutModel.Sider.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, mudDrawer);
                }
            });
        }
        layoutHtml.AddHtmlElement("MudMainContent", layoutContent =>
        {
            layoutContent.AddAttribute("Class", "mt-16 pa-4");

            foreach (var child in layoutModel.Body.InternalElement.ChildElements)
            {
                _componentResolver.ResolveFor(child).BuildComponent(child, layoutContent);
            }
            layoutContent.WithText("@Body");
        });
        //});

    }
}