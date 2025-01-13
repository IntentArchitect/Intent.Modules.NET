using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class ComponentViewBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public ComponentViewBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var view = new ComponentViewModel(component);
        if (view.HasLoaderSettings())
        {
            var isLoadingProperty = _bindingManager.GetElementBinding(view, "2cfd43b2-2a18-4ac0-8cf3-d1aec9d7e699", isTargetNullable: false);
            var loadingCode = IRazorCodeDirective.Create(new CSharpStatement($"@if ({isLoadingProperty})"), _componentTemplate.RazorFile);
            loadingCode.AddHtmlElement("MudProgressLinear", loadingBar =>
            {
                loadingBar.AddAttribute("Color", "Color.Primary");
                loadingBar.AddAttribute("Indeterminate", "true");
                loadingBar.AddAttribute("Class", "my-7");
            });
            parentNode.AddChildNode(loadingCode);

            var errorMessageProperty = _bindingManager.GetElementBinding(view, "2e482e27-b176-43cf-b80a-33123036142a", isTargetNullable: true);
            if (errorMessageProperty != null)
            {
                var errorCode = IRazorCodeDirective.Create(new CSharpStatement($"@if (!{isLoadingProperty} && {errorMessageProperty} != null)"), _componentTemplate.RazorFile);
                errorCode.AddHtmlElement("MudAlert", alert =>
                {
                    alert.AddAttribute("Severity", "Severity.Error");
                    alert.WithText("@" + errorMessageProperty);
                });
                parentNode.AddChildNode(errorCode);
            }
        }

        //var mudGrid = new HtmlElement("MudGrid", _componentTemplate.RazorFile);
        //parentNode.AddChildNode(mudGrid);
        foreach (var child in component.ChildElements)
        {
            _componentResolver.BuildComponent(child, parentNode);
        }
        return [];
    }
}