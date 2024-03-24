using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class ComponentRendererResolver : IComponentRendererResolver
{
    public RazorComponentTemplate Template { get; }
    private Dictionary<string, Func<IElement, IComponentRenderer>> _componentRenderers = new();

    public ComponentRendererResolver(RazorComponentTemplate template)
    {
        Template = template;
        _componentRenderers[FormModel.SpecializationTypeId] = (component) => new FormComponentRenderer(this, Template);
        _componentRenderers[TextInputModel.SpecializationTypeId] = (component) => new TextInputComponentRenderer(this, Template);
        _componentRenderers[ButtonModel.SpecializationTypeId] = (component) => new ButtonRenderer(this, Template);
        _componentRenderers[TableModel.SpecializationTypeId] = (component) => new TableRenderer(this, Template);
        _componentRenderers[DisplayComponentModel.SpecializationTypeId] = (component) => new CustomComponentRenderer(this, Template);
    }
    public IComponentRenderer ResolveFor(IElement component)
    {
        if (!_componentRenderers.ContainsKey(component.SpecializationTypeId))
        {
            return new EmptyElementRenderer(this, Template);
        }
        return _componentRenderers[component.SpecializationTypeId](component);
    }
}


public interface IComponentRendererResolver
{
    IComponentRenderer ResolveFor(IElement component);
}

public interface IComponentRenderer
{
    void Render(IElement component, IRazorFileNode node);
}

public class TextInputComponentRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TextInputComponentRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("label", _template.RazorFile)
            .WithText(textInput.GetLabelAddon()?.Label())
            .AddHtmlElement("InputText", inputText =>
            {
                inputText.AddAttribute("@bind-Value", textInput.Value.Trim('{', '}'));
            });
        node.AddNode(htmlElement);
    }
}

public class EmptyElementRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public EmptyElementRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var htmlElement = new HtmlElement(component.Name, _template.RazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).Render(child, htmlElement);
        }
        node.AddNode(htmlElement);
    }
}

public class ButtonRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public ButtonRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var button = new ButtonModel(component);
        var htmlElement = new HtmlElement("button", _template.RazorFile)
            .AddAttribute("type", "submit")
            .WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : "Submit");
        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        node.AddNode(htmlElement);
    }
}

public class TableRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TableRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void Render(IElement component, IRazorFileNode node)
    {
        var table = new TableModel(component);
        var tableCode = new RazorCodeBlock(new CSharpStatement($"if ({_template.GetElementBinding(table)} is not null)"), _template.RazorFile);
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
                    tbody.AddCodeBlock($"foreach(var item in {_template.GetElementBinding(table, out var mappedEnd, mappingManager)})", block =>
                    {
                        mappingManager.SetFromReplacement(mappedEnd.SourceElement, "item");
                        block.AddHtmlElement("tr", tr =>
                        {
                            foreach (var column in table.Columns)
                            {
                                tr.AddHtmlElement("td", th =>
                                {
                                    th.WithText($"@{_template.GetElementBinding(column, mappingManager)}");
                                });
                            }
                        });
                    });
                });
        });
        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        node.AddNode(tableCode);
    }
}