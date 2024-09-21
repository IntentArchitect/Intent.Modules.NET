using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class TextInputComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public TextInputComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new TextInputModel(component);
        var htmlElement = new HtmlElement("MudTextField", _componentTemplate.RazorFile);
        var valueMapping = _bindingManager.GetMappedEndFor(model);
        var valueBinding = _bindingManager.GetBinding(valueMapping, parentNode);
        var onValueChangedMapping = _bindingManager.GetMappedEndFor(model, "914fd4f3-75fb-4b93-9c1b-21ba304f054b"); // On Value Changed
        var onValueChangedBinding = _bindingManager.GetBinding(onValueChangedMapping, parentNode); 
        htmlElement.AddAttributeIfNotEmpty("@bind-Value", onValueChangedBinding == null ? valueBinding?.ToString() : null)
            .AddAttributeIfNotEmpty("T", onValueChangedBinding != null ? "string" : null)
            .AddAttributeIfNotEmpty("Value", onValueChangedBinding != null ? valueBinding?.ToString() : null)
            .AddAttributeIfNotEmpty("ValueChanged", onValueChangedBinding?.ToLambda("value"))
            .AddAttributeIfNotEmpty("Label", string.IsNullOrWhiteSpace(model.GetAppearance().Placeholder()) ? model.Name : null)
            .AddAttributeIfNotEmpty("Placeholder", model.GetAppearance().Placeholder())
            .AddAttributeIfNotEmpty("Immediate", model.GetBehaviours().Immediate() ? "true" : null)
            .AddAttributeIfNotEmpty("DebounceInterval", !model.GetBehaviours().Immediate() && model.GetBehaviours().DebounceInterval() != null ? model.GetBehaviours().DebounceInterval().Value.ToString() : null)
            .AddAttributeIfNotEmpty("AdornmentIcon", model.HasIcon() ? $"@Icons.Material.{model.GetIcon().Variant().Name}.{model.GetIcon().IconValue().Name}" : null)
            .AddAttributeIfNotEmpty("Adornment", model.HasIcon() ? "Adornment.Start" : null)
            ;

        if (valueMapping != null && onValueChangedMapping != null)
        {
            _componentTemplate.RazorFile.AfterBuild(file =>
            {
                _componentTemplate.GetCodeBehind().TryGetReferenceForModel(onValueChangedMapping.SourceElement.Id, out var reference);
                if (reference is CSharpClassMethod method)
                {
                    method.InsertStatement(0, $"{valueBinding} = value;");
                }
            });
        }

        parentNode.AddChildNode(htmlElement);
        if (parentNode.GetAllNodesInHierarchy().OfType<HtmlElement>().Any(x => x.Name == "MudForm"))
        {
            if (valueMapping != null)
            {
                htmlElement.AddAttribute("For", $"@(() => {valueBinding})");
            }
        }
        return [htmlElement];
    }
}