using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
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
        if (component.GetParentPath().All(x => !x.IsLayoutSiderModel()))
        {
            IHtmlElement htmlElement = new HtmlElement("MudMenu", _componentTemplate.RazorFile);
            htmlElement.AddAttributeIfNotEmpty("Icon", navigationModel.HasIcon() ? $"@Icons.Material.{navigationModel.GetIcon().Variant().Name}.{navigationModel.GetIcon().IconValue().Name}" : null)
                .AddAttributeIfNotEmpty("IconColor", navigationModel.GetIcon()?.IconColor() != null ? $"Color.{navigationModel.GetIcon()?.IconColor().Name}" : null);
            foreach (var menuItemModel in navigationModel.MenuItems)
            {
                htmlElement.AddHtmlElement("MudMenuItem", navLink =>
                {
                    var mappingEnds = _bindingManager.GetMappedEndsFor(menuItemModel, "Link To");
                    navLink.AddAttributeIfNotEmpty("Href", _bindingManager.GetHrefRoute(mappingEnds));

                    navLink.AddAttributeIfNotEmpty("Icon", menuItemModel.HasIcon() ? $"@Icons.Material.{menuItemModel.GetIcon().Variant().Name}.{menuItemModel.GetIcon().IconValue().Name}" : null)
                        .AddAttributeIfNotEmpty("IconColor", menuItemModel.GetIcon()?.IconColor() != null ? $"Color.{menuItemModel.GetIcon()?.IconColor().Name}" : null);
                    if (!menuItemModel.InternalElement.ChildElements.Any())
                    {
                        navLink.WithText(!string.IsNullOrWhiteSpace(menuItemModel.Value) ? menuItemModel.Value : menuItemModel.Name);
                    }

                    foreach (var innerChild in menuItemModel.InternalElement.ChildElements)
                    {
                        _componentResolver.BuildComponent(innerChild, navLink);
                    }
                });
            }

            parentNode.AddChildNode(htmlElement);

            return [htmlElement];
        }

        var navMenu = new HtmlElement("MudNavMenu", _componentTemplate.RazorFile);
        if (navigationModel.MenuItems.Any())
        {
            foreach (var menuItemModel in navigationModel.MenuItems)
            {
                navMenu.AddHtmlElement("MudNavLink", navLink =>
                {
                    navLink.AddAttributeIfNotEmpty("Icon", menuItemModel.HasIcon() ? $"@Icons.Material.{menuItemModel.GetIcon().Variant().Name}.{menuItemModel.GetIcon().IconValue().Name}" : null)
                        .AddAttributeIfNotEmpty("IconColor", menuItemModel.GetIcon()?.IconColor() != null ? $"Color.{menuItemModel.GetIcon()?.IconColor().Name}" : null);
                    if (menuItemModel.NavigationItems.Count == 0)
                    {
                        foreach (var child in menuItemModel.InternalElement.ChildElements)
                        {
                            _componentResolver.BuildComponent(child, navLink);
                        }

                        if (!menuItemModel.InternalElement.ChildElements.Any())
                        {
                            navLink.WithText(!string.IsNullOrWhiteSpace(menuItemModel.Value) ? menuItemModel.Value : menuItemModel.Name);
                        }
                        var mappingEnds = _bindingManager.GetMappedEndsFor(menuItemModel, "Link To");
                        navLink.AddAttributeIfNotEmpty("Href", _bindingManager.GetHrefRoute(mappingEnds));
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
        parentNode.AddChildNode(navMenu);
        return [navMenu];
    }
}