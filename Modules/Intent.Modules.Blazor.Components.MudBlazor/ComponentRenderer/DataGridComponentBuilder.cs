using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;
using static Intent.Modelers.UI.Core.Api.TableModelStereotypeExtensions;

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
            //mudDataGrid.AddAttribute("Items", $"@{_bindingManager.GetElementBinding(model)}");
            mudDataGrid.AddAttribute("ServerData", $"Load{model.Name.ToCSharpIdentifier()}Data");
            mudDataGrid.AddAttribute("Hover", "true");

            if (!string.IsNullOrWhiteSpace(model.GetInteraction()?.OnRowClick()))
            {
                mudDataGrid.AddMappingReplacement(mappedEnd.SourceElement, "e.Item");
                mudDataGrid.AddAttributeIfNotEmpty("RowClick", $"{_bindingManager.GetBinding(model, "On Row Click", mudDataGrid).ToLambda("e")}");
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

            var pageRequestBinding = _bindingManager.GetBinding(model, "53a618ca-e5aa-49ca-a993-8cd935683748");

            if (pageRequestBinding != null)
            {
                _componentTemplate.RazorFile.AfterBuild(file =>
                {
                    var codeBlock = _componentTemplate.GetCodeBehind();
                    var returnType = $"GridData<{_componentTemplate.GetTypeName(mappedSourceType.AsTypeReference())}>";
                    codeBlock.AddMethod(returnType, $"Load{model.Name.ToCSharpIdentifier()}Data", method =>
                    {
                        method.Private().Async();
                        method.AddParameter($"GridState<{_componentTemplate.GetTypeName(mappedSourceType.AsTypeReference())}>", "state");
                        method.AddStatement("var pageNo = state.Page + 1;");
                        method.AddStatement("var pageSize = state.PageSize;");
                        method.AddStatement("var sorting = string.Join(\", \", state.SortDefinitions.Select(x => $\"{x.SortBy} {(x.Descending ? \"desc\" : \"asc\")}\"));");
                        method.AddStatement(new CSharpAwaitExpression(pageRequestBinding.WithSemicolon()));
                        method.AddStatement($"return new {returnType}() {{ TotalItems = {_bindingManager.GetBinding(model, "a5e50b44-c402-4006-bcea-a25ab7dc0c56")}, Items = {_bindingManager.GetElementBinding(model)} }};");
                    });

                });
            }
        };

        parentNode.AddChildNode(mudDataGrid);
    }
}