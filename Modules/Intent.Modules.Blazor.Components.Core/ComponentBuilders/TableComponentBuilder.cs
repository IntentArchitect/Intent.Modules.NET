using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Core.ComponentBuilders;

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
                    var mappedEnd = _bindingManager.GetMappedEndFor(table);
                    tbody.AddCodeBlock($"@foreach(var item in {_bindingManager.GetBinding(mappedEnd, parentNode)})", block =>
                    {
                        parentNode.AddMappingReplacement(mappedEnd.SourceElement, "item");
                        block.AddHtmlElement("tr", tr =>
                        {
                            if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
                            {
                                tr.AddAttribute("@onclick", $"() => {_bindingManager.GetBinding(table, "On Row Click", parentNode)}");
                            }
                            foreach (var column in table.Columns)
                            {
                                tr.AddHtmlElement("td", td =>
                                {
                                    var columnMapping = _bindingManager.GetElementBinding(column, parentNode);
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

        parentNode.AddChildNode(tableCode);
    }
}