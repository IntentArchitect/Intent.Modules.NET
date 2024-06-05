using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;

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

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var textInput = new TextInputModel(component);
        var htmlElement = new HtmlElement("MudTextField", _componentTemplate.RazorFile);
        var valueMapping = _bindingManager.GetMappedEndFor(textInput);
        var valueBinding = _bindingManager.GetBinding(valueMapping)?.ToString();
        htmlElement.AddAttributeIfNotEmpty("@bind-Value", valueBinding)
                        .AddAttributeIfNotEmpty("Label", textInput.GetLabelAddon()?.Label().TrimEnd(':'));
        parentNode.AddChildNode(htmlElement);
        if (parentNode.GetAllNodesInHierarchy().OfType<HtmlElement>().Any(x => x.Name == "MudForm"))
        {
            if (valueMapping != null)
            {
                htmlElement.AddAttribute("For", $"@(() => {valueBinding})");
                //if (valueMapping.SourceElement.TypeReference?.IsNullable == false)
                //{
                //    htmlElement.AddAttribute("Required", "true");
                //    htmlElement.AddAttribute("RequiredError", $"{textInput.GetLabelAddon()?.Label().TrimEnd(':') ?? textInput.Name} is required!");
                //}
            }
        }
    }
}