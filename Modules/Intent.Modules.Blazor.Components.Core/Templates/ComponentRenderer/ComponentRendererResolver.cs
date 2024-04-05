using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;

public class ComponentRendererResolver : IRazorComponentBuilderResolver
{
    public RazorComponentTemplate Template { get; }
    private Dictionary<string, IRazorComponentBuilder> _componentRenderers = new();

    public ComponentRendererResolver(RazorComponentTemplate template)
    {
        Template = template;
    }

    public void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder)
    {
        _componentRenderers[elementSpecializationId] = componentBuilder;
    }

    public IRazorComponentBuilder ResolveFor(IElement component)
    {
        if (!_componentRenderers.ContainsKey(component.SpecializationTypeId))
        {
            return new EmptyElementRenderer(this, Template);
        }
        return _componentRenderers[component.SpecializationTypeId];
    }
}


public interface IRazorComponentBuilderResolver
{
    void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder);
    IRazorComponentBuilder ResolveFor(IElement component);
}

public interface IRazorComponentBuilder
{
    void BuildComponent(IElement component, IRazorFileNode node);
}

public class TextInputComponentRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public TextInputComponentRenderer(IRazorComponentBuilderResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("label", _template.BlazorFile)
            .WithText(textInput.GetLabelAddon()?.Label())
            .AddHtmlElement("InputText", inputText =>
            {
                inputText.AddAttribute("@bind-Value", textInput.Value.Trim('{', '}'));
            });
        node.AddNode(htmlElement);
    }
}

public class EmptyElementRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public EmptyElementRenderer(IRazorComponentBuilderResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var htmlElement = new HtmlElement(component.Name, _template.BlazorFile);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }
        node.AddNode(htmlElement);
    }
}