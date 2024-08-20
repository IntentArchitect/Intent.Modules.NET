using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class DialogComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public DialogComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var dialogModel = new DialogModel(component);
        var htmlElement = new HtmlElement("MudDialog", _componentTemplate.RazorFile);
        parentNode.AddChildNode(htmlElement);
        if (dialogModel.ContentContainer != null)
        {
            htmlElement.AddHtmlElement("DialogContent", content =>
            {
                foreach (var child in dialogModel.ContentContainer.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, content);
                }
            });
        }

        if (dialogModel.ContentContainer != null)
        {
            htmlElement.AddHtmlElement("DialogActions", actions =>
            {
                foreach (var child in dialogModel.ActionsContainer.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(child).BuildComponent(child, actions);
                }
            });
        }

        _componentTemplate.RazorFile.AfterBuild(file =>
        {
            var codeBehind = _componentTemplate.GetCodeBehind();

            codeBehind.AddProperty("MudDialogInstance", "Dialog", p => p.AddAttribute("[CascadingParameter]"));
        });
    }
}