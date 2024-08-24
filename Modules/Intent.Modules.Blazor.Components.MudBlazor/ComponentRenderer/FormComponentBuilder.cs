using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class FormComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public FormComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var formModel = new FormModel(component);
        var modelMapping = _bindingManager.GetMappedEndFor(formModel, "Model");
        var modelBinding = _bindingManager.GetBinding(modelMapping);
        if (modelBinding == null)
        {
            throw new ElementException(component, "Form component's Model is required and has not been specified.");
        }
        if (_bindingManager.GetMappedEndFor(formModel, "Model").SourceElement?.TypeReference.IsNullable == true)
        {
            var loadingCode = IRazorCodeDirective.Create(new CSharpStatement($"@if ({modelBinding} is null)"), _componentTemplate.RazorFile);
            loadingCode.AddHtmlElement("MudProgressLinear", loadingBar =>
            {
                loadingBar.AddAttribute("Color", "Color.Primary");
                loadingBar.AddAttribute("Indeterminate", "true");
                loadingBar.AddAttribute("Class", "my-7");
            });
            parentNode.AddChildNode(loadingCode);
        }
        var codeBlock = IRazorCodeDirective.Create(new CSharpStatement($"@if ({modelBinding} is not null)"), _componentTemplate.RazorFile);
        parentNode.AddChildNode(codeBlock);
        codeBlock.AddHtmlElement("MudGrid", x => x.AddHtmlElement("MudItem", mudItem =>
        {
            mudItem.AddAttribute("xs", "12").AddAttribute("sm", "7");
            mudItem.AddHtmlElement("MudForm", form =>
            {
                var formField = formModel.Name.ToCSharpIdentifier().ToPrivateMemberName();
                var razorComponentClass = _componentTemplate.GetCodeBehind();
                razorComponentClass.AddField(razorComponentClass.Template.UseType("MudBlazor.MudForm"), formField);
                form.AddAttribute("@ref", $"@{formField}");
                form.AddAttributeIfNotEmpty("Model", modelBinding.ToString());


                var validator = _componentTemplate.ExecutionContext.FindTemplateInstance("Blazor.HttpClient.Contracts.Dto.Validation", modelMapping.SourceElement.TypeReference.Element.Id);
                if (validator != null)
                {
                    _componentTemplate.RazorFile.AddInjectDirective(_componentTemplate.GetTypeName("Blazor.Client.Validation.ValidatorProviderInterface"), "ValidatorProvider");
                    form.AddAttribute("Validation", $"@(ValidatorProvider.GetValidationFunc<{_componentTemplate.GetTypeName((IElement)modelMapping.SourceElement.TypeReference.Element)}>())");
                }

                foreach (var child in formModel.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, form);
                }

                //form.AddAttributeIfNotEmpty("OnValidSubmit", $"{_bindingManager.GetBinding(formModel, "On Valid Submit")?.ToLambda()}");
                //form.AddAttributeIfNotEmpty("OnInvalidSubmit", $"{_bindingManager.GetBinding(formModel, "On Invalid Submit")?.ToLambda()}");
            });
        }));
    }
}