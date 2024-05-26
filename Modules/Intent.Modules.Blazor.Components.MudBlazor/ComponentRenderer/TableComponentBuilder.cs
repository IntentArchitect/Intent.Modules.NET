using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var table = new TableModel(component);
        var loadingCode = new RazorCodeDirective(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is null)"), _componentTemplate.RazorFile);
        loadingCode.AddHtmlElement("MudProgressLinear", loadingBar =>
        {
            loadingBar.AddAttribute("Color", "Color.Primary");
            loadingBar.AddAttribute("Indeterminate", "true");
            loadingBar.AddAttribute("Class", "my-7");
        });
        parentNode.AddChildNode(loadingCode);
        var tableCode = new RazorCodeDirective(new CSharpStatement($"if ({_bindingManager.GetElementBinding(table)} is not null)"), _componentTemplate.RazorFile);
        tableCode.AddHtmlElement("MudTable", mudTable =>
        {
            mudTable.AddAttribute("T", _componentTemplate.GetTypeName(_bindingManager.GetMappedEndFor(table).SourceElement.TypeReference.Element.AsTypeReference()));
            mudTable.AddAttribute("Items", $"@{_bindingManager.GetElementBinding(table)}");
            mudTable.AddAttribute("Hover", "true");

            var mappedEnd = _bindingManager.GetMappedEndFor(table);

            if (!string.IsNullOrWhiteSpace(table.GetInteraction()?.OnRowClick()))
            {
                mudTable.AddMappingReplacement(mappedEnd.SourceElement, "e.Item");
                mudTable.AddAttributeIfNotEmpty("OnRowClick", $"{_bindingManager.GetStereotypePropertyBinding(table, "On Row Click", mudTable).ToLambda("e")}");
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


                        var columnMapping = _bindingManager.GetElementBinding(column, rowTemplate);
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

        parentNode.AddChildNode(tableCode);
    }
}