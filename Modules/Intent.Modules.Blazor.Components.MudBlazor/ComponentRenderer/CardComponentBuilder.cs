using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class CardComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public CardComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var formModel = new CardModel(component);
        var card = new HtmlElement("MudCard", _componentTemplate.RazorFile);
        parentNode.AddChildNode(card);
        var headerModel = formModel.Header;
        if (headerModel != null)
        {
            card.AddHtmlElement("MudCardHeader", cardHeader =>
            {
                cardHeader.AddHtmlElement("CardHeaderContent", cardTitle =>
                {
                    foreach (var child in headerModel.InternalElement.ChildElements)
                    {
                        _componentResolver.BuildComponent(child, cardTitle);
                    }
                });
            });
        }

        var bodyModel = formModel.Content;
        if (bodyModel != null)
        {
            card.AddHtmlElement("MudCardContent", cardBody =>
            {
                foreach (var child in bodyModel.InternalElement.ChildElements)
                {
                    _componentResolver.BuildComponent(child, cardBody);
                }
            });
        }

        var footerModel = formModel.Actions;
        if (footerModel != null)
        {
            card.AddHtmlElement("MudCardActions", cardFooter =>
            {
                foreach (var child in footerModel.InternalElement.ChildElements)
                {
                    _componentResolver.BuildComponent(child, cardFooter);
                }
            });
        }

        return [card];
    }
}