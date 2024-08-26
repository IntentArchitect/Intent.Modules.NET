using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var navigationModel = new NavigationMenuModel(component);
        var htmlElement = new HtmlElement("MudNavMenu", _componentTemplate.RazorFile);

        if (navigationModel.MenuItems.Any())
        {
            foreach (var navigationItemModel in navigationModel.MenuItems)
            {
                htmlElement.AddHtmlElement("MudNavLink", navLink =>
                {
                    if (navigationItemModel.NavigationItems.Count == 0)
                    {
                        foreach (var child in navigationItemModel.InternalElement.ChildElements)
                        {
                            _componentResolver.BuildComponent(child, navLink);
                        }

                        if (!navigationItemModel.InternalElement.ChildElements.Any())
                        {
                            navLink.WithText(!string.IsNullOrWhiteSpace(navigationItemModel.Value) ? navigationItemModel.Value : navigationItemModel.Name);
                        }
                        if (navigationItemModel.TryGetNavigationLink(out var navigationLink))
                        {
                            var pageRoute = navigationLink.NavigateTo()?.AsNavigationTargetEndModel().Element.AsComponentModel()?.GetPage()?.Route();
                            navLink.AddAttribute("Href", pageRoute);
                        }
                        else
                        {
                            var mappingEnd = _bindingManager.GetMappedEndFor(navigationItemModel, "Link To");
                            if (mappingEnd != null)
                            {
                                navLink.AddAttribute("Href", mappingEnd.SourcePath.Last().Element.AsNavigationTargetEndModel().TypeReference.Element.AsComponentModel().GetPage().Route());
                            }
                        }
                    }
                    //else
                    //{
                    //    navLink.AddHtmlElement("BarDropdown", barDropdown =>
                    //    {
                    //        barDropdown.AddHtmlElement("BarDropdownToggle", barDropdownToggle => { barDropdownToggle.WithText(navigationItemModel.Value ?? navigationItemModel.Name); });
                    //        barDropdown.AddHtmlElement("BarDropdownMenu", barDropdownMenu =>
                    //        {
                    //            foreach (var dropdownItemModel in navigationModel.NavigationItems)
                    //            {
                    //                barDropdownMenu.AddHtmlElement("BarDropdownItem", barDropdownItem =>
                    //                {
                    //                    barDropdownItem.WithText(dropdownItemModel.Value ?? dropdownItemModel.Name);
                    //                    if (dropdownItemModel.TryGetNavigationLink(out var navigationLink))
                    //                    {
                    //                        var pageRoute = navigationLink.NavigateTo()?.AsNavigationTargetEndModel().Element.AsComponentModel()?.GetPage()?.Route();
                    //                        barDropdownItem.AddAttribute("To", pageRoute);
                    //                    }
                    //                });
                    //            }
                    //        });
                    //    });
                    //}
                });
            }
        }
        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}