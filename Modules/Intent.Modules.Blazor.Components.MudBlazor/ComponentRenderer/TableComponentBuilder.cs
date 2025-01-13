using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class TableComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public TableComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var table = new TableModel(component);
        var tableCode = IRazorCodeDirective.Create(new CSharpStatement($"@if ({_bindingManager.GetElementBinding(table, isTargetNullable: true)} is not null)"), _componentTemplate.RazorFile);
        tableCode.AddHtmlElement("MudTable", mudTable =>
        {
            var mappedEnd = _bindingManager.GetMappedEndFor(table);
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

            mudTable.AddAttribute("T", _componentTemplate.GetTypeName(mappedSourceType.AsTypeReference()));
            mudTable.AddAttribute("Items", $"@{_bindingManager.GetElementBinding(table)}");
            mudTable.AddAttribute("Hover", "true");

            if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
            {
                mudTable.AddMappingReplacement(mappedEnd.SourceElement, "e.Item");
                mudTable.AddAttributeIfNotEmpty("OnRowClick", $"{_bindingManager.GetBinding(table, "On Row Click", mudTable).ToLambda("e")}");
            }
            mudTable.AddHtmlElement("HeaderContent", tableHeader =>
            {
                foreach (var column in table.Columns)
                {
                    tableHeader.AddHtmlElement("MudTh", headerCell =>
                    {
                        headerCell.WithText(column.Name);
                    });
                }
            });
            mudTable.AddHtmlElement("RowTemplate", rowTemplate =>
            {
                if (mappedEnd == null)
                {
                    return;
                }

                rowTemplate.AddMappingReplacement(mappedEnd.SourceElement, "context");
                foreach (var column in table.Columns)
                {
                    rowTemplate.AddHtmlElement("MudTd", td =>
                    {

                        td.AddAttributeIfNotEmpty("Class", !string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()) ? "cursor-pointer" : null);
                        var columnMapping = _bindingManager.GetMappedEndFor(column);
                        if (columnMapping != null)
                        {
                            var columnBinding = _bindingManager.GetBinding(columnMapping, rowTemplate);
                            td.WithText(Regex.IsMatch(columnBinding.ToString(), @"[^\w\.]") ? $"@({columnBinding})" : columnMapping.Sources.Count() != 0 ? $"@{columnBinding}" : columnBinding.ToString());
                        }
                        else
                        {
                            foreach (var child in column.InternalElement.ChildElements)
                            {
                                _componentResolver.BuildComponent(child, td);
                            }
                        }
                    });
                }
            });

            if (table.HasPagination())
            {
                _componentTemplate.RazorFile.AfterBuild(file =>
                {
                    if (_componentTemplate.GetCodeBehind().TryGetReferenceForModel(_bindingManager.GetMappedEndFor(table, "913e0abf-0bec-43ea-9286-eb70187c84ef")?.SourceElement.Id, out var reference)
                        && reference is CSharpClassMethod method)
                    {
                        var processingFieldName = method.AddProcessingFieldName();
                        var code = IRazorCodeDirective.Create(new CSharpStatement($"@if ({processingFieldName})"), _componentTemplate.RazorFile);

                        code.AddHtmlElement("MudProgressLinear", loadingBar =>
                        {
                            loadingBar.AddAttribute("Color", "Color.Primary");
                            loadingBar.AddAttribute("Indeterminate", "true");
                            loadingBar.AddAttribute("Class", "my-7");
                        });
                        tableCode.AddAbove(code);
                    }
                });
                mudTable.AddHtmlElement("PagerContent", pagerContent =>
                {
                    pagerContent.AddHtmlElement("MudPagination", pagination =>
                    {
                        //pagination.AddAttributeIfNotEmpty("Selected", _bindingManager.GetBinding(table, "e09c822e-26fa-435c-aabd-d2f4709b2cc4")?.ToString());
                        pagination.AddAttributeIfNotEmpty("SelectedChanged", _bindingManager.GetBinding(table, "913e0abf-0bec-43ea-9286-eb70187c84ef").ToLambda("value"));
                        pagination.AddAttributeIfNotEmpty("Count", _bindingManager.GetBinding(table, "d0c38d62-7399-4eb3-853b-e67902c6d4fc")?.ToString());
                        pagination.AddAttribute("Class", "pa-4");
                    });
                });
            }
        });

        parentNode.AddChildNode(tableCode);
        return [tableCode];
    }
}