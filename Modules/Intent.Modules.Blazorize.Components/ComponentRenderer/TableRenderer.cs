using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class TableRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TableRenderer(IRazorComponentBuilderResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var table = new TableModel(component);
        var loadingCode = new RazorCodeDirective(new CSharpStatement($"if ({_template.GetElementBinding(table)} is null)"), _template.BlazorFile);
        loadingCode.AddHtmlElement("div", rowDiv =>
        {
            rowDiv.AddAttribute("class", "row");
            rowDiv.AddHtmlElement("div", alertDiv =>
            {
                alertDiv.AddAttribute("class", "col-12 alert alert-info")
                    .WithText("Loading...");
            });
        });
        node.AddNode(loadingCode);
        var tableCode = new RazorCodeDirective(new CSharpStatement($"if ({_template.GetElementBinding(table)} is not null)"), _template.BlazorFile);
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
                var mappingManager = _template.CreateMappingManager();
                var mappedEnd = _template.GetMappedEndFor(table);
                tbody.AddCodeBlock($"foreach(var item in {_template.GetBinding(mappedEnd, mappingManager)})", block =>
                {
                    mappingManager.SetFromReplacement(mappedEnd.SourceElement, "item");
                    block.AddHtmlElement("TableRow", tr =>
                    {
                        if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
                        {
                            tr.AddAttribute("@onclick", $"() => {_template.GetStereotypePropertyBinding(table, "On Row Click", mappingManager)}");
                        }
                        foreach (var column in table.Columns)
                        {
                            tr.AddHtmlElement("TableRowCell", td =>
                            {
                                var columnMapping = _template.GetElementBinding(column, mappingManager);
                                if (columnMapping != null)
                                {
                                    td.WithText($"@{columnMapping}");
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

        node.AddNode(tableCode);
    }
}