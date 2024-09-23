using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var result = new List<IRazorFileNode>();
        var formModel = new FormModel(component);
        var formModelStaticId = "9d16e967-a2ab-45b4-ba69-b286daf4488c";
        var modelMapping = _bindingManager.GetMappedEndFor(formModel, formModelStaticId);
        var modelBinding = _bindingManager.GetBinding(modelMapping);
        if (modelBinding == null)
        {
            throw new ElementException(component, "Form component's Model is required and has not been specified.");
        }
        if (_bindingManager.GetMappedEndFor(formModel, formModelStaticId).SourceElement?.TypeReference.IsNullable == true)
        {
            var loadingCode = IRazorCodeDirective.Create(new CSharpStatement($"@if ({modelBinding} is null)"), _componentTemplate.RazorFile);
            loadingCode.AddHtmlElement("MudProgressLinear", loadingBar =>
            {
                loadingBar.AddAttribute("Color", "Color.Primary");
                loadingBar.AddAttribute("Indeterminate", "true");
                loadingBar.AddAttribute("Class", "my-7");
            });
            parentNode.AddChildNode(loadingCode);
            result.Add(loadingCode);
        }
        var codeBlock = IRazorCodeDirective.Create(new CSharpStatement($"@if ({modelBinding} is not null)"), _componentTemplate.RazorFile);
        parentNode.AddChildNode(codeBlock);
        codeBlock.AddHtmlElement("MudForm", form =>
        {
            var formField = formModel.Name.ToCSharpIdentifier().ToPrivateMemberName();
            var razorComponentClass = _componentTemplate.GetCodeBehind();
            razorComponentClass.AddField(razorComponentClass.Template.UseType("MudBlazor.MudForm"), formField);
            form.AddAttribute("@ref", $"@{formField}");
            form.AddAttributeIfNotEmpty("Model", modelBinding.ToString());


            var validator = _componentTemplate.ExecutionContext.FindTemplateInstance(TemplateRoles.Blazor.Client.Model.Validator, modelMapping.SourceElement.TypeReference.Element.Id);
            if (validator != null)
            {
                _componentTemplate.RazorFile.AddInjectDirective(_componentTemplate.GetTypeName("Blazor.Client.Validation.ValidatorProviderInterface"), "ValidatorProvider");
                form.AddAttribute("Validation", $"@(ValidatorProvider.GetValidationFunc<{_componentTemplate.GetTypeName((IElement)modelMapping.SourceElement.TypeReference.Element)}>())");
            }

            foreach (var child in formModel.InternalElement.ChildElements)
            {
                _componentResolver.BuildComponent(child, form);
            }

            //form.AddAttributeIfNotEmpty("OnValidSubmit", $"{_bindingManager.GetBinding(formModel, "On Valid Submit")?.ToLambda()}");
            //form.AddAttributeIfNotEmpty("OnInvalidSubmit", $"{_bindingManager.GetBinding(formModel, "On Invalid Submit")?.ToLambda()}");
        });
        result.Add(codeBlock);
        return result;
    }
}