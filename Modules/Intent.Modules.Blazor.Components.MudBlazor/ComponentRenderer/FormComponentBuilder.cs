using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Razor;
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
            var loadingCode = new RazorCodeDirective(new CSharpStatement($"if ({modelBinding} is null)"), _componentTemplate.RazorFile);
            loadingCode.AddHtmlElement("MudProgressLinear", loadingBar =>
            {
                loadingBar.AddAttribute("Color", "Color.Primary");
                loadingBar.AddAttribute("Indeterminate", "true");
                loadingBar.AddAttribute("Class", "my-7");
            });
            parentNode.AddChildNode(loadingCode);
        }
        var codeBlock = new RazorCodeDirective(new CSharpStatement($"if ({modelBinding} is not null)"), _componentTemplate.RazorFile);
        parentNode.AddChildNode(codeBlock);
        codeBlock.AddHtmlElement("MudGrid", x => x.AddHtmlElement("MudItem", mudItem =>
        {
            mudItem.AddAttribute("xs", "12").AddAttribute("sm", "7");
            mudItem.AddHtmlElement("MudForm", form =>
            {
                var formField = formModel.Name.ToCSharpIdentifier().ToCamelCase();
                _componentTemplate.GetCodeBlock().AddField("MudForm", formField);
                form.AddAttribute("@ref", $"@{formField}");
                form.AddAttributeIfNotEmpty("Model", modelBinding.ToString());

                var validator = _componentTemplate.ExecutionContext.FindTemplateInstance("Blazor.HttpClient.Contracts.Dto.Validation", modelMapping.SourceElement.TypeReference.Element.Id);
                if (validator != null)
                {
                    form.AddAttribute("Validation", $"@(ValidatorProvider.GetValidationFunc<{_componentTemplate.GetTypeName(validator)}>())");
                }

                //form.AddAttributeIfNotEmpty("OnValidSubmit", $"{_bindingManager.GetBinding(formModel, "On Valid Submit")?.ToLambda()}");
                //form.AddAttributeIfNotEmpty("OnInvalidSubmit", $"{_bindingManager.GetBinding(formModel, "On Invalid Submit")?.ToLambda()}");

                form.AddHtmlElement("MudCard", card =>
                {
                    var headerModel = formModel.Containers.SingleOrDefault(x => x.Name == "Header");
                    if (headerModel != null)
                    {
                        card.AddHtmlElement("MudCardHeader", cardHeader =>
                        {
                            cardHeader.AddHtmlElement("CardHeaderContent", cardTitle =>
                            {
                                foreach (var child in headerModel.InternalElement.ChildElements)
                                {
                                    _componentResolver.ResolveFor(child).BuildComponent(child, cardTitle);
                                }
                            });
                        });
                    }

                    var bodyModel = formModel.Containers.SingleOrDefault(x => x.Name == "Body");
                    if (bodyModel != null)
                    {
                        card.AddHtmlElement("MudCardContent", cardBody =>
                        {
                            foreach (var child in bodyModel.InternalElement.ChildElements)
                            {
                                _componentResolver.ResolveFor(child).BuildComponent(child, cardBody);
                            }
                        });
                    }

                    var footerModel = formModel.Containers.SingleOrDefault(x => x.Name == "Footer");
                    if (footerModel != null)
                    {
                        card.AddHtmlElement("MudCardActions", cardFooter =>
                        {
                            foreach (var child in footerModel.InternalElement.ChildElements)
                            {
                                _componentResolver.ResolveFor(child).BuildComponent(child, cardFooter);
                            }
                        });
                    }
                });
            });
        }));
    }


}