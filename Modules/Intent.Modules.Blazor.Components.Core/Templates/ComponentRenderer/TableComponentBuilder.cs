using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class TableComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;
    private readonly BindingManager _bindingManager;

    public TableComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var table = new TableModel(component);
        var loadingCode = new RazorCodeDirective(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is null)"), _template.BlazorFile);
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
        var tableCode = new RazorCodeDirective(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is not null)"), _template.BlazorFile);
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
                    var mappedEnd = _bindingManager.GetMappedEndFor(table);
                    tbody.AddCodeBlock($"foreach(var item in {_bindingManager.GetBinding(mappedEnd, mappingManager)})", block =>
                    {
                        mappingManager.SetFromReplacement(mappedEnd.SourceElement, "item");
                        block.AddHtmlElement("tr", tr =>
                        {
                            if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
                            {
                                tr.AddAttribute("@onclick", $"() => {_bindingManager.GetStereotypePropertyBinding(table, "On Row Click", mappingManager)}");
                            }
                            foreach (var column in table.Columns)
                            {
                                tr.AddHtmlElement("td", td =>
                                {
                                    var columnMapping = _bindingManager.GetElementBinding(column, mappingManager);
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