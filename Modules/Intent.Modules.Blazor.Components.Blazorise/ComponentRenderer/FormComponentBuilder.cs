using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.Blazorise.ComponentRenderer;

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
        var modelBinding = _bindingManager.GetBinding(formModel, "Model");
        if (modelBinding == null)
        {
            throw new ElementException(component, "Form component's Model is required and has not been specified.");
        }
        var codeBlock = IRazorCodeDirective.Create(new CSharpStatement($"if ({modelBinding} is not null)"), _componentTemplate.RazorFile);
        codeBlock.AddHtmlElement("EditForm", htmlElement =>
        {
            htmlElement.AddAttributeIfNotEmpty("Model", modelBinding.ToString());
            htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"{_bindingManager.GetBinding(formModel, "On Valid Submit")?.ToLambda()}");
            htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"{_bindingManager.GetBinding(formModel, "On Invalid Submit")?.ToLambda()}");

            htmlElement.AddHtmlElement("Card", card =>
            {
                var headerModel = formModel.Containers.Single(x => x.Name == "Header");
                if (headerModel != null)
                {
                    card.AddHtmlElement("CardHeader", cardHeader =>
                    {
                        cardHeader.AddHtmlElement("CardTitle", cardTitle =>
                        {
                            foreach (var child in headerModel.InternalElement.ChildElements)
                            {
                                _componentResolver.ResolveFor(child).BuildComponent(child, cardTitle);
                            }
                        });
                    });
                }
                var bodyModel = formModel.Containers.Single(x => x.Name == "Body");
                if (bodyModel != null)
                {
                    card.AddHtmlElement("CardBody", cardBody =>
                    {
                        cardBody.AddHtmlElement("Validations", validations =>
                        {
                            validations.AddAttributeIfNotEmpty("Model", _bindingManager.GetBinding(formModel, "Model").ToString());
                            validations.AddAttribute("Mode", "ValidationMode.Auto");
                            validations.AddAttribute("ValidateOnLoad", "false");
                            foreach (var child in bodyModel.InternalElement.ChildElements)
                            {
                                _componentResolver.ResolveFor(child).BuildComponent(child, validations);
                            }
                        });
                    });
                }

                var footerModel = formModel.Containers.Single(x => x.Name == "Footer");
                if (footerModel != null)
                {
                    card.AddHtmlElement("CardFooter", cardFooter =>
                    {
                        foreach (var child in footerModel.InternalElement.ChildElements)
                        {
                            _componentResolver.ResolveFor(child).BuildComponent(child, cardFooter);
                        }
                    });
                }
            });
        });

        parentNode.AddChildNode(codeBlock);

    }


}