using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

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
        _componentRenderers[ContainerModel.SpecializationTypeId] = (component) => new ContainerRenderer(this, Template);
        _componentRenderers[TableModel.SpecializationTypeId] = (component) => new TableRenderer(this, Template);
        _componentRenderers[TextModel.SpecializationTypeId] = (component) => new TextRenderer(this, Template);
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
    void BuildComponent(IElement component, IRazorFileNode node);
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

public class EmptyElementRenderer : IComponentRenderer
{
    private readonly IComponentRendererResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public EmptyElementRenderer(IComponentRendererResolver componentResolver, RazorComponentTemplate template)
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