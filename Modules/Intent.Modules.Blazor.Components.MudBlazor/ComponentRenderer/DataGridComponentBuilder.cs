using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
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
            var refBinding = _bindingManager.GetBinding(model, "28fb9d1a-cd73-4a5e-ac21-6340ec2e90d0", mudDataGrid);
            if (refBinding != null)
            {
                mudDataGrid.AddAttribute("@ref", refBinding.ToString());
            }

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
                        _componentResolver.BuildComponent(child, toolbar);
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
                    var columnMapping = _bindingManager.GetElementBinding(column, rowTemplate);
                    if (columnMapping != null)
                    {
                        rowTemplate.AddHtmlElement("PropertyColumn", col =>
                        {
                            col.AddAttributeIfNotEmpty("Class", !string.IsNullOrWhiteSpace(model.GetInteraction()?.OnRowClick()) ? "cursor-pointer" : null);
                            col.AddAttribute("Title", column.Name);

                            col.AddAttribute("Property", $"x => x.{columnMapping}");
                        });
                    }
                    else
                    {
                        rowTemplate.AddHtmlElement("TemplateColumn", tc =>
                        {
                            tc.AddAttribute("Title", column.Name);
                            tc.AddHtmlElement("CellTemplate", ct =>
                                ct.AddHtmlElement("MudStack", ms =>
                                {
                                    ms.AddMappingReplacement(mappedEnd.SourceElement, "context.Item");
                                    ms.AddAttribute("Row");

                                    foreach (var child in column.InternalElement.ChildElements)
                                    {
                                        _componentResolver.BuildComponent(child, ms);
                                    }
                                }));
                        });

                    }
                }
            });

            //if (model.HasPagination())
            //{
            mudDataGrid.AddHtmlElement("PagerContent", pagerContent =>
            {
                pagerContent.AddHtmlElement("MudDataGridPager", pagination =>
                {
                    pagination.AddAttribute("T", _componentTemplate.GetTypeName(mappedSourceType.AsTypeReference()));
                    pagination.AddAttributeIfNotEmpty("PageSizeOptions", "new int[] { 10, 25, 50, 100 }");
                });
            });
            //}

            var pageRequestMapping = _bindingManager.GetMappedEndFor(model, "53a618ca-e5aa-49ca-a993-8cd935683748");

            if (pageRequestMapping != null)
            {
                _componentTemplate.RazorFile.AfterBuild(file =>
                {

                    var codeBlock = _componentTemplate.GetCodeBehind();
                    (codeBlock.GetReferenceForModel(pageRequestMapping.SourceElement) as CSharpClassMethod)?.Async();
                    var pageRequestBinding = _bindingManager.GetBinding(pageRequestMapping);

                    var returnType = $"GridData<{_componentTemplate.GetTypeName(mappedSourceType.AsTypeReference())}>";
                    codeBlock.AddMethod(returnType, $"Load{model.Name.ToCSharpIdentifier()}Data", method =>
                    {
                        codeBlock.Template.AddUsing("System.Linq");
                        method.Private().Async();
                        method.AddParameter($"GridState<{_componentTemplate.GetTypeName(mappedSourceType.AsTypeReference())}>", "state");
                        method.AddStatement("var pageNo = state.Page + 1;");
                        method.AddStatement("var pageSize = state.PageSize;");
                        method.AddStatement("var sorting = string.Join(\", \", state.SortDefinitions.Select(x => $\"{x.SortBy} {(x.Descending ? \"desc\" : \"asc\")}\"));");
                        method.AddStatement(new CSharpAwaitExpression(pageRequestBinding.WithSemicolon()));
                        method.AddStatement($"return new {returnType}() {{ TotalItems = {_bindingManager.GetBinding(model, "a5e50b44-c402-4006-bcea-a25ab7dc0c56", isTargetNullable: true)} ?? 0, Items = {_bindingManager.GetElementBinding(model, isTargetNullable: true)} ?? [] }};");
                    });
                });
            }
        };

        parentNode.AddChildNode(mudDataGrid);
        return [mudDataGrid];
    }
}