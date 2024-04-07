using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazorize.Components.ComponentRenderer;

public class FormComponentRenderer : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderResolver _componentResolver;
    private readonly RazorComponentTemplate _template;

    public FormComponentRenderer(IRazorComponentBuilderResolver componentResolver, RazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _template = template;
    }

    public void BuildComponent(IElement component, IRazorFileNode node)
    {
        var formModel = new FormModel(component);
        var codeBlock = new RazorCodeDirective(new CSharpStatement($"if ({formModel.GetContent()?.Model().Trim('{', '}')} is not null)"), _template.BlazorFile);
        codeBlock.AddHtmlElement("EditForm", htmlElement =>
        {
            htmlElement.AddAttributeIfNotEmpty("Model", _template.GetStereotypePropertyBinding(formModel, "Model"));
            htmlElement.AddAttributeIfNotEmpty("OnValidSubmit", $"async () => await {_template.GetStereotypePropertyBinding(formModel, "On Valid Submit")}");
            htmlElement.AddAttributeIfNotEmpty("OnInvalidSubmit", $"async () => await {_template.GetStereotypePropertyBinding(formModel, "On Invalid Submit")}");

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
                            validations.AddAttributeIfNotEmpty("Model", _template.GetStereotypePropertyBinding(formModel, "Model"));
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

        node.AddNode(codeBlock);

    }
}