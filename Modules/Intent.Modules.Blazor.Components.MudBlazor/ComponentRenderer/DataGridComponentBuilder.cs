using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class DataGridComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public DataGridComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new DataGridModel(component);
        var mudDataGrid = new HtmlElement("MudDataGrid", _componentTemplate.RazorFile);
        {
            var mappedEnd = _bindingManager.GetMappedEndFor(model);
            var genericTypeArguments = new Dictionary<string, ITypeReference>();
            var mappedSourceType = mappedEnd.SourceElement.TypeReference.Element;
            foreach (var pathTarget in mappedEnd.SourcePath)
            {
                var genericTypes = (pathTarget.Element.TypeReference?.Element as IElement)?.GenericTypes.ToArray() ?? [];
                for (var i = genericTypes.Length - 1; i >= 0; i--)
                {
                    genericTypeArguments.Add(genericTypes[i].Id, pathTarget.Element.TypeReference.GenericTypeParameters.ToArray()[i]);
                }
            }

            if (mappedSourceType.SpecializationTypeId == "Generic Type")
            {
                mappedSourceType = genericTypeArguments[mappedSourceType.Id].Element;
            }

            mudDataGrid.AddAttribute("T", _componentTemplate.GetTypeName(mappedSourceType.AsTypeReference()));
            mudDataGrid.AddAttribute("Items", $"@{_bindingManager.GetElementBinding(model)}");
            mudDataGrid.AddAttribute("Hover", "true");

            if (!string.IsNullOrWhiteSpace(model.GetInteraction()?.OnRowClick()))
            {
                mudDataGrid.AddMappingReplacement(mappedEnd.SourceElement, "e.Item");
                mudDataGrid.AddAttributeIfNotEmpty("OnRowClick", $"{_bindingManager.GetBinding(model, "On Row Click", mudDataGrid).ToLambda("e")}");
            }

            if (model.Toolbar != null)
            {
                mudDataGrid.AddHtmlElement("ToolBarContent", toolbar =>
                {
                    foreach (var child in model.Toolbar.InternalElement.ChildElements)
                    {
                        _componentResolver.ResolveFor(child).BuildComponent(child, toolbar);
                    }
                });
            }

            mudDataGrid.AddHtmlElement("Columns", rowTemplate =>
            {
                if (mappedEnd == null)
                {
                    return;
                }

                rowTemplate.AddMappingReplacement(mappedEnd.SourceElement, null);
                foreach (var column in model.Columns)
                {
                    rowTemplate.AddHtmlElement("PropertyColumn", col =>
                    {
                        var columnMapping = _bindingManager.GetElementBinding(column, rowTemplate);
                        if (columnMapping != null)
                        {
                            col.AddAttribute("Property", $"x => x.{columnMapping}");
                        }
                        col.AddAttributeIfNotEmpty("Class", !string.IsNullOrWhiteSpace(model.GetInteraction()?.OnRowClick()) ? "cursor-pointer" : null);
                        //else
                        //{
                        //    foreach (var child in column.InternalElement.ChildElements)
                        //    {
                        //        _componentResolver.ResolveFor(child).BuildComponent(child, col);
                        //    }
                        //}
                    });
                }
            });

            //if (model.HasPagination())
            //{
            mudDataGrid.AddHtmlElement("PagerContent", pagerContent =>
            {
                pagerContent.AddHtmlElement("MudDataGridPager", pagination =>
                {
                    pagination.AddAttribute("T", _componentTemplate.GetTypeName(mappedSourceType.AsTypeReference()));
                    pagination.AddAttributeIfNotEmpty("PageSizeOptions", "new int[] { 5, 10, 20 }");
                });
            });
            //}
        };

        parentNode.AddChildNode(mudDataGrid);
    }
}