using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;

namespace Intent.Modules.Blazor.Components.Core.Templates;

public interface IRazorComponentBuilderProvider
{
    void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder);
    IRazorComponentBuilder ResolveFor(IElement component);
}

public interface IRazorComponentBuilder
{
    void BuildComponent(IElement component, IRazorFileNode node);
}

public class RazorComponentBuilderProvider : IRazorComponentBuilderProvider
{
    public IRazorComponentTemplate ComponentTemplate { get; }
    private Dictionary<string, IRazorComponentBuilder> _componentRenderers = new();

    public RazorComponentBuilderProvider(IRazorComponentTemplate template)
    {
        ComponentTemplate = template;
    }

    public void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder)
    {
        _componentRenderers[elementSpecializationId] = componentBuilder;
    }

    public IRazorComponentBuilder ResolveFor(IElement component)
    {
        if (!_componentRenderers.ContainsKey(component.SpecializationTypeId))
        {
            return new EmptyElementRenderer(this, ComponentTemplate);
        }
        return _componentRenderers[component.SpecializationTypeId];
    }
}
