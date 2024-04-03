using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class TableRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TableRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var table = new TableModel(component);
        var loadingCode = new RazorCodeBlock(new CSharpStatement($"if ({_template.GetElementBinding(table)} is null)"), _template.BlazorFile);
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
        var tableCode = new RazorCodeBlock(new CSharpStatement($"if ({_template.GetElementBinding(table)} is not null)"), _template.BlazorFile);
        tableCode.AddHtmlElement("table", htmlTable =>
        {
            htmlTable.AddAttribute("class", "table")
                .AddHtmlElement("thead", thead =>
                {
                    thead.AddHtmlElement("tr", tr =>
                    {
                        foreach (var column in table.Columns)
                        {
                            tr.AddHtmlElement("th", th =>
                            {
                                th.WithText(column.Name);
                            });
                        }
                    });
                })
                .AddHtmlElement("tbody", tbody =>
                {
                    var mappingManager = _template.CreateMappingManager();
                    var mappedEnd = _template.GetMappedEndFor(table);
                    tbody.AddCodeBlock($"foreach(var item in {_template.GetBinding(mappedEnd, mappingManager)})", block =>
                    {
                        mappingManager.SetFromReplacement(mappedEnd.SourceElement, "item");
                        block.AddHtmlElement("tr", tr =>
                        {
                            if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
                            {
                                tr.AddAttribute("@onclick", $"() => {_template.GetStereotypePropertyBinding(table, "On Row Click", mappingManager)}");
                            }
                            foreach (var column in table.Columns)
                            {
                                tr.AddHtmlElement("td", td =>
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