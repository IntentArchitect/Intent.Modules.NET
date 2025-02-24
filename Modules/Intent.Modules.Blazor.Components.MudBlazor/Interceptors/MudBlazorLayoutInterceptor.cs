using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.Interceptors;

public class MudBlazorLayoutInterceptor(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template) : IRazorComponentInterceptor
{
    // In this interceptor we dynamically add in MudGrids as they are required, based on the presence of `Layout` stereotype.
    public void Handle(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node)
    {
        if (RequiresANonGridContainer(component, razorNodes, node))
        {

        }
        else if (!(component.HasStereotype("Layout") && razorNodes.Any()))
        {
            return;
        }

        var elementsToSwapParent = razorNodes;
        if (component.IsContainerModel())
        {
            if (TryGetExistingGrid(node, component, out var childGrid))
            {
                elementsToSwapParent = new List<IRazorFileNode> { childGrid };
            }
        }

        HtmlElement grid;
        if (TryGetExistingGrid(node, component.ParentElement, out var existingGrid))
        {
            grid = existingGrid;
        }
        else
        {
            grid = new HtmlElement("MudGrid", template.RazorFile);
            //This is used to denote which component the grid represents
            grid.AddMetadata("_gridModel", component.ParentElement);

            if (component.IsContainerModel())
            {
                node.AddChildNode(grid);
            }
            else
            {
                var index = node.ChildNodes.IndexOf(razorNodes.First());
                node.InsertChildNode(index, grid);
            }
        }
        AddMudItem(component, grid, elementsToSwapParent);
    }

    private bool RequiresANonGridContainer(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node)
    {
        return component.IsContainerModel() 
            && !component.HasStereotype("Layout") //Container doesn't have Layout
            && !TryGetExistingGrid(node, component, out var childGrid); // Children don't have a layout
    }

    private static bool TryGetExistingGrid(IRazorFileNode node, IElement parent, out HtmlElement grid)
    {
        grid = node.ChildNodes.FirstOrDefault(c => c is HtmlElement { Name: "MudGrid" } && c.HasMetadata("_gridModel") && c.GetMetadata<IElement>("_gridModel") == parent) as HtmlElement;
        return grid != null;
    }

    private static void AddMudItem(IElement component, HtmlElement grid, IEnumerable<IRazorFileNode> nodesToMove)
    {
        grid.AddHtmlElement("MudItem", mudItem =>
        {
            mudItem.AddAttribute("xs", component.GetStereotypeProperty("Layout", "xs", "12"));
            mudItem.AddAttributeIfNotEmpty("sm", component.GetStereotypeProperty("Layout", "sm", ""));
            mudItem.AddAttributeIfNotEmpty("md", component.GetStereotypeProperty("Layout", "md", ""));
            mudItem.AddAttributeIfNotEmpty("lg", component.GetStereotypeProperty("Layout", "lg", ""));
            mudItem.AddAttributeIfNotEmpty("xl", component.GetStereotypeProperty("Layout", "xl", ""));
            mudItem.AddAttributeIfNotEmpty("xxl", component.GetStereotypeProperty("Layout", "xxl", ""));
            foreach (var nodeToMove in nodesToMove)
            {
                nodeToMove.Remove();
                mudItem.AddChildNode(nodeToMove);
            }
        });
    }

    /*
    private bool IsMyView(IElement component)
    {
        while (component.ParentElement is not null && component.Name != "MyProduct") 
        { 
            component = component.ParentElement;
        }
        return component?.ParentElement is not null;
    }*/
}