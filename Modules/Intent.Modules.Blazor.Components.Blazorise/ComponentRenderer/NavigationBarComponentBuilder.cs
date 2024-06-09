using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

public class NavigationBarComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public NavigationBarComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var navigationModel = new NavigationMenuModel(component);
        var htmlElement = new HtmlElement("Bar", _componentTemplate.RazorFile);
        htmlElement.AddAttribute("Breakpoint", "Breakpoint.Desktop");

        if (parentNode is HtmlElement htmlParent && htmlParent.Name is "LayoutHeader")
        {
            htmlElement.AddAttribute("ThemeContrast", "ThemeContrast.Light");
            htmlElement.AddAttribute("Background", "Background.Light");
        }

        //if (navigationModel.BrandLogo != null)
        //{
        //    htmlElement.AddHtmlElement("BarBrand", barBrand =>
        //    {
        //        foreach (var child in navigationModel.BrandLogo.InternalElement.ChildElements)
        //        {
        //            _componentResolver.ResolveFor(child).BuildComponent(child, barBrand);
        //        }

        //        barBrand.WithText(navigationModel.BrandLogo.Value);
        //    });
        //}

        if (navigationModel.MenuItems.Any())
        {
            htmlElement.AddHtmlElement("BarMenu", barMenu =>
            {
                barMenu.AddHtmlElement("BarStart", barStart =>
                {
                    foreach (var navigationItemModel in navigationModel.MenuItems)
                    {
                        barStart.AddHtmlElement("BarItem", barItem =>
                        {
                            if (!navigationItemModel.NavigationItems.Any())
                            {
                                barItem.AddHtmlElement("BarLink", barLink =>
                                {
                                    foreach (var child in navigationItemModel.InternalElement.ChildElements)
                                    {
                                        _componentResolver.ResolveFor(child).BuildComponent(child, barLink);
                                    }
                                    barLink.WithText(!string.IsNullOrWhiteSpace(navigationItemModel.Value) ? navigationItemModel.Value : navigationItemModel.Name);
                                    if (navigationItemModel.TryGetNavigationLink(out var navigationLink))
                                    {
                                        var pageRoute = navigationLink.NavigateTo()?.AsNavigationTargetEndModel().Element.AsComponentModel()?.GetPage()?.Route();
                                        barLink.AddAttribute("To", pageRoute);
                                    }
                                    else
                                    {
                                        var mappingEnd = _bindingManager.GetMappedEndFor(navigationItemModel, "Link To");
                                        if (mappingEnd != null)
                                        {
                                            barLink.AddAttribute("To", mappingEnd.SourcePath.Last().Element.AsNavigationTargetEndModel().TypeReference.Element.AsComponentModel().GetPage().Route());
                                        }
                                    }
                                });
                            }
                            else
                            {
                                barItem.AddHtmlElement("BarDropdown", barDropdown =>
                                {
                                    barDropdown.AddHtmlElement("BarDropdownToggle", barDropdownToggle => { barDropdownToggle.WithText(navigationItemModel.Value ?? navigationItemModel.Name); });
                                    barDropdown.AddHtmlElement("BarDropdownMenu", barDropdownMenu =>
                                    {
                                        foreach (var dropdownItemModel in navigationModel.MenuItems)
                                        {
                                            barDropdownMenu.AddHtmlElement("BarDropdownItem", barDropdownItem =>
                                            {
                                                barDropdownItem.WithText(dropdownItemModel.Value ?? dropdownItemModel.Name);
                                                if (dropdownItemModel.TryGetNavigationLink(out var navigationLink))
                                                {
                                                    var pageRoute = navigationLink.NavigateTo()?.AsNavigationTargetEndModel().Element.AsComponentModel()?.GetPage()?.Route();
                                                    barDropdownItem.AddAttribute("To", pageRoute);
                                                }
                                            });
                                        }
                                    });
                                });
                            }
                        });
                    }
                });
            });
        }
        parentNode.AddChildNode(htmlElement);
    }
}