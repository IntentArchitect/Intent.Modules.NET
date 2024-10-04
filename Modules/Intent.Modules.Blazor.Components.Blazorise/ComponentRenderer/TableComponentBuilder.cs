using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var table = new TableModel(component);
        var loadingCode = IRazorCodeDirective.Create(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is null)"), _componentTemplate.RazorFile);
        loadingCode.AddHtmlElement("div", rowDiv =>
        {
            rowDiv.AddAttribute("class", "row");
            rowDiv.AddHtmlElement("div", alertDiv =>
            {
                alertDiv.AddAttribute("class", "col-12 alert alert-info")
                    .WithText("Loading...");
            });
        });
        parentNode.AddChildNode(loadingCode);
        var tableCode = IRazorCodeDirective.Create(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is not null)"), _componentTemplate.RazorFile);
        tableCode.AddHtmlElement("Table", htmlTable =>
        {
            htmlTable.AddHtmlElement("TableHeader", tableHeader =>
            {
                foreach (var column in table.Columns)
                {
                    tableHeader.AddHtmlElement("TableHeaderCell", headerCell =>
                    {
                        headerCell.WithText(column.Name);
                    });
                }
            });
            htmlTable.AddHtmlElement("TableBody", tbody =>
            {
                var mappingManager = _componentTemplate.CreateMappingManager();
                var mappedEnd = _bindingManager.GetMappedEndFor(table);
                if (mappedEnd == null)
                {
                    return;
                }
                tbody.AddCodeBlock($"@foreach(var item in {_bindingManager.GetBinding(mappedEnd, parentNode)})", block =>
                {
                    tbody.AddMappingReplacement(mappedEnd.SourceElement, "item");
                    block.AddHtmlElement("TableRow", tr =>
                    {
                        if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
                        {
                            tr.AddAttributeIfNotEmpty("@onclick", $"{_bindingManager.GetBinding(table, "On Row Click", tbody).ToLambda()}");
                        }
                        foreach (var column in table.Columns)
                        {
                            tr.AddHtmlElement("TableRowCell", td =>
                            {
                                var columnMapping = _bindingManager.GetElementBinding(column, tbody);
                                if (columnMapping != null)
                                {
                                    td.WithText(columnMapping.ToString().Contains(" ") ? $"@({columnMapping})" : $"@{columnMapping}");
                                }
                                else
                                {
                                    foreach (var child in column.InternalElement.ChildElements)
                                    {
                                        _componentResolver.ResolveFor(child).BuildComponent(child, td);
                                    }
                                }
                            });
                        }
                    });
                });
            });
        });

        parentNode.AddChildNode(tableCode);
    }
}