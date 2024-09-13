using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.Interceptors;

public class MudBlazorLayoutInterceptor(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template) : IRazorComponentInterceptor
{
    public void Handle(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node)
    {
        if (component.HasStereotype("Layout") && razorNodes.Any())
        {
            var index = node.ChildNodes.IndexOf(razorNodes.First());

            HtmlElement grid = new HtmlElement("MudGrid", template.RazorFile);
            if (index > 0 && node.ChildNodes[index - 1] is HtmlElement { Name: "MudGrid" } previousHtmlElement)
            {
                grid = previousHtmlElement;
            }
            else
            {
                node.InsertChildNode(index, grid);
            }

            grid.AddHtmlElement("MudItem", mudItem =>
            {
                mudItem.AddAttribute("xs", component.GetStereotypeProperty("Layout", "xs", "12"));
                mudItem.AddAttributeIfNotEmpty("sm", component.GetStereotypeProperty("Layout", "sm", ""));
                mudItem.AddAttributeIfNotEmpty("md", component.GetStereotypeProperty("Layout", "md", ""));
                mudItem.AddAttributeIfNotEmpty("lg", component.GetStereotypeProperty("Layout", "lg", ""));
                mudItem.AddAttributeIfNotEmpty("xl", component.GetStereotypeProperty("Layout", "xl", ""));
                mudItem.AddAttributeIfNotEmpty("xxl", component.GetStereotypeProperty("Layout", "xxl", ""));
                foreach (var razorFileNode in razorNodes)
                {
                    razorFileNode.Remove();
                    mudItem.AddChildNode(razorFileNode);
                }
            });

        }
    }
}