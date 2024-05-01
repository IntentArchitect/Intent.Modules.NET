using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var navigationModel = new NavigationBarModel(component);
        var htmlElement = new HtmlElement("Bar", _componentTemplate.RazorFile);
        htmlElement.AddAttribute("Breakpoint", "Breakpoint.Desktop");
        if (navigationModel.NavigationItems.Any())
        {
            htmlElement.AddHtmlElement("BarMenu", barMenu =>
            {
                barMenu.AddHtmlElement("BarStart", barStart =>
                {
                    foreach (var navigationItemModel in navigationModel.NavigationItems)
                    {
                        barStart.AddHtmlElement("BarItem", barItem =>
                        {
                            if (!navigationItemModel.NavigationItems.Any())
                            {
                                barItem.AddHtmlElement("BarLink", barLink =>
                                {
                                    barLink.WithText(navigationItemModel.Value ?? navigationItemModel.Name);
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
                                        foreach (var dropdownItemModel in navigationModel.NavigationItems)
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
        node.AddChildNode(htmlElement);
    }
}