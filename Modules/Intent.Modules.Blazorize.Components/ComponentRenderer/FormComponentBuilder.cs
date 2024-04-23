using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class FormComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _template;
    private readonly BindingManager _bindingManager;

    public FormComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var formModel = new FormModel(component);
        var codeBlock = new RazorCodeDirective(new CSharpStatement($"if ({formModel.GetContent()?.Model().Trim('{', '}')} is not null)"), _template.BlazorFile);
        codeBlock.AddHtmlElement("EditForm", htmlElement =>
        {
            htmlElement.AddAttributeIfNotEmpty("Model", _bindingManager.GetStereotypePropertyBinding(formModel, "Model"));
            htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Valid Submit")}");
            htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"async () => await {_bindingManager.GetStereotypePropertyBinding(formModel, "On Invalid Submit")}");

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
                            validations.AddAttributeIfNotEmpty("Model", _bindingManager.GetStereotypePropertyBinding(formModel, "Model"));
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

        node.AddChildNode(codeBlock);

    }
}