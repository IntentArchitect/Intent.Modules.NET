using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class ButtonComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public ButtonComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var model = new ButtonModel(component);
        IHtmlElement htmlElement = new HtmlElement(model.GetAppearance().IconOnly() ? "MudIconButton" : "MudButton", _componentTemplate.RazorFile);

        var mappingEnds = _bindingManager.GetMappedEndsFor(model, "Link To");
        htmlElement.AddAttributeIfNotEmpty("Href", _bindingManager.GetHrefRoute(mappingEnds));

        htmlElement.AddAttributeIfNotEmpty("Variant", model.GetAppearance()?.Variant() != null ? $"Variant.{model.GetAppearance()?.Variant().Name}" : null)
            .AddAttributeIfNotEmpty(model.GetAppearance().IconOnly() ? "Icon" : "StartIcon", model.HasIcon() ? $"@Icons.Material.{model.GetIcon().Variant().Name}.{model.GetIcon().IconValue().Name}" : null)
            .AddAttributeIfNotEmpty("IconColor", !model.GetAppearance().IconOnly() && model.GetIcon()?.IconColor() != null ? $"Color.{model.GetIcon()?.IconColor().Name}" : null)
            .AddAttributeIfNotEmpty("Class", string.IsNullOrWhiteSpace(model.GetAppearance().Class()) ? "my-2 mr-2" : model.GetAppearance().Class());

        htmlElement.AddAttribute("Color", _bindingManager.GetBinding(model, "3a04c387-3b5b-4a3d-b03a-4ecd0dcc301a", parentNode)?.ToString()
            ?? "Color." + (model.GetAppearance()?.Color()?.Name ?? (model.GetInteraction().Type().IsSubmit() ? "Primary" : "Default")));

        if (component.ChildElements.Any())
        {
            foreach (var child in component.ChildElements)
            {
                _componentResolver.BuildComponent(child, htmlElement);
            }
        }
        else if (!model.GetAppearance().IconOnly())
        {
            htmlElement.AddHtmlElement("MudText", text => text.WithText(!string.IsNullOrWhiteSpace(model.InternalElement.Value) ? model.InternalElement.Value : model.Name));
        }

        var onClickMapping = _bindingManager.GetMappedEndFor(model, "On Click");
        if (onClickMapping != null)
        {
            htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");

            _componentTemplate.RazorFile.AfterBuild(file =>
            {
                _componentTemplate.GetCodeBehind().TryGetReferenceForModel(onClickMapping.SourceElement?.Id ?? string.Empty, out var reference);
                if (reference is CSharpClassMethod method)
                {
                    var form = model.GetInteraction().Form() ?? component.GetParentPath().Reverse().FirstOrDefault(x => x.IsFormModel());
                    if (form != null && model.GetInteraction().Type().IsSubmit())
                    {
                        method.Async();
                        ((IHasCSharpStatements)method.FindStatement(x => x is CSharpTryBlock) ?? method)?.InsertStatements(0,
                            $$"""
                                  await {{form.Name.ToPrivateMemberName()}}!.Validate();
                                  if (!{{form.Name.ToPrivateMemberName()}}.IsValid)
                                  {
                                      return;
                                  }
                                  """.ConvertToStatements().ToList());
                    }

                    if (!method.IsAsync)
                    {
                        return;
                    }

                    var processingFieldName = $"{method.Name}Processing".ToPrivateMemberName();
                    _componentTemplate.GetCodeBehind().AddField("bool", processingFieldName, f => f.WithAssignment("false"));
                    ((CSharpTryBlock)method.FindStatement(x => x is CSharpTryBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "true").WithSemicolon());
                    var finallyBlock = ((CSharpFinallyBlock)method.FindStatement(x => x is CSharpFinallyBlock));
                    if (finallyBlock is null)
                    {
                        finallyBlock = new CSharpFinallyBlock();
                        ((CSharpCatchBlock)method.FindStatement(x => x is CSharpCatchBlock))?.InsertBelow(finallyBlock);
                    }

                    finallyBlock?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "false").WithSemicolon());

                    if (!model.GetAppearance().IconOnly())
                    {
                        htmlElement.AddAttribute("Disabled", $"@{processingFieldName}");
                        htmlElement.InsertCodeBlock(0, $"@if ({processingFieldName})", code =>
                        {
                            code.AddHtmlElement("MudProgressCircular", spinner =>
                            {
                                spinner.AddAttribute("Class", "ms-n1");
                                spinner.AddAttribute("Size", "Size.Small");
                                spinner.AddAttribute("Indeterminate", "true");
                            });
                        });
                    }
                }
            });
        }

        parentNode.AddChildNode(htmlElement);
        return [htmlElement];
    }
}