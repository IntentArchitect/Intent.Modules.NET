using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public interface IRazorComponentBuilderProvider
{
    void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder);
    //void BuildComponents(IEnumerable<IElement> components, IRazorFileNode node);
    IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode node);
}

public interface IRazorComponentBuilder
{
    IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode);
}

public class RazorComponentBuilderProvider : IRazorComponentBuilderProvider
{
    public IRazorComponentTemplate ComponentTemplate { get; }
    private readonly Dictionary<string, IRazorComponentBuilder> _componentRenderers = new();
    private List<IRazorComponentInterceptor> _interceptors = [];
    private static readonly HashSet<Type> ComponentTypesWhichHaveConfiguredRazor = [];

    public RazorComponentBuilderProvider(IRazorComponentTemplate template)
    {
        ComponentTemplate = template;
        _componentRenderers.Add(DisplayComponentModel.SpecializationTypeId, new DisplayCommonComponentBuilder(this, template));
    }

    public void Register(string elementSpecializationId, IRazorComponentBuilder componentBuilder)
    {
        _componentRenderers[elementSpecializationId] = componentBuilder;
    }

    public void AddInterceptor(IRazorComponentInterceptor interceptor)
    {
        _interceptors.Add(interceptor);
    }

    public IRazorComponentBuilder ResolveFor(IElement component)
    {
        if (!_componentRenderers.ContainsKey(component.SpecializationTypeId))
        {
            return new EmptyElementRenderer(this, ComponentTemplate);
        }
        return _componentRenderers[component.SpecializationTypeId];
    }

    public void BuildComponents(IEnumerable<IElement> components, IRazorFileNode node)
    {
        foreach (var component in components)
        {
            BuildComponent(component, node);
        }
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode node)
    {
        var razorComponentBuilder = ResolveFor(component);



        var builtComponent = razorComponentBuilder.BuildComponent(component, node);

        foreach (var interceptor in _interceptors)
        {
            interceptor.Handle(component, builtComponent, node);
        }

        return builtComponent;
    }
}

public class DisplayCommonComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public DisplayCommonComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new DisplayComponentModel(component).TypeReference.Element.AsComponentModel();

        var htmlElement = new HtmlElement(_componentTemplate.GetTypeName((IElement)component.TypeReference.Element), _componentTemplate.RazorFile);
        foreach (var property in model.Properties.Where(x => x.HasBindable()))
        {
            htmlElement.AddAttributeIfNotEmpty(property.Name, _bindingManager.GetElementBinding(property, parentNode)?.ToString());
        }
        foreach (var property in model.EventEmitters.Where(x => x.HasBindable()))
        {
            var binding = _bindingManager.GetElementBinding(property, parentNode);
            if (binding is CSharpInvocationStatement invocation && invocation.Statements.Any(x => x.Text == "value"))
            {
                htmlElement.AddAttributeIfNotEmpty(property.Name, binding?.ToLambda("value"));
            }
            else
            {
                htmlElement.AddAttributeIfNotEmpty(property.Name, binding?.ToLambda());
            }
        }
        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}

public static class EventBindingHelper
{
    public static string ToLambda(this ICSharpExpression invocation, string parameter = null)
    {
        if (invocation == null)
        {
            return null;
        }

        return $"({parameter ?? ""}) => {invocation}";
    }
}
