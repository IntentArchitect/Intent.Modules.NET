using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.MudBlazor.Templates.AppNav;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

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

                    var onClickMapping = _bindingManager.GetMappedEndFor(menuItemModel, "On Click");
                    if (onClickMapping != null)
                    {
                        navLink.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode)!.ToLambda()}");
                    }

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

        // Sider nav: render via the shared presentational NavLinks component. The nav items are
        // resolved here (this builder holds the BindingManager that turns each "Link To" mapping into
        // an href) and pushed into the generated AppNav.Items list — a single source shared with the
        // static-SSR ManageLayout drawer.
        PopulateAppNavItems(component, navigationModel);

        var navLinks = new HtmlElement("NavLinks", _componentTemplate.RazorFile);
        navLinks.AddAttribute("Items", "AppNav.Items");
        parentNode.AddChildNode(navLinks);
        return [navLinks];
    }

    private void PopulateAppNavItems(IElement navComponent, NavigationMenuModel navigationModel)
    {
        var layoutElement = navComponent.GetParentPath()
            .FirstOrDefault(x => x.SpecializationTypeId == LayoutModel.SpecializationTypeId);
        if (layoutElement == null)
        {
            return;
        }

        var appNav = _componentTemplate.ExecutionContext
            .FindTemplateInstance<AppNavTemplate>(AppNavTemplate.TemplateId, layoutElement);
        if (appNav == null)
        {
            return;
        }

        // Menu items which do not have a "Link-To" property set are excluded form the list
        var menuEntries = navigationModel.MenuItems.Select(BuildNavItemExpression).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        // Items which just have a [navigate] association
        var entries = AddNavigationItems(menuEntries, layoutElement, navigationModel);

        var initializer = entries.Count == 0
            ? "[]"
            : $"[\n            {string.Join(",\n            ", entries)}\n        ]";

        appNav.SetNavItems(initializer);
    }

    private string BuildNavItemExpression(MenuItemModel menuItemModel)
    {
        var label = !string.IsNullOrWhiteSpace(menuItemModel.Value) ? menuItemModel.Value : menuItemModel.Name;
        var mappendEnds = _bindingManager.GetMappedEndsFor(menuItemModel, "Link To");

        if (mappendEnds.Count == 0 || mappendEnds is null)
        {
            return string.Empty;
        }

        var href = ToCSharpHref(_bindingManager.GetHrefRoute(mappendEnds));

        var icon = menuItemModel.HasIcon()
            ? $"Icons.Material.{menuItemModel.GetIcon().Variant().Name}.{menuItemModel.GetIcon().IconValue().Name}"
            : null;

        return icon != null
            ? $"new(\"{label}\", {href}, {icon})"
            : $"new(\"{label}\", {href})";
    }

    private static string ToCSharpHref(string href)
    {
        if (string.IsNullOrEmpty(href))
        {
            return "\"\"";
        }

        // GetHrefRoute returns a razor expression `@($"...")` for parameterised routes; unwrap it to a
        // plain C# expression (`$"..."`) for the AppNav.cs context. A plain route becomes a string literal.
        if (href.StartsWith("@"))
        {
            var expr = href.Substring(1);
            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                expr = expr.Substring(1, expr.Length - 2);
            }

            return expr;
        }

        return $"\"{href}\"";
    }

    private List<string> AddNavigationItems(
        List<string> menuEntries,
        IElement layoutElement,
        NavigationMenuModel navigationModel)
    {
        // Build route → pre-built expression map from explicit menu items
        var menuByRoute = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in navigationModel.MenuItems)
        {
            var href = _bindingManager.GetHrefRoute(_bindingManager.GetMappedEndsFor(item, "Link To"));
            if (string.IsNullOrWhiteSpace(href) || href!.StartsWith('@'))
                continue;
            var expression = BuildNavItemExpression(item);
            if (!string.IsNullOrWhiteSpace(expression))
                menuByRoute[NormalizeRoute(href)] = expression;
        }

        var result = new List<string>();
        var usedExpressions = new HashSet<string>();

        // Iterate layout navigation associations in their modelled order
        foreach (var assocEnd in layoutElement.AssociatedElements
            .Where(e => e.IsNavigationEndModel() && e.IsNavigable))
        {
            var navEnd = assocEnd.AsNavigationEndModel();
            var targetComponent = new ComponentModel((IElement)navEnd.TypeReference.Element);

            if (!targetComponent.HasPage())
                continue;

            var route = targetComponent.GetPage().Route();

            if (menuByRoute.TryGetValue(NormalizeRoute(route), out var existingEntry))
            {
                result.Add(existingEntry);
                usedExpressions.Add(existingEntry);
            }
            else
            {
                var fullRoute = route.StartsWith('/') ? route : "/" + route;
                var label = ToHumanReadableLabel(targetComponent.Name);
                result.Add($"new(\"{label}\", {ToCSharpHref(fullRoute)})");
            }
        }

        // Append any menu items not matched to a layout navigation association
        foreach (var entry in menuEntries.Where(e => !usedExpressions.Contains(e)))
            result.Add(entry);

        return result;
    }

    private static string NormalizeRoute(string route) =>
        route.TrimStart('/').ToLowerInvariant();

    private static string ToHumanReadableLabel(string name)
    {
        if (name.EndsWith("Page"))
            name = name[..^4];
        return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
    }

    private void AddMenuItem(IHtmlElement parent, MenuItemModel menuItemModel)
    {
        bool isGroup = menuItemModel.InternalElement.ChildElements.Any(c => c.IsMenuItemModel());
        var parentElement = parent;

        if (menuItemModel.InternalElement is IElement element && element.HasStereotype(Intent.Modelers.UI.Core.Api.ComponentModelStereotypeExtensions.Secured.DefinitionId))
        {
            var authComponent = parentElement.AuthorizeComponent(element, _componentTemplate);
            parentElement = authComponent;
        }

        AddMenuItem(menuItemModel, isGroup, parentElement);
    }

    private void AddMenuItem(MenuItemModel menuItemModel, bool isGroup, IHtmlElement parentElement)
    {
        parentElement.AddHtmlElement(isGroup ? "MudNavGroup" : "MudNavLink", navLink =>
        {
            navLink.AddAttributeIfNotEmpty("Icon", menuItemModel.HasIcon() ? $"@Icons.Material.{menuItemModel.GetIcon().Variant().Name}.{menuItemModel.GetIcon().IconValue().Name}" : null)
                .AddAttributeIfNotEmpty("IconColor", menuItemModel.GetIcon()?.IconColor() != null ? $"Color.{menuItemModel.GetIcon()?.IconColor().Name}" : null);
            if (!isGroup)
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
            else
            {
                navLink.SetAttribute("Title", !string.IsNullOrWhiteSpace(menuItemModel.Value) ? menuItemModel.Value : menuItemModel.Name);
                foreach (var childMenuItem in menuItemModel.NavigationItems)
                {
                    AddMenuItem(navLink, childMenuItem);
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